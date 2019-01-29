using System;
using System.Collections.Generic;
using System.Linq;

namespace VibrantCode.Podrace.Model
{
    /// <summary>
    /// Represents a 'racefile'
    /// </summary>
    public class Racefile : IEquatable<Racefile>
    {
        /// <summary>
        /// Gets the name of this Race.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Role"/> objects representing the agents to deploy.
        /// </summary>
        public IList<Role> Roles { get; }

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

            return Equals(Name, other.Name) && Roles.SequenceEqual(other.Roles) && Warmup.SequenceEqual(other.Warmup) && Benchmark.SequenceEqual(other.Benchmark) && Collectors.SequenceEqual(other.Collectors);
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

        public override int GetHashCode() => HashCode.Combine(Name, Roles, Warmup, Benchmark, Collectors);
    }
}
