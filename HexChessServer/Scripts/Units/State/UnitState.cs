using Networking.Server;
using System;
using Units;
public abstract class UnitState
{
    public abstract void Enter(Unit unit, Game game_data);
    public abstract void Execute(Unit unit, Game game_data);
    public abstract void Exit(Unit unit, Game game_data);
}

public class IdleState : UnitState
{
    public override void Enter(Unit unit, Game game_data)
    {
    }

    public override void Execute(Unit unit, Game game_data)
    {
    }

    public override void Exit(Unit unit, Game game_data)
    {
    }
}
public class MoveState : UnitState
{
    public override void Enter(Unit unit, Game game_data)
    {
        game_data.events.OnStartMovement?.Invoke(unit, game_data);
    }

    public override void Execute(Unit unit, Game game_data)
    {
        unit.MoveUnit(game_data);
    }

    public override void Exit(Unit unit, Game game_data)
    {
        game_data.events.OnExitMovement?.Invoke(unit, game_data);

        Map.Hex hex = game_data.map.GetHex(unit.column, unit.row);
        if(hex != null && hex.isHexContainTrap())
        {
            Map.Modified_Hex modified_hex = hex.GetTrapModifier();
            if(modified_hex != null && modified_hex.not_visible_for_the_class_type == unit.class_type)
            {
                Ability ability = game_data.GetAbilityByID(modified_hex.ability_id);
                if(ability != null)
                {
                    Stun stun = new Stun(unit, ability);
                    Database.CreateCC(game_data, stun);
                    unit.RecieveDamage(1, game_data);
                    Database.DestroyModifiedHex(modified_hex);
                }
            }
        }
    }
}
public class AttackState : UnitState
{
    public override void Enter(Unit unit, Game game_data)
    {
    }

    public override void Execute(Unit unit, Game game_data)
    {
        unit.AttackUnit(game_data);
    }

    public override void Exit(Unit unit, Game game_data)
    {

    }
}

public class AbilityState : UnitState
{
    public override void Enter(Unit unit, Game game_data)
    {
    }

    public override void Execute(Unit unit, Game game_data)
    {
        unit.UseAbility(game_data);
    }

    public override void Exit(Unit unit, Game game_data)
    {
        unit.UpdateAbilityCooldown();
        Console.WriteLine("On exit ability state");
    }
}
public class DeathState : UnitState
{
    public override void Enter(Unit unit, Game game)
    {
    }

    public override void Execute(Unit unit, Game game)
    {
        //unit.AttackUnit();
    }

    public override void Exit(Unit unit, Game game)
    {
    }
}

