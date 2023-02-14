using Networking.Server;
using System.Collections.Generic;
using Map;
using System.Numerics;

namespace Units
{
    public class Knight : Melee
    {
        public Knight(Data.Unit unit) : base(unit)
        {
        }
        public override void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            if(CurrentState != death_state)
            {
                path.Clear();
                path.Add(from_hex);
                path.Add(to_hex);

                from_hex.walkable = true;
                Database.UpdateHex(game_data, from_hex);

                ChangeState(move_state, game_data);

            }

            ChangeState(move_state, game_data);
        }
        public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();

            foreach (Hex h in game.map.HexesInRange(hex, 2))
            {
                if (h.walkable)
                    availableMoves.Add(h);
            }
            availableMoves.Remove(hex);
            foreach (Hex h in hex.neighbors)
                if (availableMoves.Contains(h))
                    availableMoves.Remove(h);

        }
    }

    public class KnightLight : Knight
    {
        public KnightLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class KnightDark : Knight
    {
        public KnightDark(Data.Unit unit) : base(unit)
        {
        }
    }

}
