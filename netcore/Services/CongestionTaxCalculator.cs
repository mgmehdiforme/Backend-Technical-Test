using System;
using System.Collections.Generic;
using congestion.calculator.Vehicles;

public class CongestionTaxCalculator
{
    // Stores the tax rules, such as free vehicles, fee schedule, and max daily fee
    private readonly CongestionTaxRules _rules;

    // Constructor that initializes the calculator with the provided congestion rules
    public CongestionTaxCalculator(CongestionTaxRules rules)
    {
        _rules = rules;
    }

    // Calculates the total congestion tax for a vehicle based on the provided date/times
    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        // Check if the vehicle is exempt from tolls (e.g., emergency vehicles)
        if (IsTollFreeVehicle(vehicle)) return 0;

        DateTime intervalStart = dates[0];  // Tracks the start of the current fee interval
        int totalFee = 0;                   // Accumulates the total fee for the day

        foreach (DateTime date in dates)
        {
            // Skip the date if it falls on a toll-free day (e.g., holiday or weekend)
            if (IsTollFreeDate(date)) continue;

            int nextFee = GetTollFee(date);  // Retrieve the fee based on the time of day
            TimeSpan diff = date - intervalStart;  // Calculate the time difference between the current date and the interval start
            double minutes = diff.TotalMinutes;    // Convert the time difference to minutes

            // If the time difference is within 60 minutes, use the maximum fee in the interval
            if (minutes <= 60)
            {
                totalFee = Math.Max(totalFee, nextFee);
            }
            // Otherwise, reset the interval and add the new fee
            else
            {
                intervalStart = date;
                totalFee += nextFee;
            }

            // Ensure the total fee does not exceed the daily maximum
            if (totalFee > _rules.MaxDailyFee)
            {
                totalFee = _rules.MaxDailyFee;
                break;  // Stop further calculations once the max fee is reached
            }
        }

        return totalFee;  // Return the final calculated fee for the day
    }

    // Determines if the vehicle is exempt from tolls based on its type
    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        return _rules.TollFreeVehicles.Contains(vehicle.GetVehicleType());
    }

    // Checks if a given date is toll-free (e.g., public holiday, weekend, or July)
    private bool IsTollFreeDate(DateTime date)
    {
        return _rules.PublicHolidays.Contains(date.Date) || date.Month == 7 ||
               date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    // Retrieves the toll fee based on the time of day from the defined time slots
    private int GetTollFee(DateTime date)
    {
        TimeSpan currentTime = date.TimeOfDay;  // Extract the time of day from the DateTime object

        // Iterate through the time slots to find the corresponding fee for the current time
        foreach (var slot in _rules.TimeSlots)
        {
            if (currentTime >= slot.Start && currentTime <= slot.End)
            {
                return slot.Fee;  // Return the fee that applies to the current time
            }
        }
        return 0;  // Return 0 if the time doesn't match any fee slot
    }
}
