using Cloudey.Reflex.Authorization.HotChocolate;
using Cloudey.Reflex.Database;
using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Entities;
using DotnetStarter.Core.Modules.HelloWorld.Policies;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetStarter.Core.Modules.HelloWorld.Entities;

[Guard<HelloPolicy>]
public class Hello : Entity
{
	public string? Name { get; set; }

	[Guard<HelloPolicy>]
	public string? Message { get; set; }

	public Ulid? UserId { get; set; }
	public User? User { get; set; }

	public class Configuration : EntityConfiguration<Hello>
	{
		public override void Configure (EntityTypeBuilder<Hello> builder)
		{
			base.Configure(builder);

			builder.HasIndex(x => x.Name).IsUnique();

			builder.HasData(
				new Hello
				{
					Id = new Ulid(new Guid("68bbcab1-4aec-4117-8ed9-1101f4768825")),
					Revision = new Ulid(new Guid("35f9f62b-1d5e-4a37-87b9-b3372771425e")),
					Name = "Someone",
					Message = "Hello Someone!",
					UserId = null,
				},
				new Hello
				{
					Id = new Ulid(new Guid("7ffdf241-7645-42e3-a55c-45f924401534")),
					Revision = new Ulid(new Guid("c5800c39-9046-4b5a-a091-67c28f8b8ade")),
					Name = "Kristo",
					Message = "Hello Kristo!",
					UserId = new Ulid(new Guid("7bc36eff-a304-4a0d-970e-1b32606e1bb3")),
				}
			);
		}
	}
}