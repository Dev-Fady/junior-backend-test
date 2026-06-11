using junior_backend_test.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unior_backend_test.Domain.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllIgnoreFilters();
        Task<TEntity> GetByIDAsync(Guid id);
        Task<TEntity> GetByIDIgnoreFiltersAsync(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Archived(TEntity entity);
        void UnArchived(TEntity entity);

        Task<bool> SaveChangesAsync();
    }
}
