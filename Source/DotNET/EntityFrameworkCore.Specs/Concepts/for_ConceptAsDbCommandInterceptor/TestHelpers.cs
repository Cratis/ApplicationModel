// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Data;
using System.Data.Common;
using Cratis.Concepts;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor;

#pragma warning disable SA1402 // Single type per file

public record TestStringConcept(string Value) : ConceptAs<string>(Value)
{
    public static readonly TestStringConcept Empty = new(string.Empty);
    public static implicit operator TestStringConcept(string value) => new(value);
}

public record TestIntConcept(int Value) : ConceptAs<int>(Value)
{
    public static readonly TestIntConcept NotSet = new(0);
    public static implicit operator TestIntConcept(int value) => new(value);
}

public class TestDbCommand : DbCommand
{
    readonly List<DbParameter> _parameters = [];

    public override string CommandText { get; set; } = string.Empty;
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override bool DesignTimeVisible { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    protected override DbConnection? DbConnection { get; set; }
    protected override DbParameterCollection DbParameterCollection => new TestDbParameterCollection(_parameters);
    protected override DbTransaction? DbTransaction { get; set; }

    public override void Cancel() { }
    public override int ExecuteNonQuery() => 0;
    public override object ExecuteScalar() => null!;
    protected override DbParameter CreateDbParameter() => new TestDbParameter();
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => null!;
    public override void Prepare() { }
}

public class TestDbParameter : DbParameter
{
    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; }
    public override bool IsNullable { get; set; }
    public override string ParameterName { get; set; } = string.Empty;
    public override int Size { get; set; }
    public override string SourceColumn { get; set; } = string.Empty;
    public override bool SourceColumnNullMapping { get; set; }
    public override object? Value { get; set; }
    public override void ResetDbType() { }
}

public class TestDbParameterCollection(List<DbParameter> parameters) : DbParameterCollection
{
    public override int Count => parameters.Count;
    public override object SyncRoot => parameters;

    public override int Add(object value)
    {
        parameters.Add((DbParameter)value);
        return parameters.Count - 1;
    }

    public override void AddRange(Array values)
    {
        foreach (DbParameter param in values)
        {
            parameters.Add(param);
        }
    }

    public override void Clear() => parameters.Clear();
    public override bool Contains(object value) => parameters.Contains((DbParameter)value);
    public override bool Contains(string value) => parameters.Exists(p => p.ParameterName == value);
    public override void CopyTo(Array array, int index) => parameters.CopyTo((DbParameter[])array, index);
    public override System.Collections.IEnumerator GetEnumerator() => parameters.GetEnumerator();
    public override int IndexOf(object value) => parameters.IndexOf((DbParameter)value);
    public override int IndexOf(string parameterName) => parameters.FindIndex(p => p.ParameterName == parameterName);
    public override void Insert(int index, object value) => parameters.Insert(index, (DbParameter)value);
    public override void Remove(object value) => parameters.Remove((DbParameter)value);
    public override void RemoveAt(int index) => parameters.RemoveAt(index);
    public override void RemoveAt(string parameterName) => parameters.RemoveAll(p => p.ParameterName == parameterName);
    protected override DbParameter GetParameter(int index) => parameters[index];
    protected override DbParameter GetParameter(string parameterName) => parameters.First(p => p.ParameterName == parameterName);
    protected override void SetParameter(int index, DbParameter value) => parameters[index] = value;
    protected override void SetParameter(string parameterName, DbParameter value)
    {
        var index = IndexOf(parameterName);
        if (index >= 0) parameters[index] = value;
    }
}
