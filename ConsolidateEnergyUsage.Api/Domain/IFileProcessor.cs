using ConsolidateEnergyUsage.Api.Helpers;
using ConsolidateEnergyUsage.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public interface IFileProcessor
    {
        Task<Result<List<ConsolidatedEnergyConsumption>>> Process();
        Task<Result<List<TotalUsages>>> TotalUsage();
    }
}