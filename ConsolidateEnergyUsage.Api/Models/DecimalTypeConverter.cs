using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class DecimalTypeConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) => Convert.ToDecimal(text);

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) => throw new System.NotImplementedException();
    }
}