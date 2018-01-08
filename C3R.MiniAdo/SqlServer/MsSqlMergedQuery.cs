using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace C3R.MiniAdo.SqlServer
{
    /// <summary>
    /// MsSqlQuery that provides Query Merging feature
    /// </summary>
    class MsSqlMergedQuery : MsSqlQuery
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="query1"></param>
        /// <param name="query2"></param>
        public MsSqlMergedQuery(DataContext context, MsSqlQuery query1, MsSqlQuery query2) : base(context, "", CommandType.Text)
        {
            this.Merge(query1);
            this.Merge(query2);
        }

        /// <summary>
        /// Merges given query into current query
        /// </summary>
        /// <param name="query">Query to be merged into current query</param>
        /// <returns></returns>
        public override IQuery Merge(IQuery query)
        {
            var mssqlQuery = (MsSqlQuery)query;
            var rebuilt = RebuildCommand(query.QueryText, query.GetParams().Cast<SqlParameter>(), query.CommandType);

            lock (this.Command)
            {
                var newQuery = (string)rebuilt[0];
                var currentQuery = Command.CommandText;

                Command.CommandText = string.Join(";", new[] {
                    currentQuery.Trim(';'),
                    newQuery.Trim(';')
                });

                foreach (var p in (IEnumerable<SqlParameter>)rebuilt[1])
                {
                    this.Command.Parameters.Add(p);
                }
            }

            return this;
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
            var paramNameMap = paramDic.ToDictionary(kvp => kvp.Key, kvp => {
                var counter = Interlocked.Increment(ref _counter);
                return $"@p{counter}";
            });

            foreach (var token in tokens)
            {
                if (token.StartsWith("@") && paramDic.ContainsKey(token))
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
                    if (!newParams.ContainsKey(kvp.Key))
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
