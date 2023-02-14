using Networking.Server;
using System;
using System.Collections.Generic;
using System.Numerics;
using Units;

namespace Map
{
    public class Map
    {
        public List<Hex> hexes = new List<Hex>();

        public readonly List<Vector2> Vector2Neigbors = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0), new Vector2(-1, 1), new Vector2(1, -1) };


        public Map(List<Data.Hex> data)
        {
            foreach (var hex_data in data)
            {
                hexes.Add(new Hex(hex_data));
            }
            foreach (var hex in hexes)
            {
                SetNeighbors(hex);
            }
        }

        private void SetNeighbors(Hex hex)
        {
            foreach (Vector2 vector2 in Vector2Neigbors)
            {
                Hex neighbor = GetHex(hex.column + (int)vector2.X, hex.row + (int)vector2.Y);
                if (neighbor != null)
                    hex.neighbors.Add(neighbor);
            }
        }

        public Hex GetHex(int col, int row)
        {
            foreach (var hex in hexes)
            {
                if (hex.column == col && hex.row == row)
                    return hex;
            }
            return null;
        }

        public Hex GetHexInDirection(Hex center, Direction direction)
        {
            Hex hex = null;

            switch (direction)
            {
                case Direction.Top:
                    hex = GetHex(center.column, center.row + 1);
                    break;
                case Direction.Bottom:
                    hex = GetHex(center.column, center.row - 1);
                    break;
                case Direction.Top_Left:
                    hex = GetHex(center.column + 1, center.row);
                    break;
                case Direction.Top_Right:
                    hex = GetHex(center.column - 1, center.row + 1);
                    break;
                case Direction.Bottom_Left:
                    hex = GetHex(center.column + 1, center.row - 1);
                    break;
                case Direction.Bottom_Right:
                    hex = GetHex(center.column - 1, center.row);
                    break;
                default:
                    break;
            }
            return hex;
        }

        public List<Hex> HexesInRange(Hex center, int range)
        {
            List<Hex> hices = new List<Hex>();

            for (int q = -range; q <= range; q++)
            {
                for (int r = Math.Max(-range, -q - range); r <= Math.Min(range, -q + range); r++)
                {
                    Hex hex = GetHex(q + center.column, r + center.row);
                    if (hex != null)
                        hices.Add(hex);
                }
            }
            return hices;
        }
    }
}
