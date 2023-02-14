using Networking.Server;

namespace Units
{
    public class King : Melee
    {
        public King(Data.Unit unit) : base(unit)
        {
        }
    }

    public class KingLight : King
    {
        public KingLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class KingDark : King
    {
        public KingDark(Data.Unit unit) : base(unit)
        {
        }
    }
}

