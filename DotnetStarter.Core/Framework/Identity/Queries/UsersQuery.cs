﻿using Cloudey.Reflex.Authorization.HotChocolate;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;

namespace DotnetStarter.Core.Framework.Identity.Queries;

[QueryType]
public class UsersQuery
{
	[Guard(new[] { Role.Admin })]
	[UseOffsetPaging]
	[UseProjection]
	[UseFiltering]
	[UseSorting]
	public IQueryable<User> Users (MainDb db)
	{
		return db.Set<User>().AsNoTracking().AsQueryable();
	}
}