using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class Wizzard : Range
{
    public Wizzard(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Wizzard(Data.Unit unit) : base(unit)
    {
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        special_ability.GetAbilityHexes(hex, ref specialMoves);
    }
}

public class WizzardLight : Wizzard
{
    public WizzardLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public WizzardLight(Data.Unit unit) : base(unit)
    {
    }
}

public class WizzardDark : Wizzard
{
    public WizzardDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public WizzardDark(Data.Unit unit) : base(unit)
    {
    }
}


