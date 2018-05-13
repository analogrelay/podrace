using System;
using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace VibrantCode.Podrace
{
    [Command(Name, Description = "Runs the race defined in the specified racefile.")]
    internal class RunCommand
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<RunCommand> _logger;
        public const string Name = "run";

        [Option("-p|--path <PATH>", CommandOptionType.SingleValue, Description = "The path containing the Racefile and configs to run.")]
        public string BasePath { get; set; }

        [Option("--no-clean", CommandOptionType.NoValue, Description = "Set this switch to skip cleaning up race resources.")]
        public bool NoClean { get; set; }

        [Option("-o|--output <OUTPUT>", CommandOptionType.SingleValue, Description = "The directory in which to place output files.")]
        public string OutputDirectory {get; set;}

        public RunCommand(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<RunCommand>();
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
                var outDir = string.IsNullOrEmpty(OutputDirectory) ? Directory.GetCurrentDirectory() : OutputDirectory;
                var race = new PodraceSession(podraceContext, _loggerFactory, outDir);

                try
                {
                    // Deploy the race
                    await race.DeployAsync();

                    // Run the warmup
                    await race.WarmupAsync();

                    // Run the benchmark!
                    await race.RunAsync();

                    // Collect outputs
                    await race.CollectAsync();
                }
                finally
                {
                    if (!NoClean)
                    {
                        _logger.LogInformation("Cleaning up race resources...");
                        await race.RemoveAsync();
                    }
                    else
                    {
                        _logger.LogWarning("Skipping clean-up because --no-clean was set.");
                    }
                }

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
