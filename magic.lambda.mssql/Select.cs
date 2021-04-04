﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using System.Threading.Tasks;
using magic.node;
using magic.data.common;
using magic.node.extensions;
using magic.signals.contracts;
using magic.lambda.mssql.helpers;

namespace magic.lambda.mssql
{
    /// <summary>
    /// [mssql.select] slot, for executing a select type of SQL, returning
    /// data rows to the caller.
    /// </summary>
    [Slot(Name = "mssql.select")]
    public class Select : ISlot, ISlotAsync
    {
        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        public void Signal(ISignaler signaler, Node input)
        {
            var mrsNode = input.Children.FirstOrDefault(x => x.Name == "multiple-result-sets");
            var multipleResultSets = mrsNode?.GetEx<bool>() ?? false;
            mrsNode?.UnTie();
            Executor.Execute(
                input,
                signaler.Peek<SqlConnectionWrapper>("mssql.connect").Connection,
                signaler.Peek<Transaction>("mssql.transaction"),
                (cmd, max) =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    do
                    {
                        Node parentNode;
                        if (multipleResultSets)
                        {
                            parentNode = new Node();
                            input.Add(parentNode);
                        }
                        else
                        {
                            parentNode = input;
                        }
                        while (reader.Read())
                        {
                            if (max != -1 && max-- == 0)
                                break; // Reached maximum limit
                            var rowNode = new Node();
                            for (var idxCol = 0; idxCol < reader.FieldCount; idxCol++)
                            {
                                var colNode = new Node(reader.GetName(idxCol), magic.data.common.Converter.GetValue(reader[idxCol]));
                                rowNode.Add(colNode);
                            }
                            parentNode.Add(rowNode);
                        }
                    } while (multipleResultSets && reader.NextResult());
                }
            });
        }

        /// <summary>
        /// Implementation of your slot.
        /// </summary>
        /// <param name="signaler">Signaler used to raise the signal.</param>
        /// <param name="input">Arguments to your slot.</param>
        /// <returns>An awaitable task.</returns>
        public async Task SignalAsync(ISignaler signaler, Node input)
        {
            var mrsNode = input.Children.FirstOrDefault(x => x.Name == "multiple-result-sets");
            var multipleResultSets = mrsNode?.GetEx<bool>() ?? false;
            mrsNode?.UnTie();
            await Executor.ExecuteAsync(
                input,
                signaler.Peek<SqlConnectionWrapper>("mssql.connect").Connection,
                signaler.Peek<Transaction>("mssql.transaction"),
                async (cmd, max) =>
            {
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    do
                    {
                        Node parentNode;
                        if (multipleResultSets)
                        {
                            parentNode = new Node();
                            input.Add(parentNode);
                        }
                        else
                        {
                            parentNode = input;
                        }
                        while (await reader.ReadAsync())
                        {
                            if (max != -1 && max-- == 0)
                                break; // Reached maximum limit
                            var rowNode = new Node();
                            for (var idxCol = 0; idxCol < reader.FieldCount; idxCol++)
                            {
                                var colNode = new Node(reader.GetName(idxCol), magic.data.common.Converter.GetValue(reader[idxCol]));
                                rowNode.Add(colNode);
                            }
                            parentNode.Add(rowNode);
                        }
                    } while (multipleResultSets && await reader.NextResultAsync());
                }
            });
        }
    }
}
