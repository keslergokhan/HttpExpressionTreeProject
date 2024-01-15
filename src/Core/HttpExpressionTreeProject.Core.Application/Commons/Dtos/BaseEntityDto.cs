using HttpExpressionTreeProject.Core.Application.Interfaces.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application.Commons.Dtos
{
    public abstract class BaseEntityDto : IBaseBasicEntityDto
    {
        public Guid Id { get; set; }
        public byte State { get; set; }
    }
}
