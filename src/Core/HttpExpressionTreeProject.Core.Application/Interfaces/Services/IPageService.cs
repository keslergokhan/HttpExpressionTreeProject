using HttpExpressionTreeProject.Core.Application.Dtos.Pages;
using HttpExpressionTreeProject.Core.Application.Services;
using HttpExpressionTreeProject.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application.Interfaces.Services
{
    public interface IPageService
    {
        public List<ReadPageDto> GetFilterPages(HttpExpression httpExpression);
    }
}
