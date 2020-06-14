using Microsoft.EntityFrameworkCore;
using Nibo.Models;

namespace Nibo.Data {
    public class ApplicationDbContext : DbContext {
        public virtual DbSet<Transaction> Company { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public ApplicationDbContext() : base() { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            configTransactionEntity(builder);
        }

        #region Config Entity Methods




        public void configTransactionEntity(ModelBuilder builder) {
            builder.Entity<Transaction>(
                (entity) => {
                    entity.HasKey(e => e.Hash);

                    entity.Property(e => e.Type)
                                 .IsRequired()
                                 .HasMaxLength(5);

                    entity.Property(e => e.Memo)
                                 .IsRequired()
                                 .HasMaxLength(100);

                    entity.Property(e => e.Value)
                                 .IsRequired()
                                 .HasMaxLength(200);

                    entity.Property(e => e.Data)
                                 .IsRequired()
                                 .HasMaxLength(200);

                    entity.Property(e => e.Data)
                                 .IsRequired()
                                 .HasMaxLength(200);

                    entity.Property(e => e.Data)
                                 .IsRequired()
                                 .HasMaxLength(200);
                });
        }

        #endregion

    }
}


