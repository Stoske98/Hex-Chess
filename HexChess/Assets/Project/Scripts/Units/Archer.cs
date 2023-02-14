using Networking.Client;
using UnityEngine;

public class Archer : Range
{
    public Archer(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }

    public Archer(Data.Unit unit) : base(unit)
    {
    }
}

public class ArcherLight : Archer
{
    public ArcherLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public ArcherLight(Data.Unit unit) : base(unit)
    {
    }
}

public class ArcherDark : Archer
{
    public ArcherDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public ArcherDark(Data.Unit unit) : base(unit)
    {
    }
}


