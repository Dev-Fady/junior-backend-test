using junior_backend_test.Domain.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using unior_backend_test.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace junior_backend_test.Infrastructure.Database
{
    public class JuniorBackendTestContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly ICurrentUserService _currentUser;

        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<Product> Products { get; set; }

        public JuniorBackendTestContext(DbContextOptions<JuniorBackendTestContext> options, ICurrentUserService currentUser) : base(options)
        {
            _currentUser = currentUser;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Product>().HasIndex(p => p.Price).HasDatabaseName("idx_products_price");

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType).HasQueryFilter(
                        ConvertFilterExpression<BaseEntity>(e => !e.IsArchived, entityType.ClrType));
                }
            }
        }

        private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression<TInterface>(
            System.Linq.Expressions.Expression<Func<TInterface, bool>> filterExpression, Type entityType)
        {
            var newParam = System.Linq.Expressions.Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
            return System.Linq.Expressions.Expression.Lambda(newBody, newParam);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entiries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entiries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedById = _currentUser.UserId;
                        entry.Entity.CreatedByName = _currentUser.UserName;
                        entry.Entity.CreatedDateTime = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedById = _currentUser.UserId;
                        entry.Entity.ModifiedByName = _currentUser.UserName;
                        entry.Entity.ModifiedDateTime = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsArchived = true;
                        entry.Entity.ArchivedById = _currentUser.UserId;
                        entry.Entity.ArchivedByName = _currentUser.UserName;
                        entry.Entity.ArchivedDateTime = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
