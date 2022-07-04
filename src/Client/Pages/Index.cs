using CheckAvailability.Client.Models;
using CheckAvailability.Client.Services;
using CheckAvailability.Shared;
using Microsoft.AspNetCore.Components;

namespace CheckAvailability.Client.Pages;

public partial class Index : PageBase
{
    private List<EmployeeModel> _employees;
    private readonly DropDownValueModel[] _meetingLengths =  
    {
        new()
        {
            Description = "30 Minutes",
            Value = "30"
        },
        new()
        {
            Description = "An hour",
            Value = "60"
        }
        ,
        new()
        {
            Description = "An hour and a half",
            Value = "90"
        }
    };
    private string _meetingLength = "30";

    private readonly DropDownValueModel[] _meetingTimeOfDays =
    {
        new()
        {
            Description = "Office hours (9AM – 5PM)",
            Value = "1"
        },
        new()
        {
            Description = "Off-hours",
            Value = "2"
        },
        new()
        {
            Description = "Anytime",
            Value = "3"
        },
    };
    private string _meetingTimeOfDay = "1";

    private IEnumerable<string> _selectedEmployees = new List<string>();
    private bool _isBusy;
    private DateTime? _meetingDate;
    private DateTime? _meetingDateFrom;
    private DateTime? _meetingDateTo;
    private DateTime? _availableMeetingTime;

    [Inject] private IEmployeeService _employeeService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _employees = await _employeeService.GetEmployees(Cts.Token);
    }

    private async Task OnFindAvailableTimeClick()
    {
        _isBusy = true;
        try
        {
            var request = new FindAvailableTimeRequest
            {
                EmployeeIds = _selectedEmployees.ToList(),
                Length = _meetingLength,
                TimeOfDay = _meetingTimeOfDay,
                Date = _meetingDate
            };
            var response = await _employeeService.FindAvailableTime(request, Cts.Token);
            _availableMeetingTime = response?.MeetingTime;
        }
        finally
        {
            _isBusy = false;
        }
    }
}