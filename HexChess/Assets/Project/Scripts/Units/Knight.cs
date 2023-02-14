using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Melee
{
    public Knight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Knight(Data.Unit unit) : base(unit)
    {
    }
    public override void SetPath(Hex hex)
    {
        if (current_state != death_state)
        {
            Hex currentHex = MapGenerator.Instance.GetHex(column, row);
            path.Clear();

            path.Add(currentHex);
            path.Add(hex);

            currentHex.walkable = true;
            ChangeState(move_state);

        }
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();

        foreach (Hex h in MapGenerator.Instance.HexesInRange(hex, 2))
        {
            if (h.walkable)
                availableMoves.Add(h);
        }
        availableMoves.Remove(hex);
        foreach (Hex h in hex.neighbors)
            if (availableMoves.Contains(h))
                availableMoves.Remove(h);

    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        special_ability.GetAbilityHexes(hex, ref specialMoves);
    }
}

public class KnightLight : Knight
{
    public KnightLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public KnightLight(Data.Unit unit) : base(unit)
    {
    }
}

public class KnightDark : Knight
{
    public KnightDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public KnightDark(Data.Unit unit) : base(unit)
    {
    }
}


