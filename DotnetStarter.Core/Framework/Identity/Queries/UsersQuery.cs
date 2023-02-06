using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.GraphQl.Types;
using DotnetStarter.Core.Framework.Identity.Attributes;
using DotnetStarter.Core.Framework.Identity.Entities;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[ExtendObjectType(typeof(Query))]
public class UsersQuery
{
    [Guard(Roles = new[] { Role.Admin })]
    [UseOffsetPaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> Users (MainDb db)
    {
        return db.Set<User>().AsNoTracking().AsQueryable();
    }
}