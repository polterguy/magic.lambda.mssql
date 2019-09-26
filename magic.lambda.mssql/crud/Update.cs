﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Linq;
using System.Data.SqlClient;
using magic.node;
using magic.signals.contracts;
using magic.lambda.mssql.utilities;
using magic.lambda.mssql.crud.builders;

namespace magic.lambda.mysql.crud
{
    [Slot(Name = "mssql.update")]
    public class Update : ISlot
    {
        public void Signal(ISignaler signaler, Node input)
        {
            var builder = new SqlUpdateBuilder(input, signaler);
            var sqlNode = builder.Build();

            // Checking if this is a "build only" invocation.
            if (builder.IsGenerateOnly)
            {
                input.Value = sqlNode.Value;
                input.Clear();
                input.AddRange(sqlNode.Children.ToList());
                return;
            }

            // Executing SQL, now parametrized.
            Executor.Execute(sqlNode, signaler.Peek<SqlConnection>("mssql-connection"), signaler, (cmd) =>
            {
                input.Value = cmd.ExecuteNonQuery();
                input.Clear();
            });
        }
    }
}
