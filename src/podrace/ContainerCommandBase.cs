using McMaster.Extensions.CommandLineUtils;

namespace VibrantCode.Podrace.Commands
{
    internal abstract class ContainerCommandBase
    {
        protected int OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
