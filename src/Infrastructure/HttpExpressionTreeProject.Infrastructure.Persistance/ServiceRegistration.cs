using HttpExpressionTreeProject.Infrastructure.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Infrastructure.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddPersistance(this IServiceCollection service,Action<DbContextOptionsBuilder> op)
        {
            service.AddDbContext<HttpExpressionTreeProjectContext>(op);
        }
    }
}
