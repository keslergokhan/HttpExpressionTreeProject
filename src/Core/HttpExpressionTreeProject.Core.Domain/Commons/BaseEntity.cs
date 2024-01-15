using HttpExpressionTreeProject.Core.Domain.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Domain.Commons
{
    public abstract class BaseEntity : IBasicEntity
    {
        public byte State { get; set; }
    }
}
