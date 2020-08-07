
# Magic Lambda MS SQL Server data adapters

[![Build status](https://travis-ci.org/polterguy/magic.lambda.mssql.svg?master)](https://travis-ci.org/polterguy/magic.lambda.mssql)

These are the MS SQL Server data adapters for [Magic](https://github.com/polterguy/magic). They allow you to provide a semantic
lambda strucutre to its slots, which in turn will dynamically create a MS SQL dialectic SQL statement for you, for all basic
types of SQL statements. In addition, it provides slots to open a MS SQL database connection, and such, to allow you to
declare your own SQL statements, to be executed towards a MS SQL database. Slots this project encapsulates are as follows.

* __[mssql.connect]__ - Connects to a database, either taking an entire connection string, or a reference to a configuration connection string.
* __[mssql.create]__ - Creates a single redorc in the specified table.
* __[mssql.delete]__ - Deletes a single record in the specified table.
* __[mssql.read]__ - Reads multiple records from the specified table.
* __[mssql.update]__ - Updates a single record in the specified table.
* __[mssql.select]__ - Executes an arbitrary SQL statement, and returns results of reader as lambda object to caller.
* __[mssql.scalar]__ - Executes an arbitrary SQL statement, and returns the result as a scalar value to caller.
* __[mssql.execute]__ - Executes an aribitrary SQL statement.
* __[mssql.execute-batch]__ - Executes a _"batch"_ of SQL statements, where each statement is separated by the word _"GO"_.
* __[mssql.transaction.create]__ - Creates a new transaction, that will be explicitly rolled back as execution leaves scope, unless __[mssql.transaction.commit]__ is explicitly called before leaving scope.
* __[mssql.transaction.commit]__ - Explicitly commits an open transaction.
* __[mssql.transaction.rollback]__ - Explicitly rolls back an open transaction.

Most of the above slots also have async (wait.) overloads.

## mssql.read

This event allows you to read records in CRUD style, implying you don't need to create your own SQL, but the slot will automatically
create your SQL for you, avoding things such as SQL insertion attacks automatically for you, etc.

```
/*
 * Connecting to Northwind database
 */
mssql.connect:[Northwind]

   /*
    * Selecting only ContactName from Customers table
    */
   mssql.read
      table:Customers
      columns
         ContactName
```

Notice, if you want to inspect the SQL that is generated, you can pass in **[generate]** and set its value to boolean true.

## Conditional select, update, delete

The __[mssql.delete]__, __[mssql.read]__ and __[mssql.update]__ slots can be given relatively complex where conditions, where you apply
conditions to these as a __[where]__ node. This will become a part of the SQL _"where"_ clause, where each condition by default will
be _"AND'ed"_ together, but this too can be changed by adding YALOA that declares your logical operator. For instance, to select
all records that have a `value` of _5_ and an `content` of _"foo"_ you can do something like the following

```
mssql.read
   table:SomeTable
   where
      value:int:5
      content:foo
```

The above invocation will return all records that have _both_ a _"value"_ of _"5"_, and a _"content"_ of _"foo"_. You can optionally apply
a logical operator to it, to change it to becoming an _"OR"_ SQL where clause, by adding an _"OR"_ node in between the __[where]__ and
the values, such as the following illustrates.

```
mssql.read
   table:SomeTable
   where
      or
         value:int:5
         content:foo
```

The above will return all records that have _either_ a value of _"5"_ OR a _"content"_ of _"foo"_. To understand how the above logic works,
it might be useful to play around with the _"Evaluator"_ in the Magic frontend, and make sure you add __[generate]__ and set its value
to boolean _"true"_, which will return the resulting SQL, instead of actually evaluating the SQL.

Both select, delete and read slots have the same logic when it comes to creating _"where"_ clauses and attaching these to your resulting SQL.

## More complex operators

In addition to the above, you can also supply any operators, in two ways in fact, which becomes your comparison operators. Below is its most
simple example.

```
mssql.read
   table:SomeTable
   where
      and
         !=
            value:int:5
            content:foo
```

The above will return all records which does _not_ have a _"value"_ of 5, nor a _"content"_ of _"foo"_. Supported operators are as follows.

* !=
* <
* \>
* \>=
* <=
* =
* like

In addition to the above operators, you can supply an _"in"_ operator, which is structurally different, though similar in logic.
Below is an example.

```
mssql.read
   table:SomeTable
   where
      or
         in
            value
               :long:5
               :long:7
```

The above will return all records where its _"value_" is either equal to 5 or 7. You can create as many _"in"_ values as you wish, but corrrently
only integer (long, int types of columns) are supported.

All operators are also supported as _"column-name.operator-name"_ type of arguments, such as the following illustrates.

```
mssql.read
   table:SomeTable
   where
      and
         foo1.like:query%
         foo2.mt:int:5
         foo3.lt:int:5
         foo4.mteq:int:5
         foo5.lteq:int:5
         foo6.neq:int:5
         foo7.eq:int:5
```

_"eq"_ implies _"equals"_, _"lt"_ implies _"less than"_, _"mt"_ implies _"more than"_, _"neq"_ implies _"not equal to"_, etc. All combinations
of previous said words, becomes the equivalent combinatory operator.

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 7 days after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
