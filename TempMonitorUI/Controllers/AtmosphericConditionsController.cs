using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TempMonitorUI.Data;
using TempMonitorUI.Models;

namespace TempMonitorUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AtmosphericConditionsController : ControllerBase
	{
		private readonly TempMonitorService _service;

		public AtmosphericConditionsController(TempMonitorService service) => _service = service;

		[HttpPost]
		public async Task<ActionResult> Create(AtmosphericCondition condition)
		{
			await _service.Add(condition);
			return StatusCode(StatusCodes.Status201Created);
		}
	}
}
