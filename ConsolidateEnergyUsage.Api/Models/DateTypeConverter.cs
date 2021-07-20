using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Globalization;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class DateTypeConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData) => DateTime.ParseExact(text, new string[] { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm" }, CultureInfo.InvariantCulture).Date;

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData) => throw new System.NotImplementedException();
    }
}