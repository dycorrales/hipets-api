using HiPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiPets.Infra.Data.Mappings
{
    public sealed class AnimalMap : BaseMap<Animal>
    {
        public override void Map(EntityTypeBuilder<Animal> builder)
        {
            builder.ToTable("Animal");
                        
            builder.Property(animal => animal.Name)
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(animal => animal.Breed)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(animal => animal.Age)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(animal => animal.PrevalentColor)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(animal => animal.Behavior)
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.Property(animal => animal.AnimalStatus)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(animal => animal.AnimalType)
                .HasColumnType("int")
                .IsRequired();

            builder.HasOne(animal => animal.Adopter)
                .WithMany(adopter => adopter.Animals)
                .HasForeignKey(animal => animal.AdopterId)
                .IsRequired(false);

            builder.Property(animal => animal.PictureUrl)
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            base.Map(builder);
        }
    }
}
