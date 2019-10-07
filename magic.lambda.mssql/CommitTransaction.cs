﻿/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using magic.node;
using magic.signals.contracts;
using magic.lambda.mssql.utilities;

namespace magic.lambda.mysql
{
    /// <summary>
    /// [mssql.transaction.commit] slot for committing the top level MS SQL
    /// database transaction.
    /// </summary>
	[Slot(Name = "mssql.transaction.commit")]
	public class CommitTransaction : ISlot
	{
        /// <summary>
        /// Handles the signal for the class.
        /// </summary>
        /// <param name="signaler">Signaler used to signal the slot.</param>
        /// <param name="input">Root node for invocation.</param>
		public void Signal(ISignaler signaler, Node input)
		{
            signaler.Peek<Transaction>("mssql.transaction").Commit();
        }
	}
}
