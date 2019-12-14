using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Podrace.Commands
{
    public class DescribeCommand
    {
        public static void Register(CommandLineApplication application)
        {
            application.Command("describe", cmd =>
            {
                cmd.Description = "Dumps the content of the racefile, as understood by podrace";

                var racefile = cmd.Argument("<RACEFILE>", "The racefile to run. Defaults to 'racefile.json' in the current directory.");

                cmd.OnExecuteAsync((ct) => OnExecuteAsync(racefile, ct));
            });
        }

        private static async Task<int> OnExecuteAsync(
            CommandArgument racefileArgument,
            CancellationToken cancellationToken)
        {
            var racefile = racefileArgument.Value ?? Path.Combine(Directory.GetCurrentDirectory(), "racefile.json");
            Console.WriteLine($"Using racefile: {racefile}");

            return 0;
        }
    }
}