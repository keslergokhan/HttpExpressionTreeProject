using HttpExpressionTreeProject.Core.Domain.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application.Interfaces.Repositories
{
    public interface IReadRepository<T> where T : BaseEntity
    {
        public IEnumerable<T> GetFilter(Expression<Func<T, bool>> filter);
    }
}
