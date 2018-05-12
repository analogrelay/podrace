using System;
using System.Collections.Generic;
using Microsoft.Extensions.Internal;

namespace VibrantCode.Podrace.Model
{
    public class ExecLap : Lap, IEquatable<Lap>, IEquatable<ExecLap>
    {
        /// <summary>
        /// Gets or sets the Role on which to perform the action
        /// </summary>
        /// <remarks>
        /// The action will be performed on each of the Pods marked with a matching 'podrace-role' label.
        /// </remarks>
        public string Role { get; set; }

        /// <summary>
        /// Gets the command and arguments to execute.
        /// </summary>
        public IList<string> Command { get; } = new List<string>();

        public override int GetHashCode()
        {
            var hash = new HashCodeCombiner();
            hash.Add(Role);
            hash.Add(Command);
            return hash;
        }

        public bool Equals(ExecLap other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Role, other.Role) && Equals(Command, other.Command);
        }

        public bool Equals(Lap other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is ExecLap e && Equals(e);
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

            return obj is ExecLap e && Equals(e);
        }

        public override string ToString() => $"Exec [{Role}]: {string.Join(" ", Command)}";
    }
}
