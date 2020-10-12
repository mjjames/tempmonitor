using Microsoft.AspNetCore.Mvc;
using System;

namespace TempMonitorUI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TimeController : ControllerBase
	{
		[HttpGet]
		public DateTime Get()
		{
			return DateTime.UtcNow;
		}
	}
}
