using EntityFrameworkCore.Triggers;

namespace DotnetStarter.Core.Framework.Database;

public interface IEntity
{
	static IEntity ()
	{
		Triggers<IEntity>.GlobalUpdating.Add(entry => { entry.Entity.Updated = DateTime.UtcNow; });

		Triggers<IEntity>.GlobalUpdating.Add(entry => { entry.Entity.Revision = Ulid.NewUlid().ToGuid(); });

		Triggers<IEntity>.GlobalInserting.Add(
			entry =>
			{
				var time = DateTime.UtcNow;
				entry.Entity.Created = time;
				entry.Entity.Updated = time;
			}
		);

		Triggers<IEntity>.GlobalInserting.Add(entry => { entry.Entity.Revision = Ulid.NewUlid().ToGuid(); });
	}

	public Guid Id { get; set; }
	public DateTime Created { get; set; }
	public DateTime Updated { get; set; }
	public Guid Revision { get; set; }
}