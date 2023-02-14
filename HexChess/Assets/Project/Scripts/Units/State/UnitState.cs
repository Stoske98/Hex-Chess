using UnityEngine;

public abstract class UnitState
{
    public abstract void Enter(Unit unit);
    public abstract void Execute(Unit unit);
    public abstract void Exit(Unit unit);
}

public class IdleState : UnitState
{
    public override void Enter(Unit unit)
    {
    }

    public override void Execute(Unit unit)
    {
    }

    public override void Exit(Unit unit)
    {
    }
}
public class MoveState : UnitState
{
    public override void Enter(Unit unit)
    {
        Unit.OnStartMovement?.Invoke(unit);
        unit.animator.SetBool("Run",true);
    }

    public override void Execute(Unit unit)
    {
        unit.MoveUnit();
    }

    public override void Exit(Unit unit)
    {
        unit.animator.SetBool("Run", false);
        Unit.OnExitMovement?.Invoke(unit);
        GameUIManager.Instance.UpdateUnitUI(unit);
        Hex hex = MapGenerator.Instance.GetHex(unit.column, unit.row);
        if (hex != null && hex.isHexContainTrap())
        {
            Modified_Hex modified_hex = hex.GetTrapModifier();
            if (modified_hex != null && modified_hex.not_visible_for_the_class_type == unit.class_type)
            {
                modified_hex.OnDestroy(2);
                modified_hex.GameObject.SetActive(true);
                modified_hex.GameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Ability ability = GameManager.Instance.GetAbilityByID(modified_hex.ability_id);
                if (ability != null)
                {
                    Stun stun = new Stun(unit, ability);
                    stun.update_cc = true;
                    unit.ReceiveDamage(1);
                    hex.modified_hexes.Remove(modified_hex);
                }
            }
        }
    }
}
public class AttackState : UnitState
{
    public override void Enter(Unit unit)
    {
        unit.current_time = Time.time;
        unit.animator.SetBool("Attack", true);
    }

    public override void Execute(Unit unit)
    {
        unit.AttackUnit();
    }

    public override void Exit(Unit unit)
    {
        unit.animator.SetBool("Attack", false);
    }
}

public class AbilityState : UnitState
{
    public override void Enter(Unit unit)
    {
        unit.current_time = Time.time;

        if(unit.special_ability == unit.ability)
            unit.animator.Play("Special");
        else if(unit.abilities[0] == unit.ability)
            unit.animator.Play("Ability1");
        else if (unit.abilities[1] == unit.ability)
            unit.animator.Play("Ability2");
        else if (unit.abilities[2] == unit.ability)
            unit.animator.Play("Ability3");

    }

    public override void Execute(Unit unit)
    {
        unit.UseAbility();
    }

    public override void Exit(Unit unit)
    {
        unit.UpdateAbilityCooldown();
    }
}

public class DeathState : UnitState
{
    public override void Enter(Unit unit)
    {
        unit.animator.SetBool("Death", true);
    }

    public override void Execute(Unit unit)
    {
    }

    public override void Exit(Unit unit)
    {
        unit.animator.SetBool("Death", false);
    }
}