using Buddget.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buddget.DAL.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<FinancialSpaceEntity> FinancialSpaces { get; set; }
        public DbSet<FinancialSpaceMemberEntity> FinancialSpaceMembers { get; set; }
        public DbSet<FinancialGoalEntity> FinancialGoals { get; set; }
        public DbSet<FinancialGoalSpaceEntity> FinancialGoalSpaces { get; set; }
        public DbSet<FinancialGoalCategoryEntity> FinancialGoalCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasMany(e => e.Categories)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One user can own many financial spaces
                entity.HasMany(e => e.OwnedSpaces)
                      .WithOne(e => e.Owner)
                      .HasForeignKey(e => e.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One user can have many financial goals
                entity.HasMany(e => e.FinancialGoals)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One user can have many transactions
                entity.HasMany(e => e.Transactions)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FinancialSpaceEntity>(entity =>
            {
                // One financial space can have many members
                entity.HasMany(e => e.Members)
                      .WithOne(e => e.FinancialSpace)
                      .HasForeignKey(e => e.FinancialSpaceId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One financial space can have many transactions
                entity.HasMany(e => e.Transactions)
                      .WithOne(e => e.FinancialSpace)
                      .HasForeignKey(e => e.FinancialSpaceId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-many relationship with FinancialGoalEntity through FinancialGoalSpaceEntity
                entity.HasMany(e => e.FinancialGoalSpaces)
                      .WithOne(e => e.FinancialSpace)
                      .HasForeignKey(e => e.FinancialSpaceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Owner)
                      .WithMany(e => e.OwnedSpaces)
                      .HasForeignKey(e => e.OwnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure FinancialGoalEntity
            modelBuilder.Entity<FinancialGoalEntity>(entity =>
            {
                entity.HasMany<FinancialGoalSpaceEntity>()
                      .WithOne(e => e.FinancialGoal)
                      .HasForeignKey(e => e.FinancialGoalId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany<FinancialGoalCategoryEntity>()
                        .WithOne(e => e.FinancialGoal)
                        .HasForeignKey(e => e.FinancialGoalId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<TransactionEntity>(entity =>
            {
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(e => e.UserId);
                entity.HasOne(e => e.FinancialSpace)
                    .WithMany(f => f.Transactions)
                    .HasForeignKey(e => e.FinancialSpaceId);
            });
        }
    }
}