using DotnetStarter.Core.Framework.Database.ValueGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotnetStarter.Core.Framework.Database;

public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class, IEntity
{
    public virtual void Configure (EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidValueGenerator>();
    }
}