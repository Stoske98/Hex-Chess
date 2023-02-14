using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Melee
{
    public Queen(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Queen(Data.Unit unit) : base(unit)
    {
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
        GetDiagonalsAvailableMove(hex,3,ref availableMoves);
    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        special_ability.GetAbilityHexes(hex, ref specialMoves);
    }
}

public class QueenLight : Queen
{
    public QueenLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public QueenLight(Data.Unit unit) : base(unit)
    {
    }
}

public class QueenDark : Queen
{
    public QueenDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public QueenDark(Data.Unit unit) : base(unit)
    {
    }
}


