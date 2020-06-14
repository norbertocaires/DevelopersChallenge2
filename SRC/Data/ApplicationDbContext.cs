using Microsoft.EntityFrameworkCore;
using Nibo.Models;

namespace Nibo.Data {
    public class ApplicationDbContext : DbContext {
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Import> Import { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public ApplicationDbContext() : base() { }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            configTransactionEntity(builder);

            configImportEntity(builder);
        }

        #region Config Entity Methods
        public void configTransactionEntity(ModelBuilder builder) {
            builder.Entity<Transaction>(
                (entity) => {
                    entity.HasKey(e => e.Hash);

                    entity.Property(e => e.Type)
                                 .IsRequired()
                                 .HasMaxLength(6);

                    entity.Property(e => e.Memo)
                                 .IsRequired();

                    entity.Property(e => e.Value)
                                 .IsRequired();

                    entity.Property(e => e.Date)
                                 .IsRequired();

                });
        }

        public void configImportEntity(ModelBuilder builder) {
            builder.Entity<Import>(
                (entity) => {
                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Date)
                                 .IsRequired()
                                 .HasMaxLength(6);

                    entity.Property(e => e.FileDuplicate)
                                 .IsRequired();

                    entity.Property(e => e.FileImported)
                                 .IsRequired();

                    entity.Property(e => e.TotalTransactions)
                                 .IsRequired();

                    entity.Property(e => e.TotalTransactionsDuplicates)
                                .IsRequired();

                    entity.Property(e => e.TotalTransactionsSaves)
                                .IsRequired();

                });
        }

        #endregion

    }
}


