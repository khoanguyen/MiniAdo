using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace C3R.MiniAdo.SqlServer
{
    class MsSqlMergedQuery : MsSqlQuery
    {
        private class Trie
        {
            public char Char { get; set; }
            public Dictionary<char, Trie> Children { get; private set; } = new Dictionary<char, Trie>();
            public bool IsLeaf
            {
                get { return Children.Count == 0; }
            }

            public Trie(char val)
            {
                this.Char = val;
            }

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
        private volatile int _counter;

        public MsSqlMergedQuery(DataContext context, MsSqlQuery query1, MsSqlQuery query2) : base(context, "", CommandType.Text)
        {
            this.Merge(query1);
            this.Merge(query2);
        }

        public override IQuery Merge(IQuery query)
        {
            var mssqlQuery = (MsSqlQuery)query;
            var rebuilt = RebuildCommand(query.QueryText, query.GetParams().Cast<SqlParameter>(), query.CommandType);

            lock (this.Command)
            {
                var newQuery = (string)rebuilt[0];
                var currentQuery = Command.CommandText;

                Command.CommandText = string.Join(";", new[] {
                    currentQuery.TrimEnd(';'),
                    newQuery.TrimStart(';')
                });

                foreach (var p in (IEnumerable<SqlParameter>)rebuilt[1])
                {
                    this.Command.Parameters.Add(p);
                }
            }

            return this;
        }

        private object[] RebuildCommand(string cmdText, IEnumerable<SqlParameter> parameters, CommandType cmdType)
        {
            var tokens = TokenizeCommand(cmdText,parameters);
            var builder = new StringBuilder();
            var counter = Interlocked.Increment(ref _counter);

            if (cmdType == CommandType.StoredProcedure) builder.Append("exec ");
            var newParams = new Dictionary<string, SqlParameter>();

            var paramDic = parameters.ToDictionary(p => p.ParameterName, p => p);
            var paramNameMap = paramDic.ToDictionary(kvp => kvp.Key, kvp => $"{kvp.Key}_{counter}");

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

        private Trie GetParameterTrie(IEnumerable<SqlParameter> parameters)
        {
            var root = new Trie('@');

            foreach (var p in parameters)
            {
                root.Add(p.ParameterName.Substring(1));
            }

            return root;
        }

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

        private bool IsValidParameterChar(char ch, bool isFirst)
        {
            return (isFirst ? Char.IsLetter(ch) : Char.IsLetterOrDigit(ch)) ||
                ch == '@' || ch == '#' || ch == '_';
        }
    }
}
