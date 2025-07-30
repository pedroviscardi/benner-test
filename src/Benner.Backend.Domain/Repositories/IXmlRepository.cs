using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Benner.Backend.Domain.Common;
using Benner.Backend.Shared.Common;

namespace Benner.Backend.Domain.Repositories
{
    public interface IXmlRepository<T> where T : BaseEntity
    {
        Task<Result<T>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<T>>> GetAllAsync();
        Task<Result<T>> AddAsync(T entity);
        Task<Result<T>> UpdateAsync(T entity);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<bool>> ExistsAsync(Guid id);
        Task<Result<int>> CountAsync();
        Task<Result<IEnumerable<T>>> FindAsync(Func<T, bool> predicate);
    }
}