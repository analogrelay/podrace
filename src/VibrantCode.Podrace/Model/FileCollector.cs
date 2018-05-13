using System;
using Microsoft.Extensions.Internal;

namespace VibrantCode.Podrace.Model
{
    /// <summary>
    /// A collector that copies a file or files out of the Pods matching the specified role.
    /// </summary>
    public class FileCollector : Collector, IEquatable<Collector>, IEquatable<FileCollector>
    {
        /// <summary>
        /// Gets or sets the Role from which to copy the file.
        /// </summary>
        /// <remarks>
        /// The specified file will be extracted from each Pod marked with a matching 'podrace-role' label. The files will have the pod name applied as a prefix.
        /// </remarks>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the path to the file which will be extracted.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the path to which the file will be copied.
        /// </summary>
        public string Destination { get; set; }

        public bool Equals(FileCollector other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Role, other.Role) && string.Equals(Source, other.Source) && string.Equals(Destination, other.Destination);
        }

        public bool Equals(Collector other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is FileCollector f && Equals(f);
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

            return obj is FileCollector f && Equals(f);
        }

        public override int GetHashCode()
        {
            var hash = new HashCodeCombiner();
            hash.Add(Role);
            hash.Add(Source);
            hash.Add(Destination);
            return hash;
        }

        public override string ToString() => $"File [{Role}] {Source} -> {Destination}";
    }
}
