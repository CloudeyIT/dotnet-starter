using EntityFrameworkCore.Triggers;

namespace DotnetStarter.Core.Framework.Database;

public interface IEntity
{
	static IEntity ()
	{
		Triggers<IEntity>.GlobalUpdating.Add(entry => { entry.Entity.Updated = DateTime.UtcNow; });

		Triggers<IEntity>.GlobalUpdating.Add(entry => { entry.Entity.Revision = Ulid.NewUlid(); });

		Triggers<IEntity>.GlobalInserting.Add(
			entry =>
			{
				var time = DateTime.UtcNow;
				entry.Entity.Created = time;
				entry.Entity.Updated = time;
			}
		);

		Triggers<IEntity>.GlobalInserting.Add(entry => { entry.Entity.Revision = Ulid.NewUlid(); });
	}

	public Ulid Id { get; set; }
	public Guid Guid
	{
		get => Id.ToGuid();
		set => Id = new Ulid(value);
	}
	public DateTime Created { get; set; }
	public DateTime Updated { get; set; }
	public Ulid Revision { get; set; }
}