namespace CheckAvailability.Shared;

public class EmployeeOccupiedTime
{
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public DayOfWeek StartDay => StartDate.DayOfWeek;
    public DayOfWeek EndDay => EndDate.DayOfWeek;
}