using System.IO;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace VibrantCode.Podrace
{
    [Command(Name, Description = "Checks the Racefile for errors and describes the contents.")]
    internal class DescribeCommand
    {
        public const string Name = "describe";

        [Option("-p|--path <PATH>", CommandOptionType.SingleValue, Description = "The path containing the Racefile and configs to describe.")]
        public string BasePath { get; set; }

        public async Task OnExecuteAsync(IConsole console)
        {
            var context = await PodraceContext.LoadAsync(BasePath);

            console.WriteLine($"Root Path: {context.RootPath}");
            console.WriteLine();
            console.WriteLine("Kubernetes Config Files:");
            foreach (var config in context.Racefile.Configs)
            {
                var configPath = Path.Combine(context.RootPath, config);
                if (!File.Exists(configPath))
                {
                    console.WriteLine($"* {configPath} (MISSING)");
                }
                else
                {
                    console.WriteLine($"* {configPath}");
                }
            }

            console.WriteLine("Warmup Laps:");
            foreach (var lap in context.Racefile.Warmup)
            {
                console.WriteLine($"* {lap}");
            }

            console.WriteLine("Benchmark Laps:");
            foreach (var lap in context.Racefile.Benchmark)
            {
                console.WriteLine($"* {lap}");
            }

            console.WriteLine("Collectors:");
            foreach (var collector in context.Racefile.Collectors)
            {
                console.WriteLine($"* {collector}");
            }
        }
    }
}
