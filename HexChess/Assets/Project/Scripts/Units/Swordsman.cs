using Networking.Client;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Swordsman : Melee
{
    public static Queue<Vector2Int> swordsman_passive = new Queue<Vector2Int>();
    public Swordsman(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Swordsman(Data.Unit unit) : base(unit)
    {
        OnExitMovement += OnUnitExitMovement;
    }

    public override void UnSubscibe()
    {
        OnExitMovement -= OnUnitExitMovement;
    }

    private void OnUnitExitMovement(Unit enemy)
    {
        special_ability.UseAbility(enemy);
    }
}

public class SwordsmanLight : Swordsman
{
    public SwordsmanLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public SwordsmanLight(Data.Unit unit) : base(unit)
    {
    }

}

public class SwordsmanDark : Swordsman
{
    public SwordsmanDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }

    public SwordsmanDark(Data.Unit unit) : base(unit)
    {
    }

}


