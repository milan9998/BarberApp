using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hair.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public DateTime Time { get; private set; }
    public Guid Barberid { get; private set; }
    
   
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
    public string HaircutName { get; private set; }

    public Appointment(DateTime time, Guid barberid)
    {
        Id = Guid.NewGuid();
        Time = time;
        Barberid = barberid;
    }

    public Appointment SetTime(DateTime time)
    {
        Time = time;
        return this;
    }
    public Appointment SetHaircutName(string haircutName)
    {
        HaircutName = haircutName;
        return this;
    }
   /* public Appointment()
    {
        
    }*/

    /*
       var startTime = new DateTime(2024, 12, 20, 09, 0, 0);
        var endTime = new DateTime(2024, 12,20,17,0,0);
        var helpTime = startTime;
        List<DateTime> dates = new List<DateTime>();
        
        while (helpTime <= endTime)
        {
            helpTime = startTime.AddMinutes(30);
            dates.Add(helpTime);
        }
     */
}