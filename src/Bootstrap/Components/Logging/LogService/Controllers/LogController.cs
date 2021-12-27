using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Logging.LogService.Models.Entities;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bootstrap.Components.Logging.LogService.Controllers
{
    [Route("~/log")]
    public abstract class LogController : Controller
    {
        private readonly Services.LogService _service;

        protected LogController(Services.LogService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "GetAllLogs")]
        public async Task<ListResponse<Log>> GetAll()
        {
            return new((await _service.GetAll()).OrderByDescending(a => a.DateTime));
        }

        [HttpGet("unread/count")]
        [SwaggerOperation(OperationId = "GetUnreadLogCount")]
        public async Task<SingletonResponse<int>> GetUnreadCount()
        {
            return new(data: await _service.Count(a => !a.Read));
        }

        [HttpPatch("{id}/read")]
        [SwaggerOperation(OperationId = "ReadLog")]
        public async Task<BaseResponse> Read(int id)
        {
            return await _service.Read(id);
        }

        [HttpPatch("read")]
        [SwaggerOperation(OperationId = "ReadAllLog")]
        public async Task<BaseResponse> ReadAll()
        {
            return await _service.ReadAll();
        }

        [HttpDelete]
        [SwaggerOperation(OperationId = "ClearAllLog")]
        public async Task<BaseResponse> ClearAll()
        {
            await _service.Truncate();
            return BaseResponseBuilder.Ok;
        }
    }
}