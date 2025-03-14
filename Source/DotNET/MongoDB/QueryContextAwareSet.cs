// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Reflection;
using Cratis.Applications.Queries;
using Cratis.Strings;

namespace MongoDB.Driver;

/// <summary>
/// Represents a set that is aware of <see cref="QueryContext"/>.
/// </summary>
/// <remarks>This list is not mutated in a thread-safe way.</remarks>
/// <typeparam name="TDocument">The type of the document.</typeparam>
internal sealed class QueryContextAwareSet<TDocument> : IEnumerable<TDocument>
{
    const byte Value = 0;

    int _numCompares;
    int _expectedNumCompares;
    bool? _replacing;
    bool _adding;
    bool _ignoreDirection;

    readonly IEqualityComparer _idEqualityComparer;
    readonly Func<TDocument, object> _getId;
    SortedDictionary<(object Id, TDocument Document), byte> _items;
    QueryContext? _queryContext;
    int? _maxSize;
    Func<TDocument, object?> _getSortingField = _ => null;
    IComparer _sortingFieldComparer;

    bool _compareForEquality;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryContextAwareSet{TDocument}"/> class.
    /// </summary>
    /// <param name="queryContext">The query context.</param>
    /// <param name="idProperty">The id property.</param>
    public QueryContextAwareSet(QueryContext queryContext, PropertyInfo idProperty)
    {
        _idEqualityComparer = (typeof(EqualityComparer<>)
                .MakeGenericType(idProperty.PropertyType)
                .GetProperty(nameof(EqualityComparer<object>.Default), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null)
            as IEqualityComparer)!;
        ArgumentNullException.ThrowIfNull(_idEqualityComparer);
        _getId = document =>
        {
            var id = idProperty.GetValue(document);
            ArgumentNullException.ThrowIfNull(id);
            return id;
        };
        Initialize(queryContext);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryContextAwareSet{TDocument}"/> class.
    /// </summary>
    /// <param name="queryContext">The query context.</param>
    public QueryContextAwareSet(QueryContext queryContext) : this(queryContext, typeof(TDocument).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)!)
    {
    }

    /// <summary>
    /// Adds item to the set.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(TDocument item)
    {
        _adding = false;
        _replacing = null;
        _numCompares = 0;
        _expectedNumCompares = 0;
        var key = (_getId(item), item);

        _compareForEquality = true;
        if (_items.ContainsKey(key))
        {
            if (SortingIsEnabled())
            {
                _items.Remove(key);
                _compareForEquality = false;
                _items.Add(key, Value);
            }
            else
            {
                _replacing = false;
                _items.Remove(key);
                _replacing = true;
                _items.Add(key, Value);
            }
            return;
        }
        _compareForEquality = false;
        if (_maxSize is null || _items.Count < _maxSize)
        {
            _adding = true;
            _items.Add(key, Value);
            return;
        }

        var minKey = _items.Last().Key;
        _ignoreDirection = true;
        var comparison = _items.Comparer.Compare(key, minKey);
        _ignoreDirection = false;
        if (!SortingIsEnabled() && comparison >= 0)
        {
            return;
        }
        if (SortingIsEnabled())
        {
            var shouldNotAdd = _queryContext.Sorting.Direction is Cratis.Applications.Queries.SortDirection.Descending
                ? comparison <= 0
                : comparison >= 0;
            if (shouldNotAdd)
            {
                return;
            }
        }

        _items.Remove(minKey);
        _adding = true;
        _items.Add(key, Value);
    }

    public IEnumerator<TDocument> GetEnumerator()
    {
        return _items.Keys.Select(key => key.Document).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    void Initialize(QueryContext newQueryContext)
    {
        var oldQueryContext = _queryContext;
        _queryContext = newQueryContext;
        if (_queryContext.Paging.IsPaged)
        {
            _maxSize = _queryContext.Paging.Size;
        }

        if (_maxSize < 1)
        {
            throw new ArgumentException("Page size must be greater than 0", nameof(newQueryContext));
        }
        var createNewStorage = oldQueryContext?.Paging.IsPaged == true && _maxSize < oldQueryContext.Paging.Size;
        if (oldQueryContext is not null && oldQueryContext.Sorting != Sorting.None &&
            (newQueryContext.Sorting.Direction != oldQueryContext.Sorting.Direction || newQueryContext.Sorting.Field != oldQueryContext.Sorting.Field))
        {
            createNewStorage = true;
        }

        _sortingFieldComparer = Comparer<object>.Default;

        if (SortingIsEnabled())
        {
            var sortingFieldProperty = typeof(TDocument).GetProperty(_queryContext.Sorting.Field.ToPascalCase(), BindingFlags.Instance | BindingFlags.Public);
            if (sortingFieldProperty is null)
            {
                throw new ArgumentException($"Sorting field could not be found on {typeof(TDocument)}", nameof(newQueryContext));
            }
            _sortingFieldComparer = (typeof(Comparer<>)
                .MakeGenericType(sortingFieldProperty.PropertyType)
                .GetProperty(nameof(Comparer<object>.Default), BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null)
                as IComparer)!;
            _getSortingField = document =>
            {
                try
                {
                    return sortingFieldProperty.GetValue(document);
                }
                catch (Exception)
                {
                    return null;
                }
            };
        }

        if (_items is null || createNewStorage)
        {
            _items = CreateNewStorage();
        }
    }

    SortedDictionary<(object Id, TDocument Document), byte> CreateNewStorage() => new(CreateComparer());

    Comparer<(object Id, TDocument Document)> CreateComparer() => Comparer<(object Id, TDocument Document)>.Create((x, y) =>
    {
        if (!SortingIsEnabled() || _compareForEquality)
        {
            var compareValue = _idEqualityComparer.Equals(x.Id, y.Id) ? 0 : 1;
            if (_replacing.HasValue)
            {
                if (_replacing.Value && _numCompares == _expectedNumCompares)
                {
                    return -1;
                }
                if (compareValue != 0)
                {
                    if (_replacing.Value)
                    {
                        _numCompares++;
                    }
                    else
                    {
                        _expectedNumCompares++;
                    }
                }
            }
            return compareValue;
        }
        var sortingFieldX = _getSortingField(x.Document);
        var sortingFieldY = _getSortingField(y.Document);
        var comparison = _sortingFieldComparer.Compare(sortingFieldX, sortingFieldY);
        if (!_ignoreDirection)
        {
            comparison = _queryContext!.Sorting.Direction is Cratis.Applications.Queries.SortDirection.Descending
                ? comparison * -1
                : comparison;
        }
        if (comparison == 0 && _adding)
        {
            return 1;
        }
        return comparison;
    });

    bool SortingIsEnabled() => _queryContext.Sorting != Sorting.None;
}