using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Melee
{
    public Tank(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Tank(Data.Unit unit) : base(unit)
    {
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
        GetDiagonalsAvailableMove(hex, 2, ref availableMoves);
    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        special_ability.GetAbilityHexes(hex, ref specialMoves);
    }
}

public class TankLight : Tank
{
    public TankLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public TankLight(Data.Unit unit) : base(unit)
    {
    }
}

public class TankDark : Tank
{
    public TankDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public TankDark(Data.Unit unit) : base(unit)
    {
    }
}


