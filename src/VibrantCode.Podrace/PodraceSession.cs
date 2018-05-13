using System.IO;
using System.Threading.Tasks;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Logging;

namespace VibrantCode.Podrace
{
    /// <summary>
    /// Represents a session in which a podrace can be run.
    /// </summary>
    /// <remarks>
    /// This type combines a <see cref="PodraceContext"/>, which represents the "definition" of the Race through
    /// the Racefile and Kubernetes Config files, with an active Kubernetes cluster on which the race will be run.
    /// </remarks>
    public class PodraceSession
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PodraceSession> _logger;
        private readonly Kubernetes _k8s;
        public PodraceContext Context { get; }

        public PodraceSession(PodraceContext context, KubernetesClientConfiguration kubernetesClientConfiguration, ILoggerFactory loggerFactory)
        {
            Context = context;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<PodraceSession>();
            _k8s = new Kubernetes(kubernetesClientConfiguration);
        }

        public Task DeployAsync()
        {
            // First, deploy the configuration
            _logger.LogInformation("Deploying Kubernetes Resources for {RaceName}", Context.Name);

            foreach (var config in Context.Racefile.Configs)
            {
                var configPath = Path.Combine(Context.RootPath, config);
                _logger.LogInformation("Applying configuration: {ConfigurationFilePath}", configPath);
            }

            return Task.CompletedTask;
        }
    }
}
