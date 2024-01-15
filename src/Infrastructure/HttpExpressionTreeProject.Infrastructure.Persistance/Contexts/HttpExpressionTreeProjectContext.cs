using HttpExpressionTreeProject.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Infrastructure.Persistance.Contexts
{
    public class HttpExpressionTreeProjectContext : DbContext
    {
        public HttpExpressionTreeProjectContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<Page> Pages { get; set; }
    }
}
