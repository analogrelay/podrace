using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VibrantCode.Podrace.Internal;
using VibrantCode.Podrace.Model;

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
        private readonly string _outputDirectory;
        private readonly ILogger<PodraceSession> _logger;

        public PodraceContext Context { get; }

        public PodraceSession(PodraceContext context, ILoggerFactory loggerFactory, string outputDirectory)
        {
            Context = context;
            _loggerFactory = loggerFactory;
            _outputDirectory = outputDirectory;
            _logger = loggerFactory.CreateLogger<PodraceSession>();
            _kubectl = new Kubectl(Kubectl.DefaultPath, _loggerFactory.CreateLogger<Kubectl>());
        }

        public async Task DeployAsync()
        {
            if (!Directory.Exists(_outputDirectory))
            {
                Directory.CreateDirectory(_outputDirectory);
            }

            // First, deploy the configuration
            _logger.LogInformation("Deploying Kubernetes Resources for {RaceName}", Context.Name);

            foreach (var config in Context.Racefile.Configs)
            {
                var configPath = Path.Combine(Context.RootPath, config);
                var json = await _kubectl.ExecJsonAsync("create", "-f", configPath);
                _logger.LogInformation("Created {Kind} '{ObjectName}'", (string)json["kind"], (string)json["metadata"]["name"]);
            }

            // Wait for all pods to start up
            _logger.LogInformation("Waiting for all pods to start...");
            await WaitForAllPodsToBeReadyAsync("podrace=true");
        }

        public async Task RemoveAsync()
        {
            // First, deploy the configuration
            _logger.LogInformation("Removing Kubernetes Resources for {RaceName}", Context.Name);

            foreach (var config in Context.Racefile.Configs)
            {
                var configPath = Path.Combine(Context.RootPath, config);
                try
                {
                    await _kubectl.ExecAsync("delete", "-f", configPath);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Error removing {ConfigFile}: {Message}", configPath, ex.Message);
                }

                _logger.LogInformation("Removed {ConfigFile}", configPath);
            }
        }

        public async Task CollectAsync()
        {
            // Run collectors
            _logger.LogInformation("Collecting results...");

            foreach(var collector in Context.Racefile.Collectors)
            {
                switch (collector)
                {
                    case FileCollector file:
                        await RunFileCollectorAsync(file);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported collector type: {collector.GetType().FullName}");
                }
            }
        }

        public Task WarmupAsync()
        {
            _logger.LogInformation("Performing Warmup Laps...");

            return RunLapsAsync(Context.Racefile.Warmup);
        }

        public Task RunAsync()
        {
            _logger.LogInformation("Performing Benchmark Laps...");

            return RunLapsAsync(Context.Racefile.Benchmark);
        }

        private async Task RunFileCollectorAsync(FileCollector file)
        {
            // Iterate over each pod in the role
            var pods = await WaitForAllPodsToBeReadyAsync($"podrace-role={file.Role}");
            var tasks = new Task[pods.Count];
            for (var i = 0; i < pods.Count; i++)
            {
                var podName = (string) pods[i]["metadata"]["name"];
                var resolvedDestination = Path.Combine(_outputDirectory, podName);
                if (!string.IsNullOrEmpty(file.Destination))
                {
                    resolvedDestination = Path.Combine(resolvedDestination, file.Destination);
                }
                tasks[i] = CollectFileAsync(file.Source, resolvedDestination, podName);
            }

            await Task.WhenAll(tasks);

            async Task CollectFileAsync(string source, string destination, string podName)
            {
                await _kubectl.ExecAsync("cp", $"{podName}:{source}", destination);
                _logger.LogInformation("Copied {SourceFile} from {PodName} to {Destination}", source, podName, destination);
            }
        }

        private async Task RunLapsAsync(IList<Lap> laps)
        {
            foreach (var lap in laps)
            {
                switch (lap)
                {
                    case ExecLap exec:
                        await RunExecLap(exec);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported lap type: {lap.GetType().FullName}");
                }
            }
        }

        private async Task RunExecLap(ExecLap exec)
        {
            // Wait for all pods matching the role to be ready
            var pods = await WaitForAllPodsToBeReadyAsync($"podrace-role={exec.Role}");

            _logger.LogInformation("Running 'exec' lap on {PodCount} pods in {Role}: {Command}", pods.Count, exec.Role, string.Join(" ", exec.Command));

            var tasks = new Task[pods.Count];
            for (var i = 0; i < pods.Count; i++)
            {
                tasks[i] = RunExecTask(exec.Command, (string)pods[i]["metadata"]["name"]);
            }

            await Task.WhenAll(tasks);

            // Runs a single iteration of the exec loop.
            async Task RunExecTask(IList<string> command, string podName)
            {
                var args = new string[command.Count + 3];
                args[0] = "exec";
                args[1] = podName;
                args[2] = "--";
                command.CopyTo(args, 3);
                var result = await _kubectl.ExecAsync(args);
                foreach (var line in result.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
                {
                    _logger.LogInformation("{podName} | {Message}", podName, line);
                }
            }
        }

        private async Task<JArray> WaitForAllPodsToBeReadyAsync(string selector)
        {
            var json = await _kubectl.ExecJsonAsync("get", "pod", $"--selector={selector}");
            var pods = (JArray)json["items"];

            // Find all not-ready pods
            while (pods.Any(p => (string)p["status"]["phase"] != "Running"))
            {
                // Sleep and check the pods again
                _logger.LogDebug("Some pods aren't ready yet. Sleeping for 1 second.");
                await Task.Delay(1000);

                // Check again
                json = await _kubectl.ExecJsonAsync("get", "pod", $"--selector={selector}");
                pods = (JArray)json["items"];
            }

            return pods;
        }
    }
}
