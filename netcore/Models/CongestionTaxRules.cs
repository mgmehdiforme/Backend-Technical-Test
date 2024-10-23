using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class CongestionTaxRules
{
    public List<TimeSlot> TimeSlots { get; set; }
    public List<string> TollFreeVehicles { get; set; }
    public List<DateTime> PublicHolidays { get; set; }
    public int MaxDailyFee { get; set; }
}

public class TimeSlot
{
    
    public TimeSpan Start { get; set; }
    
    public TimeSpan End { get; set; }
    public int Fee { get; set; }
}