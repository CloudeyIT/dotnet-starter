using DotnetStarter.Core.Framework.Database.ValueConverters;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Framework.Setup;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DotnetStarter.Core.Framework.Database;

public class MainDb : IdentityDbContext<User, Role, Ulid>
{
	public MainDb (DbContextOptions<MainDb> options) : base(options) { }

	protected override void ConfigureConventions (ModelConfigurationBuilder configurationBuilder)
	{
		base.ConfigureConventions(configurationBuilder);

		configurationBuilder.Properties<Ulid>()
			.HaveConversion<UlidToGuidConverter>();
	}

	/// <inheritdoc />
	protected override void OnModelCreating (ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		AppDomain.CurrentDomain.GetIncludedAssemblies()
			.ForEach(assembly => builder.ApplyConfigurationsFromAssembly(assembly));
	}

	public override int SaveChanges ()
	{
		return this.SaveChangesWithTriggers(base.SaveChanges);
	}

	public override int SaveChanges (bool acceptAllChangesOnSuccess)
	{
		return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
	}

	public override Task<int> SaveChangesAsync (CancellationToken cancellationToken = default)
	{
		return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, true, cancellationToken);
	}

	public override Task<int> SaveChangesAsync (
		bool acceptAllChangesOnSuccess,
		CancellationToken cancellationToken = default
	)
	{
		return this.SaveChangesWithTriggersAsync(
			base.SaveChangesAsync,
			acceptAllChangesOnSuccess,
			cancellationToken
		);
	}
}