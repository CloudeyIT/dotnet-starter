namespace DotnetStarter.Core.Framework.Database;

public abstract class Entity : IEntity
{
	[IsProjected(true)]
	public Guid Id { get; set; }

	[IsProjected(true)]
	public DateTime Created { get; set; }

	[IsProjected(true)]
	public DateTime Updated { get; set; }

	[IsProjected(true)]
	public Guid Revision { get; set; }
}