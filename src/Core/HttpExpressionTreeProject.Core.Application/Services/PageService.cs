using HttpExpressionTreeProject.Core.Application.Dtos.Pages;
using HttpExpressionTreeProject.Core.Application.Interfaces.Repositories;
using HttpExpressionTreeProject.Core.Application.Interfaces.Services;
using HttpExpressionTreeProject.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application.Services
{
    public class PageService : IPageService
    {
        private readonly IReadRepository<Page> _readPageRep;

        public PageService(IReadRepository<Page> readPageRep)
        {
            _readPageRep = readPageRep;
        }

        public List<ReadPageDto> GetFilterPages(HttpExpression httpExpression)
        {
            //Projeyi daha fazla uzatmamak için select kullandım fakat siz mapper kullanın
            return this._readPageRep.GetFilter(httpExpression.GetFilterExpression<Page>()).Select(x=> new ReadPageDto
            {
                Id = x.Id,
                Content = x.Content,
                Description = x.Description,
                State = x.State,
                Title = x.Title,
                Url = x.Url,
                CreateDate = x.CreateDate,  
                
            }).ToList();
        }
    }
}
