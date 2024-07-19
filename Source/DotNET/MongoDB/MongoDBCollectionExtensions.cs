// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Reflection;
using Cratis.Applications;
using Cratis.Applications.Queries;
using Cratis.Concepts;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;

namespace MongoDB.Driver;

/// <summary>
/// Extension methods for <see cref="IMongoCollection{TDocument}"/>.
/// </summary>
public static class MongoDBCollectionExtensions
{
    /// <summary>
    /// Find a single document based on Id.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="id">Id of document.</param>
    /// <typeparam name="T">Type of document.</typeparam>
    /// <typeparam name="TId">Type of identifier.</typeparam>
    /// <returns>The document if found, default if not.</returns>
    public static T? FindById<T, TId>(this IMongoCollection<T> collection, TId id) =>
        collection.Find(Builders<T>.Filter.Eq(new StringFieldDefinition<T, TId>("_id"), id)).SingleOrDefault();

    /// <summary>
    /// Find a single document based on Id - asynchronous.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="id">Id of document.</param>
    /// <typeparam name="T">Type of document.</typeparam>
    /// <typeparam name="TId">Type of identifier.</typeparam>
    /// <returns>The document if found, default if not.</returns>
    public static async Task<T?> FindByIdAsync<T, TId>(this IMongoCollection<T> collection, TId id)
    {
        var result = await collection.FindAsync(Builders<T>.Filter.Eq(new StringFieldDefinition<T, TId>("_id"), id));
        return result.SingleOrDefault();
    }

    /// <summary>
    /// Create an observable query that will observe the collection for changes matching the filter criteria.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="filter">Optional filter.</param>
    /// <param name="options">Optional options.</param>
    /// <typeparam name="TDocument">Type of document in the collection.</typeparam>
    /// <returns>Async Task holding <see cref="Subject{T}"/> with a collection of the type for the collection.</returns>
    public static async Task<ISubject<IEnumerable<TDocument>>> Observe<TDocument>(
        this IMongoCollection<TDocument> collection,
        Expression<Func<TDocument, bool>>? filter,
        FindOptions<TDocument, TDocument>? options = null)
    {
        filter ??= _ => true;
        return await collection.Observe(
            () => collection.FindAsync(filter, options),
            filter,
            documents => new BehaviorSubject<IEnumerable<TDocument>>(documents),
            (cursor, observable) => observable.OnNext(cursor.ToList()));
    }

    /// <summary>
    /// Create an observable query that will observe the collection for changes matching the filter criteria.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="filter">Optional filter.</param>
    /// <param name="options">Optional options.</param>
    /// <typeparam name="TDocument">Type of document in the collection.</typeparam>
    /// <returns>Async Task holding <see cref="Subject{T}"/> with a collection of the type for the collection.</returns>
    public static async Task<ISubject<TDocument>> ObserveSingle<TDocument>(
        this IMongoCollection<TDocument> collection,
        Expression<Func<TDocument, bool>>? filter,
        FindOptions<TDocument, TDocument>? options = null)
    {
        filter ??= _ => true;
        return await collection.ObserveSingle(() => collection.FindAsync(filter, options), filter);
    }

    /// <summary>
    /// Create an observable query that will observe the collection for changes matching the filter criteria.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="filter">Optional filter.</param>
    /// <param name="options">Optional options.</param>
    /// <typeparam name="TDocument">Type of document in the collection.</typeparam>
    /// <returns>Async Task holding <see cref="Subject{T}"/> with a collection of the type for the collection.</returns>
    public static async Task<ISubject<IEnumerable<TDocument>>> Observe<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument>? filter = null,
        FindOptions<TDocument, TDocument>? options = null)
    {
        filter ??= FilterDefinition<TDocument>.Empty;
        return await collection.Observe(
            () => collection.FindAsync(filter, options),
            filter,
            documents => new BehaviorSubject<IEnumerable<TDocument>>(documents),
            (documents, observable) => observable.OnNext(documents));
    }

    /// <summary>
    /// Create an observable query that will observe the collection for changes matching the filter criteria.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="filter">Optional filter.</param>
    /// <param name="options">Optional options.</param>
    /// <typeparam name="TDocument">Type of document in the collection.</typeparam>
    /// <returns>Async Task holding <see cref="Subject{T}"/> with a collection of the type for the collection.</returns>
    public static async Task<ISubject<TDocument>> ObserveSingle<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument>? filter = null,
        FindOptions<TDocument, TDocument>? options = null)
    {
        filter ??= FilterDefinition<TDocument>.Empty;
        return await collection.ObserveSingle(() => collection.FindAsync(filter, options), filter);
    }

    /// <summary>
    /// Create an observable query that will observe a single document based on Id of the document in the collection for changes matching the filter criteria.
    /// </summary>
    /// <param name="collection"><see cref="IMongoCollection{T}"/> to extend.</param>
    /// <param name="id">The identifier of the document to observe.</param>
    /// <typeparam name="TDocument">Type of document in the collection.</typeparam>
    /// <typeparam name="TId">Type of id - key.</typeparam>
    /// <returns>Async Task holding <see cref="Subject{T}"/> with an instance of the type.</returns>
    public static async Task<ISubject<TDocument>> ObserveById<TDocument, TId>(this IMongoCollection<TDocument> collection, TId id)
    {
        var filter = Builders<TDocument>.Filter.Eq(new StringFieldDefinition<TDocument, TId>("_id"), id);
        return await collection.ObserveSingle(() => collection.FindAsync(filter), filter);
    }

    static async Task<ISubject<TDocument>> ObserveSingle<TDocument>(
         this IMongoCollection<TDocument> collection,
         Func<Task<IAsyncCursor<TDocument>>> findCall,
         FilterDefinition<TDocument> filter)
    {
        return await collection.Observe<TDocument, TDocument>(
            findCall,
            filter,
            documents =>
            {
                var result = documents.FirstOrDefault();
                if (result is not null)
                {
                    return new BehaviorSubject<TDocument>(result);
                }

                return new Subject<TDocument>();
            },
            (documents, observable) =>
            {
                var result = documents.FirstOrDefault();
                if (result is not null)
                {
                    observable.OnNext(result);
                }
            });
    }

    static async Task<ISubject<TResult>> Observe<TDocument, TResult>(
        this IMongoCollection<TDocument> collection,
        Func<Task<IAsyncCursor<TDocument>>> findCall,
        FilterDefinition<TDocument> filter,
        Func<IEnumerable<TDocument>, ISubject<TResult>> createSubject,
        Action<IEnumerable<TDocument>, ISubject<TResult>> onNext)
    {
        var queryContext = Internals.ServiceProvider.GetService<IQueryContextManager>()!.Current;
        if (queryContext.Paging.IsPaged)
        {
        }

        var response = await findCall();
        var documents = response.ToList();
        var subject = createSubject(documents);

        onNext(documents, subject);
        response.Dispose();
        response = null!;

        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
        };

        var filterRendered = filter.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry);
        PrefixKeys(filterRendered);

        var fullFilter = Builders<ChangeStreamDocument<TDocument>>.Filter.And(
            filterRendered,
            Builders<ChangeStreamDocument<TDocument>>.Filter.In(
                new StringFieldDefinition<ChangeStreamDocument<TDocument>, string>("operationType"),
                ["insert", "replace", "update", "delete"]));

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<TDocument>>().Match(fullFilter);

        var idProperty = typeof(TDocument).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public)!;
#pragma warning disable CA2000 // Dispose objects before losing scope
        var cancellationTokenSource = new CancellationTokenSource();
#pragma warning restore CA2000 // Dispose objects before losing scope
        var cancellationToken = cancellationTokenSource.Token;

        IChangeStreamCursor<ChangeStreamDocument<TDocument>> cursor = null!;

        try
        {
            cursor = await collection.WatchAsync(pipeline, options);
            await cursor.ForEachAsync(
                changeDocument =>
                {
                    if (subject is Subject<TResult> disposableSubject && disposableSubject.IsDisposed &&
                        subject is BehaviorSubject<TResult> disposableBehaviorSubject && disposableBehaviorSubject.IsDisposed)
                    {
                        cursor.Dispose();
                        cancellationTokenSource.Dispose();
                        return;
                    }

                    if (changeDocument.DocumentKey.TryGetValue("_id", out var idValue))
                    {
                        var id = BsonTypeMapper.MapToDotNetValue(idValue);
                        if (idProperty.PropertyType.IsConcept())
                        {
                            id = ConceptFactory.CreateConceptInstance(idProperty.PropertyType, id);
                        }

                        var document = documents.Find(_ => idProperty.GetValue(_)!.Equals(id));
                        if (changeDocument.OperationType == ChangeStreamOperationType.Delete && document is not null)
                        {
                            documents.Remove(document);
                        }
                        else if (document is not null)
                        {
                            var index = documents.IndexOf(document);
                            documents[index] = changeDocument.FullDocument;
                        }
                        else
                        {
                            documents.Add(changeDocument.FullDocument);
                        }
                    }

                    onNext(documents, subject);
                },
                cancellationToken);
        }
        catch (ObjectDisposedException)
        {
            Console.WriteLine("Cursor disposed.");
            cursor.Dispose();
            subject.OnCompleted();

            if (subject is Subject<TResult> disposableSubject)
            {
                disposableSubject.Dispose();
            }
            if (subject is BehaviorSubject<TResult> disposableBehaviorSubject)
            {
                disposableBehaviorSubject.Dispose();
            }
        }

        subject.Subscribe(_ => { }, cursor.Dispose);

        return subject;
    }

    static void PrefixKeys(BsonDocument document)
    {
        foreach (var name in document.Names.ToArray())
        {
            var value = document[name];
            if (!name.StartsWith('$'))
            {
                var index = document.IndexOfName(name);
                document.InsertAt(index, new BsonElement($"fullDocument.{name}", value));
                document.Remove(name);
            }

            if (value is BsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is BsonDocument itemAsDocument)
                    {
                        PrefixKeys(itemAsDocument);
                    }
                }
            }
            else if (value is BsonDocument childAsDocument)
            {
                PrefixKeys(childAsDocument);
            }
        }
    }
}
