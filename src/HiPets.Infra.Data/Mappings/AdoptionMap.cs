using HiPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiPets.Infra.Data.Mappings
{
    public sealed class AdoptionMap : BaseMap<Adoption>
    {
        public override void Map(EntityTypeBuilder<Adoption> builder)
        {
            builder.ToTable("Adoption");
                  
            builder.Property(adoption => adoption.AdoptionStatus)
                .HasColumnType("int")
                .IsRequired();

            builder.HasOne(adoption => adoption.Adopter)
                .WithMany()
                .HasForeignKey(adoption => adoption.AdopterId)
                .IsRequired();

            builder.HasOne(adoption => adoption.Animal)
                .WithMany()
                .HasForeignKey(adoption => adoption.AnimalId)
                .IsRequired();

            builder.Property(adoption => adoption.Observation)
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            base.Map(builder);
        }
    }
}
