using HttpExpressionTreeProject.Core.Application.Interfaces.Repositories;
using HttpExpressionTreeProject.Core.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Infrastructure.Persistance.Repositories.Reads
{
    public class ReadRepository<T> : BaseReadRepository<T>, IReadRepository<T> where T : BaseEntity
    {
        public ReadRepository(DbContext dbContext):base(dbContext)
        {
            
        }

    }
}
