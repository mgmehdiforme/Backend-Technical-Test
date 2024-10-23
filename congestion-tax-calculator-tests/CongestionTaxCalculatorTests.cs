using congestion.calculator.Vehicles;
using NUnit.Framework;
using System;

[TestFixture]
public class CongestionTaxCalculatorTests
{
    private CongestionTaxCalculator _calculator;

    [SetUp]
    public void Setup()
    {
        var rules = new TaxRuleLoader().LoadRules("Gothenburg");
        _calculator = new CongestionTaxCalculator(rules);
    }

    [Test]
    public void Test_MaxDailyTax()
    {
        Vehicle car = new Car();
        DateTime[] dates = {
        new DateTime(2013, 02, 07, 06, 23, 00), // Fee: 8 SEK
        new DateTime(2013, 02, 07, 07, 15, 00), // Fee: 18 SEK
        new DateTime(2013, 02, 07, 08, 45, 00), // Fee: 8 SEK
        new DateTime(2013, 02, 07, 15, 27, 00), // Fee: 13 SEK
        new DateTime(2013, 02, 07, 16, 35, 00), // Fee: 18 SEK
        new DateTime(2013, 02, 07, 17, 49, 00)  // Fee: 13 SEK
    };

        int tax = _calculator.GetTax(car, dates);

        Assert.AreEqual(60, tax); // Max daily tax should be 60 SEK
    }

    [Test]
    public void Test_TollFreeVehicle()
    {
        Vehicle motorbike = new Motorcycle();
        DateTime[] dates = { new DateTime(2013, 02, 07, 06, 23, 00) };

        int tax = _calculator.GetTax(motorbike, dates);

        Assert.AreEqual(0, tax); // Motorbike is toll-free
    }

    [Test]
    public void Test_SingleChargeRule()
    {
        Vehicle car = new Car();
        DateTime[] dates = {
            new DateTime(2013, 02, 07, 06, 23, 00),
            new DateTime(2013, 02, 07, 06, 45, 00) // Within 60 minutes, should charge only once
        };

        int tax = _calculator.GetTax(car, dates);

        Assert.AreEqual(13, tax); // The highest charge within the interval should apply
    }

    [Test]
    public void Test_TollFreeDate()
    {
        Vehicle car = new Car();
        DateTime[] dates = { new DateTime(2013, 03, 28, 06, 23, 00) }; // Public holiday

        int tax = _calculator.GetTax(car, dates);

        Assert.AreEqual(0, tax); // Public holiday, so toll-free
    }
}
