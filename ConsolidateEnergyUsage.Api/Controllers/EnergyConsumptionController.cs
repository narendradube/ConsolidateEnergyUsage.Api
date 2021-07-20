using ConsolidateEnergyUsage.Api.Domain;
using ConsolidateEnergyUsage.Api.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace ConsolidateEnergyUsage.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/consolidated/consumption")]
    public class EnergyConsumptionController : ControllerBase
    {
        private readonly IFileProcessor _fileProcessor;
        public EnergyConsumptionController(IFileProcessor fileProcessor, ILogger<EnergyConsumptionController> logger)
        {
            _fileProcessor = fileProcessor;
        }
        /// <summary>
        /// Returns engery consumption usages using input .csv file, use version: 1.0
        /// </summary>

        /// <response code="200">energy consumption dataset</response>
        [TranslateResultToActionResult]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet()]
        [SwaggerOperation(
            OperationId = "Calculate.GetEnergyConsumptionAsync",
            Tags = new[] { "ConsolidateEnergyUsage.Api" }
            )]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _fileProcessor.Process());
        }
        [TranslateResultToActionResult]
        [ApiVersion("1.0")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("totalByMeter")]
        [SwaggerOperation(
            OperationId = "Calculate.totalByMeter",
            Tags = new[] { "ConsolidateEnergyUsage.Api" }
            )]
        public async Task<IActionResult> GetTotalUsage()
        {
            return Ok(await _fileProcessor.TotalUsage());
        }
    }
}
