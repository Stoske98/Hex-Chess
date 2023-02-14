using Networking.Client;
using System.Collections.Generic;
using UnityEngine;

public class Jester : Melee
{
    public Jester(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public Jester(Data.Unit unit) : base(unit)
    {
    }
    public override void SetPath(Hex hex)
    {
        RemoveIllusions();
        Hex currentHex = MapGenerator.Instance.GetHex(column, row);
        path.Clear();

        path.Add(currentHex);
        path.Add(hex);

        currentHex.walkable = true;
        ChangeState(move_state);
    }
    public override void ReceiveDamage(int damage)
    {
        RemoveIllusions();
        if (stats.current_health - damage > 0)
            stats.current_health -= damage;
        else
        {
            stats.current_health = 0;
            GameUIManager.Instance.death_controller_ui.OnUnitDeath(this, false);
            MapGenerator.Instance.GetHex(column, row).walkable = true;
            health_bar.canvas_health_bar.SetActive(false);
            column = -999;
            row = -999;
            current_state = death_state;
            ChangeState(death_state);
            UnSubscibe();
        }

        GameUIManager.Instance.UpdateUnitUI(this);

    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
        foreach (Hex h in PathFinding.PathFinder.BFS_HexesMoveRange(hex,2))
                availableMoves.Add(h);
    }

    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
        foreach (Hex h in PathFinding.PathFinder.BFS_HexesMoveRange(hex, 2))
            specialMoves.Add(h);
    }
      public void RemoveIllusions()
      {
        foreach (var unit in GameManager.Instance.units)
        {
            if(unit.current_state != death_state && unit.unit_type == UnitType.JesterIllusion && unit.class_type == class_type)
            {
                Jester illusion = unit as Jester;
                illusion.RemoveIllusion(illusion);
            }
        }
      }

    private void RemoveIllusion(Jester illusion)
    {
        if (illusion.current_state != illusion.death_state)
        {
            illusion.stats.current_health = 0;
            GameUIManager.Instance.death_controller_ui.OnUnitDeath(illusion);
            MapGenerator.Instance.GetHex(illusion.column, illusion.row).walkable = true;
            illusion.column = -100;
            illusion.row = -100;
            illusion.ChangeState(illusion.death_state);
            illusion.UnSubscibe();
            illusion.special_id = 0;

        }
    }
}

public class JesterLight : Jester
{
    public JesterLight(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public JesterLight(Data.Unit unit) : base(unit)
    {
    }
}

public class JesterDark : Jester
{
    public JesterDark(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }
    public JesterDark(Data.Unit unit) : base(unit)
    {
    }
}

public class JesterLightIllusion : JesterLight
{

    public JesterLightIllusion(Data.Unit unit) : base(unit)
    {
    }
    public override void SetPath(Hex hex)
    {
        Hex currentHex = MapGenerator.Instance.GetHex(column, row);
        path.Clear();

        path.Add(currentHex);
        path.Add(hex);

        currentHex.walkable = true;
        ChangeState(move_state);
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
    }
    public override void ReceiveDamage(int damage)
    {        
        if(current_state != death_state)
        {
            stats.current_health = 0;
            GameUIManager.Instance.death_controller_ui.OnUnitDeath(this, false);
            MapGenerator.Instance.GetHex(column, row).walkable = true;
            health_bar.canvas_health_bar.SetActive(false);
            column = -100;
            row = -100;
            ChangeState(death_state);
            UnSubscibe();
            special_id = 0;

        }        
    }
}
public class JesterDarkIllusion : JesterLight
{
    public JesterDarkIllusion(Data.Unit unit) : base(unit)
    {
    }
    public override void SetPath(Hex hex)
    {
        Hex currentHex = MapGenerator.Instance.GetHex(column, row);
        path.Clear();

        path.Add(currentHex);
        path.Add(hex);

        currentHex.walkable = true;
        ChangeState(move_state);
    }
    public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
        availableMoves.Clear();
    }

    public override void GetAttackMoves(Hex hex, ref List<Hex> attackMoves)
    {
        attackMoves.Clear();
    }
    public override void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
    }
    public override void ReceiveDamage(int damage)
    {       
        if(current_state != death_state)
        {
            abilities[0].UseAbility();
            stats.current_health = 0;
            GameUIManager.Instance.death_controller_ui.OnUnitDeath(this, false);
            MapGenerator.Instance.GetHex(column, row).walkable = true;
            health_bar.canvas_health_bar.SetActive(false);
            column = -100;
            row = -100;
            ChangeState(death_state);
            UnSubscibe();
            special_id = 0;

        }        
    }
}


