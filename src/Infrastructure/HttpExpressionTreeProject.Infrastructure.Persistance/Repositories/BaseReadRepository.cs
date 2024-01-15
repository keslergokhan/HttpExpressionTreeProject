using HttpExpressionTreeProject.Core.Application.Interfaces.Repositories;
using HttpExpressionTreeProject.Core.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Infrastructure.Persistance.Repositories
{
    public abstract class BaseReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly DbContext _dbContext;

        protected BaseReadRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<T> GetFilter(Expression<Func<T, bool>> filter)
        {
            return this.DbSet().AsNoTracking().Where(filter).AsQueryable().ToList();
        }

        public DbSet<T> DbSet()
        {
            return this._dbContext.Set<T>();
        }
    }
}
