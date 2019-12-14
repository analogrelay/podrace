using System.Collections.Generic;
using System.Text.Json;

namespace Podrace.Model
{
    public class Racefile
    {
        public IDictionary<string, JsonElement> Variables { get; set; } = new Dictionary<string, JsonElement>();

        public IList<string> Dependencies { get; set; } = new List<string>();

        public IDictionary<string, RaceRole> Services { get; set; } = new Dictionary<string, RaceRole>();
    }
}