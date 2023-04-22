namespace DotnetStarter.Core.Framework.Database;

public abstract class Entity : IEntity
{
	[IsProjected(true)]
	public Ulid Id { get; set; }

	[IsProjected(true)]
	public Guid Guid
	{
		get => Id.ToGuid();
		set => Id = new Ulid(value);
	}

	[IsProjected(true)]
	public DateTime Created { get; set; } = DateTime.MinValue;

	[IsProjected(true)]
	public DateTime Updated { get; set; } = DateTime.MinValue;

	[IsProjected(true)]
	public Ulid Revision { get; set; }
}