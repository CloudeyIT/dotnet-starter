using Cloudey.Reflex.Database;
using DotnetStarter.Core.Framework.Identity.Entities;

namespace DotnetStarter.Core.Framework.Database;

public class MainDb : ReflexDatabaseContext<User, Role, MainDb>
{
	public MainDb (DbContextOptions<MainDb> options) : base(options) { }
}