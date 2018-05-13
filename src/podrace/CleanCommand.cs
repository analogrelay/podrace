using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace VibrantCode.Podrace
{
    [Command(Name, Description = "Removes all Kubernetes resources for the race.")]
    internal class CleanCommand
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<CleanCommand> _logger;
        public const string Name = "clean";

        [Option("-p|--path <PATH>", CommandOptionType.SingleValue, Description = "The path containing the Racefile and configs to run.")]
        public string BasePath { get; set; }

        public CleanCommand(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<CleanCommand>();
        }

        public async Task<int> OnExecuteAsync(IConsole console)
        {
            try
            {
                var podraceContext =
                    await PodraceContext.LoadAsync(string.IsNullOrEmpty(BasePath)
                        ? Directory.GetCurrentDirectory()
                        : BasePath);

                // Create a Race session
                var race = new PodraceSession(podraceContext, _loggerFactory, outputDirectory: null);

                _logger.LogInformation("Removing all Kubernetes resources for {RaceName}", podraceContext.Name);
                await race.RemoveAsync();
                _logger.LogInformation("Removed all Kubernetes resources for {RaceName}", podraceContext.Name);

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                return 1;
            }
        }
    }
}
