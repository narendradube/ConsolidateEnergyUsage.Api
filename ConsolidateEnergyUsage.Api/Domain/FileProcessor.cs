using CsvHelper;
using CsvHelper.Configuration;
using ConsolidateEnergyUsage.Api.Helpers;
using ConsolidateEnergyUsage.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsolidateEnergyUsage.Api.Domain
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IConfiguration _configuration;
        public List<EnergyConsumption> _energyConsumptions = new List<EnergyConsumption>();
        public List<ConsolidatedEnergyConsumption> _consolidatedResult = new List<ConsolidatedEnergyConsumption>();
        public string InputFilePath { get; set; }
        public FileProcessor(IConfiguration configuration, string FilePath = "")
        {
            _configuration = configuration;
            if (string.IsNullOrEmpty(FilePath))
                InputFilePath = _configuration.GetValue<string>("InputFilePath");
            else InputFilePath = FilePath;
        }

        public async Task<Result<List<ConsolidatedEnergyConsumption>>> Process()
        {
            if (string.IsNullOrEmpty(InputFilePath) || !Directory.Exists(InputFilePath) || !Directory.GetFiles(InputFilePath).Any())
            {
                return Result<List<ConsolidatedEnergyConsumption>>.Invalid(new List<ValidationError> {
                    new ValidationError
                    {
                        Identifier = nameof(InputFilePath),
                        ErrorMessage = "Input file Directory not set up correctly or Files not available for processing "
                    }
                });
            }
            Task _processFiles = Task.Run(() =>
            {
                ImportFile();
            });
            await _processFiles;
            return _energyConsumptions.ConsolidatedEnergyConsumptions();
        }
        private void ImportFile()
        {
            foreach (var file in Directory.GetFiles(InputFilePath))
            {
                using StreamReader input = File.OpenText(file);
                using var csvReader = new CsvReader(input, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim
                });
                csvReader.Context.RegisterClassMap<EnergyConsumptionMap>();
                var result = csvReader.GetRecords<EnergyConsumption>()
                .ToList();
                _energyConsumptions.AddRange(result);
            }
        }
        public async Task<Result<List<TotalUsages>>> TotalUsage()
        {
            if (string.IsNullOrEmpty(InputFilePath) || !Directory.Exists(InputFilePath) || !Directory.GetFiles(InputFilePath).Any())
            {
                return Result<List<TotalUsages>>.Invalid(new List<ValidationError> {
                    new ValidationError
                    {
                        Identifier = nameof(InputFilePath),
                        ErrorMessage = "Input file Directory not set up correctly or Files not available for processing "
                    }
                });
            }
            Task _processFiles = Task.Run(() =>
            {
                ImportFile();
            });
            await _processFiles;
            return _energyConsumptions.TotalUsage();
        }
    }
}
