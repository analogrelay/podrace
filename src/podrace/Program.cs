using System;
using McMaster.Extensions.CommandLineUtils;
using Podrace.Commands;

namespace Podrace.CommandLine
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.HelpOption("-h|--help");
            DescribeCommand.Register(app);

            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (CommandLineException clex)
            {
                Console.Error.WriteLine(clex.Message);
                return 1;
            }

            return 0;
        }
    }
}
