﻿using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[QueryType]
public class RolesQuery
{
	[Guard(new[] { Role.Admin })]
	[UseOffsetPaging]
	[UseProjection]
	[UseFiltering]
	[UseSorting]
	public IQueryable<Role> Roles (MainDb db)
	{
		return db.Set<Role>().AsNoTracking().AsQueryable();
	}
}