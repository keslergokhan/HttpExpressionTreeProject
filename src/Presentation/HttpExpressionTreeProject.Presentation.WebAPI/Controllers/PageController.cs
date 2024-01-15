using HttpExpressionTreeProject.Core.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using HttpExpressionTreeProject.Core.Application.Services;

namespace HttpExpressionTreeProject.Presentation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageServicei;
        public PageController(IPageService pageServicei)
        {
            _pageServicei = pageServicei;
        }

        [HttpGet]
        [Route("GetPageFilter")]
        public async Task<IActionResult> GetPageFilter()
        {
            
            return Ok(this._pageServicei.GetFilterPages(HttpContext.HttExperssion()));
        }
    }
}
