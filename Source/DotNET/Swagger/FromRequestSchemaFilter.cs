// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cratis.Applications.Swagger;

/// <summary>
/// Represents an implementation of <see cref="ISchemaFilter"/> that removes properties that are decorated with <see cref="FromRouteAttribute"/> or <see cref="FromQueryAttribute"/>.
/// </summary>
public class FromRequestSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var parameters = context.Type.GetConstructors().SelectMany(_ => _.GetParameters()).ToArray();

        bool HasConstructorParameterWithRequestArgument(PropertyInfo propertyInfo) =>
            parameters.Any(_ => _.Name == propertyInfo.Name && (_.HasAttribute<FromRouteAttribute>() || _.HasAttribute<FromQueryAttribute>()));

        var properties = context.Type.GetProperties()
            .Where(_ =>
                PropertyExtensions.HasAttribute<FromRouteAttribute>(_) ||
                PropertyExtensions.HasAttribute<FromQueryAttribute>(_) ||
                HasConstructorParameterWithRequestArgument(_))
            .ToList();

        if (properties.Count > 0)
        {
            foreach (var property in properties)
            {
                var propertyToRemove = schema.Properties.SingleOrDefault(_ => _.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                if (propertyToRemove.Value is not null)
                {
                    schema.Properties.Remove(propertyToRemove.Key);
                }
            }
        }

        properties.ForEach(_ => schema.Properties.Remove(_.Name));
    }
}