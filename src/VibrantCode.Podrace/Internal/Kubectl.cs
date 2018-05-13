using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VibrantCode.Podrace.Internal
{
    public class Kubectl
    {
        private static readonly string KubectlExecutable =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "kubectl.exe" : "kubectl";
        private static readonly string[] JsonOutputArgs = new[] { "--output=json" };

        private readonly string _path;
        private readonly ILogger<Kubectl> _logger;

        public static string DefaultPath = LocateKubectl();

        public Kubectl(string path, ILogger<Kubectl> logger)
        {
            _path = path;
            _logger = logger;

            _logger.LogDebug("Using 'kubectl' from: {KubectlPath}", _path);
        }

        private static string LocateKubectl()
        {
            foreach (var path in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator))
            {
                var candidate = Path.Combine(path, KubectlExecutable);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        public async Task<T> ExecAsync<T>(string command, params string[] args)
        {
            var fullArgs = GetAllArgs(command, args);

            var str = await ExecAsync(fullArgs);

            return JsonConvert.DeserializeObject<T>(str);
        }

        public async Task<JObject> ExecJsonAsync(string command, params string[] args)
        {
            var fullArgs = GetAllArgs(command, args);

            var str = await ExecAsync(fullArgs);

            return JObject.Parse(str);
        }

        public Task<string> ExecAsync(params string[] args)
        {
            var tcs = new TaskCompletionSource<string>();

            var argString = ArgumentEscaper.EscapeAndConcatenate(args);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _path,
                    Arguments = argString,
                    CreateNoWindow = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                },
                EnableRaisingEvents = true,
            };

            // Hook the events
            var stdoutBuilder = new StringBuilder();
            var stderrBuilder = new StringBuilder();
            var stdoutFinished = new TaskCompletionSource<string>();
            var stderrFinished = new TaskCompletionSource<string>();
            process.OutputDataReceived += (sender, a) =>
            {
                if (a.Data == null)
                {
                    stdoutFinished.TrySetResult(stdoutBuilder.ToString());
                }
                else
                {
                    _logger.LogTrace("stdout: {Line}", a.Data);
                    stdoutBuilder.AppendLine(a.Data);
                }
            };
            process.ErrorDataReceived += (sender, a) =>
            {
                if (a.Data == null)
                {
                    stderrFinished.TrySetResult(stderrBuilder.ToString());
                }
                else
                {
                    _logger.LogTrace("stderr: {Line}", a.Data);
                    stderrBuilder.AppendLine(a.Data);
                }
            };

            process.Exited += async (sender, a) =>
            {
                _logger.LogDebug("< kubectl {Arguments} (exit code: {ExitCode})", argString, process.ExitCode);
                if (process.ExitCode == 0)
                {
                    // Success!
                    tcs.TrySetResult(await stdoutFinished.Task);
                }
                else
                {
                    // Failure :(
                    var stderr = await stderrFinished.Task;
                    _logger.LogError(stderr.Trim());
                    tcs.TrySetException(new Exception($"A Kubernetes command failed. See the log for details."));
                }
            };

            _logger.LogDebug("> kubectl {Arguments}", argString);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }

        private static string[] GetAllArgs(string command, string[] args)
        {
            var fullArgs = new string[args.Length + JsonOutputArgs.Length + 1];
            Array.Copy(JsonOutputArgs, 0, fullArgs, 0, JsonOutputArgs.Length);
            fullArgs[JsonOutputArgs.Length] = command;
            Array.Copy(args, 0, fullArgs, JsonOutputArgs.Length + 1, args.Length);
            return fullArgs;
        }
    }
}
