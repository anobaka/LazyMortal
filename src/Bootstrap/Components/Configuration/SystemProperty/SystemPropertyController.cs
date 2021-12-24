using System.Threading.Tasks;
using Bootstrap.Components.Miscellaneous.ResponseBuilders;
using Bootstrap.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Bootstrap.Components.Configuration.SystemProperty
{
    [Route("~/system-property")]
    public abstract class SystemPropertyController : Controller
    {
        private readonly SystemPropertyService _service;

        protected SystemPropertyController(SystemPropertyService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "GetAllSystemProperties")]
        public async Task<ListResponse<SystemPropertyDto>> GetAllSystemProperties()
        {
            return new ListResponse<SystemPropertyDto>(await _service.GetAll());
        }

        [HttpGet("{key}")]
        [SwaggerOperation(OperationId = "GetSystemProperty")]
        public async Task<SingletonResponse<string>> Get(string key)
        {
            var s = new SingletonResponse<string>((await _service.GetByKey(key))?.Value);
            return s;
        }

        [HttpDelete("{key}")]
        [SwaggerOperation(OperationId = "RemoveSystemProperty")]
        public async Task<BaseResponse> Remove(string key)
        {
            await _service.RemoveByKey(key);
            return BaseResponseBuilder.Ok;
        }

        [HttpPut("{key}")]
        [SwaggerOperation(OperationId = "UpdateSystemProperty")]
        public async Task<BaseResponse> Update(string key, [FromBody] SystemPropertyUpdateRequestModel model)
        {
            return await _service.AddOrUpdate(key, model.Value);
        }

        // [HttpPut("batch")]
        // [SwaggerOperation(OperationId = "BatchUpdateSystemProperties")]
        // public async Task<BaseResponse> Update([FromBody] Dictionary<string, string> model)
        // {
        //     return await _service.BatchAddOrUpdate(model);
        // }
    }
}