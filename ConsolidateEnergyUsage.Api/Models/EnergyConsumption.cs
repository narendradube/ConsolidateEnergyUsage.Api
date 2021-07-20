using System;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class EnergyConsumption
    {
        public string MeterCode { get; set; }
        public DateTime Date { get; set; }
        public string DataType { get; set; }
        public decimal DataValue { get; set; }
    }
}