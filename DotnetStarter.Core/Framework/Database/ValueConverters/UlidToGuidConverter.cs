using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DotnetStarter.Core.Framework.Database.ValueConverters;

public class UlidToGuidConverter : ValueConverter<Ulid, Guid>
{
	public UlidToGuidConverter () : base(
		ulid => ulid.ToGuid(),
		guid => new Ulid(guid)
	) { }
}