using CheckAvailability.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CheckAvailability.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private EmployeeCache _employeeCache;

    public EmployeeController(EmployeeCache employeeCache)
    {
        _employeeCache = employeeCache;
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
    {
        return Ok(await _employeeCache.GetEmployees(cancellationToken));
    }

    [HttpPost("find_available")]
    public async Task<IActionResult> FindAvailableTime([FromBody] FindAvailableTimeRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _employeeCache.FindAvailableTime(request, cancellationToken));
    }
}