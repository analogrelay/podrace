using McMaster.Extensions.CommandLineUtils;
using Podrace.Commands;
using System.Threading.Tasks;

namespace Podrace
{
    [Command(Name = "podrace", Description = "Flexible benchmarking with docker.")]
    [Subcommand(typeof(InitCommand))]
    internal class Program
    {
        internal static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        internal int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
