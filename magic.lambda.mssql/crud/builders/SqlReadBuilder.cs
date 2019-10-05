﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Linq;
using System.Text;
using magic.node;
using magic.node.extensions;
using com = magic.data.common;
using magic.signals.contracts;

namespace magic.lambda.mssql.crud.builders
{
    /// <summary>
    /// Builder for creating a read type of MS SQL Server statement,
    /// </summary>
    public class SqlReadBuilder : com.SqlReadBuilder
    {
        /// <summary>
        /// Creates en instance of your type.
        /// </summary>
        /// <param name="node">Arguments to create your SQL from.</param>
        /// <param name="signaler">Signaler used to invoke your original slot.</param>
        public SqlReadBuilder(Node node, ISignaler signaler)
            : base(node, signaler, "\"")
        { }

        /// <summary>
        /// Appends the "tail" parts of your SQL into the specified builder.
        /// </summary>
        /// <param name="builder">Builder where to put the tail.</param>
        protected override void GetTail(StringBuilder builder)
        {
            // Getting [order].
            GetOrderBy(builder);

            var offsetNodes = Root.Children.Where(x => x.Name == "offset");
            if (offsetNodes.Any())
            {
                // Sanity checking.
                if (offsetNodes.Count() > 1)
                    throw new ApplicationException($"syntax error in '{GetType().FullName}', too many [offset] nodes");

                var offsetValue = offsetNodes.First().GetEx<long>();
                builder.Append(" offset " + offsetValue + " rows");
            }

            // Getting [limit].
            var limitNodes = Root.Children.Where(x => x.Name == "limit");
            if (limitNodes.Any())
            {
                // Sanity checking.
                if (limitNodes.Count() > 1)
                    throw new ApplicationException($"syntax error in '{GetType().FullName}', too many [limit] nodes");

                var limitValue = limitNodes.First().GetEx<long>();
                builder.Append(" fetch next " + limitValue + " rows only");
            }
            else
            {
                // Defaulting to 25 records, unless [limit] was explicitly given.
                builder.Append(" fetch next 25 rows only");
            }
        }
    }
}
