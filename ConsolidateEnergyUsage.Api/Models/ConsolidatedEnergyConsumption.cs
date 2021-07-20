using System;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class ConsolidatedEnergyConsumption
    {
        public string MeterCode { get; set; }
        public DateTime Date { get; set; }
        public decimal MaxUsage { get; set; }
        public decimal MinUsage { get; set; }
        public decimal AvgUsage { get; set; }
        public decimal TotalUsage { get; set; }
        public string DataType { get; set; }

    }
}
