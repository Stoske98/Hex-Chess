using Networking.Server;
using System.Collections.Generic;
using Map;
using System.Numerics;

namespace Units
{
    public class Wizzard : Range
    {
        public Wizzard(Data.Unit unit) : base(unit)
        {
        }
        public override void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            path.Clear();
            path.Add(from_hex);
            path.Add(to_hex);

            from_hex.walkable = true;
            Database.UpdateHex(game_data, from_hex);

            ChangeState(move_state, game_data);
        }
        public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();
        }
    }

    public class WizzardLight : Wizzard
    {
        public WizzardLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class WizzardDark : Wizzard
    {
        public WizzardDark(Data.Unit unit) : base(unit)
        {
        }
    }

}
