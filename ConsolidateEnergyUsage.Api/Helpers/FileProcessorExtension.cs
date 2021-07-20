using ConsolidateEnergyUsage.Api.Domain;
using ConsolidateEnergyUsage.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsolidateEnergyUsage.Api.Helpers
{
    public static class FileProcessorExtension
    {

        public static List<ConsolidatedEnergyConsumption> ConsolidatedEnergyConsumptions(this List<Domain.EnergyConsumption> consumptions)
        {
            var DataTypeList = consumptions.Select(x => x.DataType).Distinct().ToList();
            return consumptions.GroupBy(meter => new { meter.Date, meter.MeterCode, meter.DataType }).
                             OrderByDescending(x => x.Key.Date).
                             Select(processedRecord =>
                             {
                                 if (processedRecord is null)
                                 {
                                     throw new ArgumentNullException(nameof(processedRecord));
                                 }
                                 return new ConsolidatedEnergyConsumption()
                                 {
                                     MeterCode = processedRecord.Key.MeterCode,
                                     DataType = processedRecord.Key.DataType,
                                     Date = processedRecord.Key.Date,
                                     TotalUsage = Math.Round(processedRecord.Sum(x => x.DataValue), 2),
                                     MaxUsage = Math.Round(processedRecord.Max(x => x.DataValue), 2),
                                     MinUsage = Math.Round(processedRecord.Min(x => x.DataValue), 2),
                                     AvgUsage = Math.Round(processedRecord.Average(x => x.DataValue), 2),
                                 };
                             }).Distinct().ToList();

        }
        public static List<TotalUsages> TotalUsage(this List<Domain.EnergyConsumption> consumptions)
        {
            var DataTypeList = consumptions.Select(x => x.DataType).Distinct().ToList();
            return consumptions.GroupBy(meter => new {  meter.MeterCode }).
                           
                             Select(processedRecord =>
                             {
                                 if (processedRecord is null)
                                 {
                                     throw new ArgumentNullException(nameof(processedRecord));
                                 }
                                 return new TotalUsages()
                                 {
                                     MeterCode = processedRecord.Key.MeterCode,
                                     TotalUsage = Math.Round(processedRecord.Sum(x => x.DataValue), 2),
                                 };
                             }).Distinct().ToList();

        }

    }
}
