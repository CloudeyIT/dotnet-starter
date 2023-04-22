using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DotnetStarter.Core.Framework.Database.ValueGenerators;

public class UlidValueGenerator : ValueGenerator<Ulid>
{
	public override bool GeneratesTemporaryValues => false;

	public override Ulid Next (EntityEntry entry)
	{
		return Ulid.NewUlid();
	}
}