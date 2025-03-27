using System;
using UnityEngine;

namespace Plamb.Character
{
    /// <summary>
    /// This struct serves as a container for the player's current input. It's used for replaying inputs later.
    /// </summary>
    public struct PlambInputSnapshot : IEquatable<PlambInputSnapshot>
    {
        // Define input data
        public Vector2 movementVector;
        public bool actionJump;
        public bool actionInteract;
        public bool actionThrow;

        // Implement equality check
        public bool Equals(PlambInputSnapshot other)
        {
            return movementVector == other.movementVector &&
                actionJump == other.actionJump &&
                actionInteract == other.actionInteract &&
                actionThrow == other.actionThrow;
        }

        public override bool Equals(object obj)
        {
            return obj is PlambInputSnapshot other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(movementVector, actionJump, actionInteract, actionThrow);
        }

        // Overload operators for direct comparisons
        public static bool operator ==(PlambInputSnapshot left, PlambInputSnapshot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlambInputSnapshot left, PlambInputSnapshot right)
        {
            return !(left == right);
        }
    }
}
