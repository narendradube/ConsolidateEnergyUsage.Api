using ConsolidateEnergyUsage.Api.Domain;
using ConsolidateEnergyUsage.Api.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConsolidateEnergyUsage.Test
{
    public class FileProcessorShould
    {
        [Fact]
        public async Task OutputProcessedOrderCsvDataAsync()
        {
            const string inputDir = ".\\InputFile";
            var _configuration = new Mock<IConfiguration>(); ;
            var sut = new FileProcessor(_configuration.Object, inputDir);
            var result = await sut.Process();
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<List<ConsolidatedEnergyConsumption>>>();

        }
    }
}
