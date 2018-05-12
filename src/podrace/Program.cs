using System;
using System.Diagnostics;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using VibrantCode.Podrace.Commands;

namespace VibrantCode.Podrace
{
    [Command(Name = "Podrace", Description = "Generic benchmarking via Kubernetes")]
    [Subcommand(DescribeCommand.Name, typeof(DescribeCommand))]
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
            return CommandLineApplication.Execute<Program>(args);
        }
    }
}
