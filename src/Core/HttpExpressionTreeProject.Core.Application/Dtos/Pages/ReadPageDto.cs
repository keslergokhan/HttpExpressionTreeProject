using HttpExpressionTreeProject.Core.Application.Commons.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpExpressionTreeProject.Core.Application.Dtos.Pages
{
    public class ReadPageDto : BaseEntityDto
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string? Content { get; set; }
    }
}
