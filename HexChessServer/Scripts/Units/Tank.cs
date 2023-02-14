using Networking.Server;
using System.Collections.Generic;
using Map;
namespace Units
{
    public class Tank : Melee
    {
        public Tank(Data.Unit unit) : base(unit)
        {
        }

        public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();
            GetDiagonalsAvailableMove(hex, 2, ref availableMoves, ref game.map);
        }
    }

    public class TankLight : Tank
    {
        public TankLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class TankDark : Tank
    {
        public TankDark(Data.Unit unit) : base(unit)
        {
        }
    }
}

