using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace C3R.MiniAdo.SqlServer
{
    class MsSqlQuery : Query
    {
        /// <summary>
        /// Trie for interpreting Query's params
        /// </summary>
        private class Trie
        {
            /// <summary>
            /// Gets and Sets Trie node's character (value of Trie node)
            /// </summary>
            public char Char { get; set; }

            /// <summary>
            /// Gets dictionary of Trie's children
            /// </summary>
            public Dictionary<char, Trie> Children { get; private set; } = new Dictionary<char, Trie>();

            /// <summary>
            /// Gets if Trie node is leaf node
            /// </summary>
            public bool IsLeaf
            {
                get { return Children.Count == 0; }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="val"></param>
            public Trie(char val)
            {
                this.Char = val;
            }

            /// <summary>
            /// Adds the given string to Trie structure starting from current node
            /// </summary>
            /// <param name="s">String to add</param>
            public void Add(string s)
            {
                var cur = this;

                foreach (var ch in s)
                {
                    if (!cur.Children.ContainsKey(ch)) cur.Children[ch] = new Trie(ch);
                    cur = cur.Children[ch];
                }
            }
        }

        /// <summary>
        /// Query index counter for creating unique index when rebuilding queries.
        /// </summary>
        private volatile int _counter;

        public MsSqlQuery(DataContext context, string cmd = null, CommandType cmdType = CommandType.Text) : base(context, cmd, cmdType)
        {
        }

        public override IQuery Merge(IQuery query)
        {
            return MergeInternal(this, query);
        }

        /// <summary>
        /// Merges given query into current query
        /// </summary>
        /// <param name="query">Query to be merged into current query</param>
        /// <returns></returns>
        protected virtual IQuery MergeInternal(IQuery query1, IQuery query2)
        {
            var mssqlQuery1 = (MsSqlQuery)query1;
            var mssqlQuery2 = (MsSqlQuery)query2;
            var rebuilt1 = RebuildCommand(query1.QueryText, query1.GetParams().Cast<SqlParameter>(), query1.CommandType);
            var rebuilt2 = RebuildCommand(query2.QueryText, query2.GetParams().Cast<SqlParameter>(), query2.CommandType);
            MsSqlQuery result = null;

            lock (this.Command)
            {
                var currentQuery = Command.CommandText;

                var newCmd = string.Join(";", new[] {
                    ((string)rebuilt1[0]).Trim(';'),
                    ((string)rebuilt2[0]).Trim(';')
                });

                result = new MsSqlQuery(this.Context, newCmd, CommandType.Text);

                foreach (var p in (IEnumerable<SqlParameter>)rebuilt1[1])
                {
                    result.Param(p);
                }

                foreach (var p in (IEnumerable<SqlParameter>)rebuilt2[1])
                {
                    result.Param(p);
                }
            }

            return result;
        }

        /// <summary>
        /// Rebuilds a Command object of from given text and parameter, 
        /// makes it unique to integrate into current query
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <param name="cmdType"></param>
        /// <returns></returns>
        private object[] RebuildCommand(string cmdText, IEnumerable<SqlParameter> parameters, CommandType cmdType)
        {
            var tokens = TokenizeCommand(cmdText, parameters);
            var builder = new StringBuilder();

            if (cmdType == CommandType.StoredProcedure) builder.Append("exec ");
            var newParams = new Dictionary<string, SqlParameter>();

            var paramDic = parameters.ToDictionary(p => p.ParameterName, p => p);
            var paramNameMap = paramDic.ToDictionary(kvp => kvp.Key, kvp =>
            {
                var counter = Interlocked.Increment(ref _counter);
                return $"@p{counter}";
            });

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                var previousToken = i > 0 ? tokens[i - 1] : null;
                var nextToken = i < tokens.Length - 1 ? tokens[i + 1] : null;

                if (token.StartsWith("@") && paramDic.ContainsKey(token) &&
                    !IsQueryVariable(token, previousToken, nextToken))
                {
                    var param = paramDic[token];
                    var newName = paramNameMap[token];
                    if (!newParams.ContainsKey(newName))
                    {
                        newParams.Add(newName, new SqlParameter(newName, param.SqlDbType, param.Size)
                        {
                            SqlValue = param.SqlValue
                        });
                    }
                    builder.Append(newName);
                }
                else
                {
                    builder.Append(token);
                }
            }

            if (cmdType == CommandType.StoredProcedure && paramNameMap.Count > 0)
            {
                builder.Append(" ");
                foreach (var kvp in paramNameMap)
                {
                    if (!newParams.ContainsKey(kvp.Value))
                    {
                        var param = paramDic[kvp.Key];
                        newParams.Add(kvp.Value, new SqlParameter(kvp.Value, param.SqlDbType, param.Size)
                        {
                            SqlValue = param.SqlValue
                        });
                    }
                    builder.Append($"{kvp.Key}={kvp.Value},");
                }
                builder.Remove(builder.Length - 1, 1);
            }

            return new object[] {
                builder.ToString(),
                newParams.Values.ToArray()
            };
        }

        /// <summary>
        /// Detects if the given name is a variable in query text
        /// </summary>
        /// <returns></returns>
        private bool IsQueryVariable(string paramName, string previousToken, string nextToken)
        {
            return (previousToken != null && previousToken.Trim().ToLower() == "declare") ||
                   (nextToken != null && nextToken.Trim().StartsWith("="));
        }

        /// <summary>
        /// Builds a Trie from parameter list
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private Trie GetParameterTrie(IEnumerable<SqlParameter> parameters)
        {
            var root = new Trie('@');

            foreach (var p in parameters)
            {
                root.Add(p.ParameterName.Substring(1));
            }

            return root;
        }

        /// <summary>
        /// Splits cmdText into chunks(tokens) by parameter names
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string[] TokenizeCommand(string cmdText, IEnumerable<SqlParameter> parameters)
        {
            var tokens = new List<string>();
            var trie = GetParameterTrie(parameters);
            Trie curTrie = null;
            var builder = new StringBuilder();
            var isMatchingParam = false;

            foreach (var ch in cmdText.Trim(';'))
            {
                if (isMatchingParam)
                {
                    if (IsValidParameterChar(ch, builder.Length == 1))
                    {
                        if (curTrie.IsLeaf || !curTrie.Children.ContainsKey(ch))
                        {
                            tokens.Add(builder.ToString());
                            isMatchingParam = false;
                            builder.Clear();
                        }
                        else
                        {
                            curTrie = curTrie.Children[ch];
                        }
                    }
                    else
                    {
                        tokens.Add(builder.ToString());
                        isMatchingParam = false;
                        builder.Clear();
                    }
                }
                else if (ch == '@')
                {
                    tokens.Add(builder.ToString());
                    isMatchingParam = true;
                    builder.Clear();
                    curTrie = trie;
                }

                builder.Append(ch);
            }

            if (builder.Length > 0) tokens.Add(builder.ToString());

            return tokens.ToArray();
        }

        /// <summary>
        /// Checks if given character is valid 
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="isFirst"></param>
        /// <returns></returns>
        private bool IsValidParameterChar(char ch, bool isFirst)
        {
            return (isFirst ? Char.IsLetter(ch) : Char.IsLetterOrDigit(ch)) ||
                ch == '@' || ch == '#' || ch == '_';
        }
    }
}
