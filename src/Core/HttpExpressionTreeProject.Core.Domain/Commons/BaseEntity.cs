using HttpExpressionTreeProject.Core.Domain.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Domain.Commons
{
    public abstract class BaseEntity : IBasicEntity
    {
        [Key]
        public Guid Id { get; set; }
        public byte State { get; set; }
        
    }
}
