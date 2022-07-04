using CheckAvailability.Shared;

namespace CheckAvailability.Server;

public class EmployeeCache
{
    private readonly IHttpClientFactory _httpClientFactory;
    private Lazy<Func<CancellationToken, Task<List<Employee>>>> _employees;

    public EmployeeCache(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _employees = new Lazy<Func<CancellationToken, Task<List<Employee>>>>(LoadEmployees);
    }

    private async Task<List<Employee>> LoadEmployees(CancellationToken cancellationToken)
    {
        var employees = new List<Employee>();
        var httpClient = _httpClientFactory.CreateClient();
        var employeesData = await httpClient.GetStringAsync("https://gryphondevstorage.blob.core.windows.net/availability/availability-1.1.csv", cancellationToken);
        var employeesList = employeesData.Split(Environment.NewLine);
        foreach (var item in employeesList)
        {
            var fields = item.Split(",");
            if (fields.Length < 2) continue;
            var employee = employees.FirstOrDefault(e => e.Id == fields[0]);
            if (employee == null)
            {
                employee = new Employee
                {
                    Id = fields[0],
                    OccupiedTime = new()
                };
                employees.Add(employee);
            }
            if (fields.Length > 2 && DateTime.TryParse(fields[1], out var start) && DateTime.TryParse(fields[2], out var end))
            {
                var occupiedTime = new EmployeeOccupiedTime
                {
                    StartDate = start.AddMonths(6),
                    EndDate = end.AddMonths(6),
                };
                employee.OccupiedTime.Add(occupiedTime);
            }
            else if (fields.Length > 1)
            {
                employee.Name = fields[1];
            }
        }

        employees.RemoveAll(e => e.Name == null);

        return employees;
    }

    public async Task<IEnumerable<EmployeeResponse>> GetEmployees(CancellationToken cancellationToken)
    {
        var employees = await _employees.Value(cancellationToken);
        return employees.Select(e =>
            new EmployeeResponse
            {
                Id = e.Id,
                Name = e.Name
            });
    }

    public async Task<AvailableTimeResponse> FindAvailableTime(FindAvailableTimeRequest request, CancellationToken cancellationToken)
    {
        var start = request.Date ?? DateTime.Now;
        start = new DateTime(start.Year, start.Month, start.Day, DateTime.Now.Hour + 1, 0, 0).AddMinutes(-30);
        start = GetNextStartTime(start, request.TimeOfDay);
        var end = start.AddMinutes(int.Parse(request.Length));
        var employees = await _employees.Value(cancellationToken);
        var queryEmployees = employees.Where(e => request.EmployeeIds.Contains(e.Id)).ToArray();
        while (queryEmployees.Any(e => Overlaps(e, start, end)))
        {
            start = GetNextStartTime(start, request.TimeOfDay);
            end = start.AddMinutes(int.Parse(request.Length));
        }

        return new AvailableTimeResponse{ MeetingTime = start };
    }

    private DateTime GetNextStartTime(DateTime start, string timeOfDay)
    {
        var next = start.AddMinutes(30);
        switch (timeOfDay)
        {
            case "1":
                if (next.Hour < 9)
                {
                    next = new DateTime(start.Year, start.Month, start.Day, 9, 0, 0);
                }
                else if (next.Hour > 17)
                {
                    next = new DateTime(start.Year, start.Month, start.Day + 1, 9, 0, 0);
                }
                break;
            case "2":
                if (next.Hour > 9 && next.Hour < 17)
                {
                    next = new DateTime(start.Year, start.Month, start.Day, 17, 0, 0);
                }
                break;
        }

        return next;
    }

    private bool Overlaps(
        Employee employee,
        DateTime start,
        DateTime end
    )
    {
        return employee.OccupiedTime.Any(o => o.StartDate < end && start < o.EndDate);
    }
}