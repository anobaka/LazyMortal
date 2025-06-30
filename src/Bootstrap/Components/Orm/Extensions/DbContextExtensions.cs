using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Bootstrap.Components.Orm.Extensions;

public static class DbContextExtensions
{
    public static void Detach<TResource>(this DbContext ctx, TResource resource)
    {
        ctx.Entry(resource!).State = EntityState.Detached;
    }

    public static void DetachAll<TResource>(this DbContext ctx, IEnumerable<TResource> resources)
    {
        foreach (var r in resources)
        {
            ctx.Detach(r);
        }
    }
}