using DotnetStarter.Core.Framework.Database;
using DotnetStarter.Core.Framework.Identity.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetStarter.Core.Framework.Identity.Entities;

[Guard(new[] { Admin })]
public class Role : IdentityRole<Guid>, IEntity
{
	public const string User = "User";
	public const string Admin = "Admin";

	public DateTime Created { get; set; }
	public DateTime Updated { get; set; }
	public Guid Revision { get; set; }

	public class Configuration : IEntityTypeConfiguration<Role>
	{
		public void Configure (EntityTypeBuilder<Role> builder)
		{
			builder.HasData(
				new Role
				{
					Id = Guid.Parse("e4cfb454-89bb-47bc-854a-515ad7fa67f7"),
					Revision = Guid.Parse("7be7db8d-770d-4050-ba1c-c19f82a21714"),
					ConcurrencyStamp = "e0295e1b-83e8-428d-a4c9-9a0c740a60d5",
					Name = "User",
					NormalizedName = "USER",
					Created = DateTime.UnixEpoch,
					Updated = DateTime.UnixEpoch,
				},
				new Role
				{
					Id = Guid.Parse("8523a77d-c182-428b-ab78-edcf96a84b28"),
					Revision = Guid.Parse("867de9c0-fcad-42de-883b-7f2e9a988338"),
					ConcurrencyStamp = "ec74d38b-4fd9-4e2f-8b7c-6c974e24df5a",
					Name = "Admin",
					NormalizedName = "ADMIN",
					Created = DateTime.UnixEpoch,
					Updated = DateTime.UnixEpoch,
				}
			);
		}
	}
}