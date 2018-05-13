using System;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VibrantCode.Podrace.Commands;

namespace VibrantCode.Podrace
{
    [Command(Name = "Podrace", Description = "Generic benchmarking via Kubernetes")]
    [Subcommand(DescribeCommand.Name, typeof(DescribeCommand))]
    [Subcommand(RunCommand.Name, typeof(RunCommand))]
    internal class Program : ContainerCommandBase
    {
        public static int Main(string[] args)
        {
#if DEBUG
            if (args.Any(a => string.Equals("--debug", a, StringComparison.OrdinalIgnoreCase)))
            {
                args = args.Where(a => !string.Equals("--debug", a, StringComparison.Ordinal)).ToArray();
                Console.WriteLine("Waiting for debugger to attach. Press ENTER to continue.");
                Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
                Console.ReadLine();
            }
#endif

            var services = new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddCliConsole();
                })
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);
            return app.Execute(args);
        }
    }
}
