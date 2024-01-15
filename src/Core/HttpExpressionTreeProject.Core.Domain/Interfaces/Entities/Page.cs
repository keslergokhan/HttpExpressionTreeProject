using HttpExpressionTreeProject.Core.Domain.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Domain.Interfaces.Entities
{
    public class Page : BaseEntity
    {
        [StringLength(200)]
        public string Title { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string? Content { get; set; }
    }
}
