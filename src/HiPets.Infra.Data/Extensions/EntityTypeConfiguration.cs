using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiPets.Infra.Data.Extensions
{
    public abstract class EntityTypeConfiguration<T> where T : class
    {
        public abstract void Map(EntityTypeBuilder<T> builder);
    }
}
