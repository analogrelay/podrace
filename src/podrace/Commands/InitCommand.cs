using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace Podrace.Commands
{
    [Command(Name, Description = "Initialize a new racefile in the current directory")]
    internal class InitCommand
    {
        public const string Name = "init";

        internal Task<int> OnExecuteAsync(IConsole console)
        {
            console.WriteLine("TODO: Init command");
            return Task.FromResult(0);
        }
    }
}
