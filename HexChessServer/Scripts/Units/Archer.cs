using Networking.Server;

namespace Units
{
    public class Archer : Range
    {
        public Archer(Data.Unit unit) : base(unit)
        {
        }
    }

    public class ArcherLight : Archer
    {
        public ArcherLight(Data.Unit unit) : base(unit)
        {
        }
    }

    public class ArcherDark : Archer
    {
        public ArcherDark(Data.Unit unit) : base(unit)
        {
        }
    }
}


