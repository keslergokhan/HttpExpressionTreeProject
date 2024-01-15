using HttpExpressionTreeProject.Core.Application;
using HttpExpressionTreeProject.Core.Application.Interfaces.Repositories;
using HttpExpressionTreeProject.Core.Application.Interfaces.Services;
using HttpExpressionTreeProject.Core.Application.Services;
using HttpExpressionTreeProject.Infrastructure.Persistance.Contexts;
using HttpExpressionTreeProject.Infrastructure.Persistance.Repositories.Reads;
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
            service.AddScoped<DbContext,HttpExpressionTreeProjectContext>();
            service.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            
        }
    }
}
