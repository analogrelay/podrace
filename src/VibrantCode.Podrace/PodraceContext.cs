using System.IO;
using System.Threading.Tasks;
using VibrantCode.Podrace.Model;

namespace VibrantCode.Podrace
{
    public class PodraceContext
    {
        public string RootPath { get; }
        public Racefile Racefile { get; }

        public PodraceContext(string rootPath, Racefile racefile)
        {
            RootPath = rootPath;
            Racefile = racefile;
        }

        public static Task<PodraceContext> LoadAsync(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Could not find path: {path}");
            }

            // Find the racefile
            if (!TryFindRacefile(path, out var racefilePath))
            {
                throw new FileNotFoundException($"Could not find 'racefile' or 'racefile.yaml' in: {path}");
            }

            // Load the racefile
            using (var stream = new FileStream(racefilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                // TODO: Async.
                var racefile = YamlRacefileFormat.Parse(reader);
                return Task.FromResult(new PodraceContext(path, racefile));
            }
        }

        private static readonly string[] RacefileNames = { "racefile", "racefile.yaml" };

        private static bool TryFindRacefile(string path, out string racefile)
        {
            foreach (var name in RacefileNames)
            {
                var candidate = Path.Combine(path, name);
                if (File.Exists(candidate))
                {
                    racefile = candidate;
                    return true;
                }
            }

            racefile = null;
            return false;
        }
    }
}
