// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines the context for a query.
/// </summary>
/// <param name="CorrelationId">The <see cref="CorrelationId"/> for the query.</param>
/// <param name="Paging">The <see cref="Paging"/> information.</param>
/// <param name="Sorting">The <see cref="Sorting"/> information.</param>
public record QueryContext(CorrelationId CorrelationId, Paging Paging, Sorting Sorting);
