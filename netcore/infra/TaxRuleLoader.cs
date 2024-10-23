using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TaxRuleLoader
{
    private const string filePath = "congestionTaxRules.json";

    public CongestionTaxRules LoadRules(string city)
    {
        var jsonString = File.ReadAllText(filePath);
        var allCityRules = JsonSerializer.Deserialize<Dictionary<string, CongestionTaxRules>>(jsonString);

        return allCityRules[city];
    }
}
