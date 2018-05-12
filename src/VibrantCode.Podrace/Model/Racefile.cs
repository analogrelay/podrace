using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Internal;

namespace VibrantCode.Podrace.Model
{
    /// <summary>
    /// Represents a 'racefile'
    /// </summary>
    public class Racefile : IEquatable<Racefile>
    {
        /// <summary>
        /// Gets a list of Kubernetes Configs to be applied to the cluster before running the benchmark.
        /// </summary>
        public IList<string> Configs { get; } = new List<string>();

        /// <summary>
        /// Gets a list of <see cref="Lap"/> objects representing the steps to take during warmup.
        /// </summary>
        public IList<Lap> Warmup { get; } = new List<Lap>();

        /// <summary>
        /// Gets a list of <see cref="Lap"/> objects representing the steps to take during the benchmark.
        /// </summary>
        public IList<Lap> Benchmark { get; } = new List<Lap>();

        /// <summary>
        /// Gets a list of <see cref="Collector" /> objets representing the collectors to use during the benchmark.
        /// </summary>
        public IList<Collector> Collectors { get; } = new List<Collector>();

        public bool Equals(Racefile other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Configs.SequenceEqual(other.Configs) && Warmup.SequenceEqual(other.Warmup) && Benchmark.SequenceEqual(other.Benchmark) && Collectors.SequenceEqual(other.Collectors);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is Racefile r && Equals(r);
        }

        public override int GetHashCode()
        {
            var hash = new HashCodeCombiner();
            hash.Add(Configs);
            hash.Add(Warmup);
            hash.Add(Benchmark);
            hash.Add(Collectors);
            return hash;
        }
    }
}
