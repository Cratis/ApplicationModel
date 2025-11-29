// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.EntityFrameworkCore.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Arc.EntityFrameworkCore.for_BaseDbContext.given;

public class a_base_db_context : Specification
{
    protected TDbContext CreateDbContext<TDbContext>()
        where TDbContext : DbContext
    {
        // Create application services (not EF internal services)
        var appServices = new ServiceCollection();
        appServices.AddSingleton<IEntityTypeRegistrar, StubEntityTypeRegistrar>();
        var appServiceProvider = appServices.BuildServiceProvider();

        var options = new DbContextOptionsBuilder<TDbContext>()
            .UseSqlite(":memory:")
            .UseApplicationServiceProvider(appServiceProvider)
            .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.ManyServiceProvidersCreatedWarning))
            .Options;

        return (TDbContext)Activator.CreateInstance(typeof(TDbContext), options)!;
    }

    protected IModel GetModel<TDbContext>(TDbContext dbContext)
        where TDbContext : DbContext => dbContext.Model;

    class StubEntityTypeRegistrar : IEntityTypeRegistrar
    {
        public void RegisterEntityMaps(DbContext dbContext, ModelBuilder modelBuilder)
        {
            // Do nothing for specs - we're testing the conversion logic, not entity maps
        }
    }
}
