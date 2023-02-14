using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class King : Melee
{
    public King(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public King(Data.Unit unit) : base(unit)
    {
    }

    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        special_ability.GetAbilityHexes(hex, ref specialMoves);
    }
}

public class KingLight : King
{
    public KingLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public KingLight(Data.Unit unit) : base(unit)
    {
    }
}

public class KingDark : King
{
    public KingDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public KingDark(Data.Unit unit) : base(unit)
    {
    }
}


