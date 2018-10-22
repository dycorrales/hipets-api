using HiPets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiPets.Infra.Data.Mappings
{
    public sealed class AdopterMap : BaseMap<Adopter>
    {
        public override void Map(EntityTypeBuilder<Adopter> builder)
        {
            builder.ToTable("Adopter");
                        
            builder.Property(adopter => adopter.Name)
                .HasColumnType("varchar(250)")
                .IsRequired();

            builder.OwnsOne(adopter => adopter.ContactInfo, contact =>
            {
                contact.Property(phoneNumber => phoneNumber.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasColumnType("varchar(11)")
                    .IsRequired();

                contact.Property(email => email.Email)
                    .HasColumnName("Email")
                    .HasColumnType("varchar(150)")
                    .IsRequired();
            });

            base.Map(builder);
        }
    }
}
