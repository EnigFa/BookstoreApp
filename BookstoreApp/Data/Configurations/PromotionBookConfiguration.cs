using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookstoreApp.Data.Configurations
{
    public class PromotionBookConfiguration : IEntityTypeConfiguration<PromotionBook>
    {
        public void Configure(EntityTypeBuilder<PromotionBook> builder)
        {
            builder.HasKey(pb => new { pb.PromotionId, pb.BookId });

            builder.Property(pb => pb.OriginalPrice).HasColumnType("decimal(18,2)");

            builder.HasOne(pb => pb.Promotion)
                   .WithMany(p => p.PromotionBooks)
                   .HasForeignKey(pb => pb.PromotionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pb => pb.Book)
                   .WithMany(b => b.PromotionBooks)
                   .HasForeignKey(pb => pb.BookId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
