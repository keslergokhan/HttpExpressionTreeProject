using HttpExpressionTreeProject.Core.Application.Interfaces.Services;
using HttpExpressionTreeProject.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplication(this IServiceCollection service)
        {
            service.AddScoped<IPageService, PageService>();
        }
    }
}
