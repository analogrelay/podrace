using System;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
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
            if (args.Length > 0 && string.Equals("--debug", args[0], StringComparison.OrdinalIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                Console.WriteLine("Waiting for debugger to attach. Press ENTER to continue.");
                Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
                Console.ReadLine();
            }
#endif
            var services = new ServiceCollection()
                .AddLogging(logging => logging.AddCliConsole())
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);
            return app.Execute(args);
        }
    }
}
