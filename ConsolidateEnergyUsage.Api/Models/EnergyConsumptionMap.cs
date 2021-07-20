using CsvHelper.Configuration;
using System.Globalization;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class EnergyConsumptionMap : ClassMap<EnergyConsumption>
    {
        public EnergyConsumptionMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.MeterCode).Name("MeterPoint Code", "MeterCode");
            Map(m => m.DataType).Name("Data Type", "DataType");
            Map(m => m.Date).Name("Date/Time", "DateTime").TypeConverter<DateTypeConverter>();
            Map(m => m.DataValue).Name("Data Value", "Energy").TypeConverter<DecimalTypeConverter>();
        }
    }
}
