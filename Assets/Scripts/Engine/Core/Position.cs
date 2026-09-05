using System;
using UnityEngine;

/// <summary>
/// Position on chessboard.
/// File, Rank has value from 1 to 8.
/// File : vertical line (a ~ h)
/// Rank : horizontal line (1 ~ 8)
/// </summary>

namespace Chess
{
    public readonly struct Position
    {
        public int File { get; } // 0 ~ 7 (a ~ h)
        public int Rank { get; } // 0 ~ 7 (1 ~ 8)

        public Position(int file, int rank)
        {
            if (file < 0 || file > 7)
                throw new ArgumentOutOfRangeException(nameof(file));
            if (rank < 0 || rank > 7)
                throw new ArgumentOutOfRangeException(nameof(rank));
            File = file;
            Rank = rank;
        }

        public Position(char file, int rank) : this(file - 'a', rank - 1) // chess notation (a1, e4, ...)
        {
        }

        public static Position From(string s)
        {
            if (s.Length != 2)
                throw new ArgumentException($"Invalid algebraic notation : {s}");

            return new Position(s[0], s[1] - '0');
        }

        public bool TryOffset(int df, int dr, out Position result)
        {
            int newFile = File + df;
            int newRank = Rank + dr;

            if (newFile < 0 || newFile > 7 ||
                newRank < 0 || newRank > 7)
            {
                result = default;
                return false;
            }

            result = new Position(newFile, newRank);
            return true;
        }
    }
}

