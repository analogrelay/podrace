using System;
using System.IO;
using System.Threading.Tasks;
using k8s;
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

        [Option("--config <CONFIGFILE>", CommandOptionType.SingleValue, Description = "The path to a Kubernetes config file to use.")]
        public string KubernetesConfig { get; set; }

        [Option("--context <CONTEXT>", CommandOptionType.SingleValue, Description = "The name of a Kubernetes context.")]
        public string Context { get; set; }

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

                // If KubernetesConfig is null, this will load the default config!
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(KubernetesConfig, Context);

                // Create a Race session
                var race = new PodraceSession(podraceContext, config, _loggerFactory);

                // Deploy the race
                await race.DeployAsync();

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
