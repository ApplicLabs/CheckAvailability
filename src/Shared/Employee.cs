namespace CheckAvailability.Shared;

public class Employee
{
   public string Id { get; set; } 
   public string Name { get; set; } 
   public List<EmployeeOccupiedTime> OccupiedTime { get; set; }
}