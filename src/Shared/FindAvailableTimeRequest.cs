namespace CheckAvailability.Shared;

public class FindAvailableTimeRequest
{
   public List<string> EmployeeIds { get; set; }
   public string Length { get; set; }
   public string TimeOfDay { get; set; }
   public DateTime? Date { get; set; }
}