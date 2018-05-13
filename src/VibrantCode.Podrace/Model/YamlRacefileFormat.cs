using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace VibrantCode.Podrace.Model
{
    public class YamlRacefileFormat
    {
        private static readonly YamlScalarNode TypeKey = new YamlScalarNode("type");
        private const string DefaultLapType = "exec";

        public static Racefile Parse(TextReader reader)
        {
            var yaml = new YamlStream();
            yaml.Load(reader);

            if (yaml.Documents.Count == 0)
            {
                throw new FormatException("No YAML documents found.");
            }

            if (yaml.Documents.Count > 1)
            {
                throw new FormatException("Multiple YAML documents found.");
            }

            var doc = yaml.Documents[0];
            if (doc.RootNode is YamlMappingNode root)
            {
                var racefile = new Racefile();
                foreach (var pair in root.Children)
                {
                    switch (pair.Key)
                    {
                        case YamlScalarNode s when s.Value == "name":
                            if (pair.Value is YamlScalarNode name)
                            {
                                racefile.Name = name.Value;
                            }
                            else
                            {
                                throw new FormatException("Invalid non-scalar value for 'name'.");
                            }
                            break;
                        case YamlScalarNode s when s.Value == "configs":
                            ProcessConfigs(pair.Value, racefile.Configs);
                            break;
                        case YamlScalarNode s when s.Value == "warmup":
                            ProcessLaps(pair.Value, racefile.Warmup);
                            break;
                        case YamlScalarNode s when s.Value == "benchmark":
                            ProcessLaps(pair.Value, racefile.Benchmark);
                            break;
                        case YamlScalarNode s when s.Value == "collectors":
                            ProcessCollectors(pair.Value, racefile.Collectors);
                            break;
                    }

                    // Ignore unknown keys.
                }

                return racefile;
            }

            throw new FormatException("Root node is not a YAML mapping.");
        }

        private static void ProcessCollectors(YamlNode value, IList<Collector> collectors)
        {
            if (value is YamlSequenceNode sequence)
            {
                foreach (var child in sequence.Children)
                {
                    if (child is YamlMappingNode collectorSequence)
                    {
                        // Get the type
                        if (child[TypeKey] is YamlScalarNode type)
                        {
                            collectors.Add(ReadCollector(type.Value, collectorSequence));
                        }
                        else
                        {
                            throw new FormatException("Collector is missing required scalar property 'type'.");
                        }
                    }
                    else
                    {
                        throw new FormatException("The child nodes of 'collectors' must be sequences.");
                    }
                }
            }
            else
            {
                throw new FormatException("'collectors' must be a seqeunce.");
            }
        }

        private static void ProcessLaps(YamlNode value, IList<Lap> laps)
        {
            if (value is YamlSequenceNode sequence)
            {
                foreach (var child in sequence.Children)
                {
                    if (child is YamlMappingNode lapSequence)
                    {
                        // Get the type
                        if (!lapSequence.Children.TryGetValue(TypeKey, out var typeNode))
                        {
                            // Default type is "exec"
                            laps.Add(ReadLap(DefaultLapType, lapSequence));
                        }
                        else if (typeNode is YamlScalarNode s)
                        {
                            laps.Add(ReadLap(s.Value, lapSequence));
                        }
                        else
                        {
                            throw new FormatException("Invalid non-scalar value in 'type'.");
                        }
                    }
                    else
                    {
                        throw new FormatException("The child nodes of 'laps' must be sequences.");
                    }
                }
            }
            else
            {
                throw new FormatException("'laps' must be a seqeunce.");
            }
        }

        private static void ProcessConfigs(YamlNode value, IList<string> configs)
        {
            if (value is YamlSequenceNode sequence)
            {
                foreach (var child in sequence.Children)
                {
                    if (child is YamlScalarNode configScalar)
                    {
                        configs.Add(configScalar.Value);
                    }
                    else
                    {
                        throw new FormatException("Invalid non-scalar value in 'configs'.");
                    }
                }
            }
            else
            {
                throw new FormatException("'configs' must be a sequence.");
            }
        }

        private static Collector ReadCollector(string type, YamlMappingNode collectorSequence)
        {
            switch (type)
            {
                case "file":
                    return ReadFileCollector(collectorSequence);
                default:
                    throw new FormatException($"Unknown collector type: {type}.");
            }
        }

        private static Lap ReadLap(string type, YamlMappingNode lapSequence)
        {
            switch (type)
            {
                case DefaultLapType:
                    return ReadExecLap(lapSequence);
                default:
                    throw new FormatException($"Unknown collector type: {type}.");
            }
        }

        private static Collector ReadFileCollector(YamlMappingNode collectorSequence)
        {
            var collector = new FileCollector();
            foreach (var pair in collectorSequence)
            {
                switch (pair.Key)
                {
                    case YamlScalarNode s when s.Value == "role":
                        if (pair.Value is YamlScalarNode role)
                        {
                            collector.Role = role.Value;
                        }
                        else
                        {
                            throw new FormatException("Invalid non-scalar value in 'role'.");
                        }

                        break;
                    case YamlScalarNode s when s.Value == "source":
                        if (pair.Value is YamlScalarNode source)
                        {
                            collector.Source = source.Value;
                        }
                        else
                        {
                            throw new FormatException("Invalid non-scalar value in 'source'.");
                        }

                        break;
                    case YamlScalarNode s when s.Value == "destination":
                        if (pair.Value is YamlScalarNode dest)
                        {
                            collector.Destination = dest.Value;
                        }
                        else
                        {
                            throw new FormatException("Invalid non-scalar value in 'destination'.");
                        }

                        break;
                }
            }

            return collector;
        }

        private static Lap ReadExecLap(YamlMappingNode lapSequence)
        {
            var lap = new ExecLap();
            foreach (var pair in lapSequence.Children)
            {
                switch (pair.Key)
                {
                    case YamlScalarNode s when s.Value == "role":
                        if (pair.Value is YamlScalarNode role)
                        {
                            lap.Role = role.Value;
                        }
                        else
                        {
                            throw new FormatException("Invalid non-scalar value in 'role'.");
                        }

                        break;
                    case YamlScalarNode s when s.Value == "command":
                        if (pair.Value is YamlSequenceNode command)
                        {
                            foreach (var item in command.Children)
                            {
                                if (item is YamlScalarNode arg)
                                {
                                    lap.Command.Add(arg.Value);
                                }
                                else
                                {
                                    throw new FormatException("Invalid non-scalar value in 'command' item.");
                                }
                            }
                        }
                        else
                        {
                            throw new FormatException("Invalid non-sequence value in 'command'.");
                        }

                        break;
                }
            }

            return lap;
        }
    }
}
