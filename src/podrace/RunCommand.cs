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
                var race = new PodraceSession(podraceContext, _loggerFactory);

                // Deploy the race
                await race.DeployAsync();
                try
                {
                    // TODO: Warmup
                    // TODO: Benchmark
                }
                finally
                {
                    await race.RemoveAsync();
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
