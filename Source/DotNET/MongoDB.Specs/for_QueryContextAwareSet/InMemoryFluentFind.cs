// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization;
using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet;

public class InMemoryFluentFind<TDocument>(IEnumerable<TDocument> collection, int? limit = null) : FindFluentBase<TDocument, TDocument>
{
    public override FilterDefinition<TDocument> Filter { get; set; }
    public override FindOptions<TDocument, TDocument> Options { get; } = new FindOptions<TDocument>();

    public override IFindFluent<TDocument, TResult> As<TResult>(IBsonSerializer<TResult> resultSerializer) => throw new NotImplementedException();
    public override Task<long> CountAsync(CancellationToken cancellationToken = new CancellationToken()) => Task.FromResult<long>(collection.Count());
    public override Task<long> CountDocumentsAsync(CancellationToken cancellationToken = new CancellationToken()) => CountAsync(cancellationToken);
    public override IFindFluent<TDocument, TDocument> Limit(int? limit) => new InMemoryFluentFind<TDocument>(collection, limit);
    public override IFindFluent<TDocument, TNewProjection> Project<TNewProjection>(ProjectionDefinition<TDocument, TNewProjection> projection) => throw new NotImplementedException();
    public override IFindFluent<TDocument, TDocument> Skip(int? skip) => new InMemoryFluentFind<TDocument>(collection.Skip(skip ?? 0));
    public override IFindFluent<TDocument, TDocument> Sort(SortDefinition<TDocument> sort) => throw new NotImplementedException();

    public override Task<IAsyncCursor<TDocument>> ToCursorAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IAsyncCursor<TDocument>>(new Cursor(collection.GetEnumerator(), limit));

    public class Cursor(IEnumerator<TDocument> enumerator, int? limit) : IAsyncCursor<TDocument>
    {
        public void Dispose() { }
        public bool MoveNext(CancellationToken cancellationToken = new CancellationToken()) => enumerator.MoveNext();
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = new CancellationToken()) => Task.FromResult(enumerator.MoveNext());
        public IEnumerable<TDocument> Current
        {
            get
            {
                var result = new List<TDocument>();
                do
                {
                    if (enumerator.Current is null)
                    {
                        break;
                    }
                    result.Add(enumerator.Current);
                    if (limit.HasValue && result.Count == limit)
                    {
                        return result;
                    }
                } while (enumerator.MoveNext());
                return result;
            }
        }
    }
}
