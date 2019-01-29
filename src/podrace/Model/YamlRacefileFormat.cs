using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace VibrantCode.Podrace.Model
{
    public static class YamlRacefileFormat
    {
        public bool TryParse(TextReader reader, out Racefile racefile)
        {
            var yaml = new YamlStream();
            yaml.Load(reader);

            if(yaml.Documents.Count == 0)
            {
                racefile = null;
                return false;
            }

            return TryParseDocument(yaml.Documents[0], out racefile);
        }

        private bool TryParseDocument(YamlDocument yamlDocument, out Racefile racefile)
        {
            if (yamlDocument.RootNode is YamlMappingNode mapping)
            {
                foreach(var child in mapping.Children)
                {
                    if(string.Equals(child.Key, "name"))
                    {

                    }
                }
            } else
            {
                racefile = null;
                return false;
            }
        }
    }
}
