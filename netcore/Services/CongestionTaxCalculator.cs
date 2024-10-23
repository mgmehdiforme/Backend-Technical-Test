using System;
using System.Collections.Generic;
using congestion.calculator.Vehicles;
public class CongestionTaxCalculator
{
    private readonly CongestionTaxRules _rules;

    public CongestionTaxCalculator(CongestionTaxRules rules)
    {
        _rules = rules;
    }

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        if (IsTollFreeVehicle(vehicle)) return 0;

        DateTime intervalStart = dates[0];
        int totalFee = 0;

        foreach (DateTime date in dates)
        {
            if (IsTollFreeDate(date)) continue;

            int nextFee = GetTollFee(date);
            TimeSpan diff = date - intervalStart;
            double minutes = diff.TotalMinutes;

            if (minutes <= 60)
            {
                totalFee = Math.Max(totalFee, nextFee);
            }
            else
            {
                intervalStart = date;
                totalFee += nextFee;
            }

            if (totalFee > _rules.MaxDailyFee)
            {
                totalFee = _rules.MaxDailyFee;
                break;
            }
        }

        return totalFee;
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        return _rules.TollFreeVehicles.Contains(vehicle.GetVehicleType());
    }

    private bool IsTollFreeDate(DateTime date)
    {
        return _rules.PublicHolidays.Contains(date.Date) || date.Month == 7 ||
               date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    private int GetTollFee(DateTime date)
    {
        TimeSpan currentTime = date.TimeOfDay;
        foreach (var slot in _rules.TimeSlots)
        {
            if (currentTime >= slot.Start && currentTime <= slot.End)
            {
                return slot.Fee;
            }
        }
        return 0;
    }
}
