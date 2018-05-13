using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VibrantCode.Podrace.Internal;

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
        private readonly Kubectl _kubectl;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PodraceSession> _logger;
        public PodraceContext Context { get; }

        public PodraceSession(PodraceContext context, ILoggerFactory loggerFactory)
        {
            Context = context;
            _loggerFactory = loggerFactory;

            _logger = loggerFactory.CreateLogger<PodraceSession>();
            _kubectl = new Kubectl(Kubectl.DefaultPath, _loggerFactory.CreateLogger<Kubectl>());
        }

        public async Task DeployAsync()
        {
            // First, deploy the configuration
            _logger.LogInformation("Deploying Kubernetes Resources for {RaceName}", Context.Name);

            foreach (var config in Context.Racefile.Configs)
            {
                var configPath = Path.Combine(Context.RootPath, config);
                var json = await _kubectl.ExecJsonAsync("create", "-f", configPath);
                _logger.LogInformation("Created {Kind} '{ObjectName}'", (string)json["kind"], (string)json["metadata"]["name"]);
            }
        }

        public async Task RemoveAsync()
        {
            // First, deploy the configuration
            _logger.LogInformation("Removing Kubernetes Resources for {RaceName}", Context.Name);

            foreach (var config in Context.Racefile.Configs)
            {
                var configPath = Path.Combine(Context.RootPath, config);
                await _kubectl.ExecAsync("delete", "-f", configPath);
                _logger.LogInformation("Removed {ConfigFile}", configPath);
            }
        }
    }
}
