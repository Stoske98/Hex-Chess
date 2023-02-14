using Networking.Server;
using System.Collections.Generic;
using Map;
namespace Units
{
    public class Queen : Melee
    {
        public Queen(Data.Unit unit) : base(unit)
        {
        }

        public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();
            GetDiagonalsAvailableMove(hex, 3, ref availableMoves, ref game.map);
        }
    }

    public class QueenLight : Queen
    {
        public QueenLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class QueenDark : Queen
    {
        public QueenDark(Data.Unit unit) : base(unit)
        {
        }
    }
}

