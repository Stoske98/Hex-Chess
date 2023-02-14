using Networking.Server;
using System;
using System.Collections.Generic;
using Units;

public enum CC_Type
{
    STUN = 1,
    DISARM = 2,
}
public class CC
{
    public Unit unit;

    public int id;
    public int max_cooldown;
    public int current_cooldown;
    public int cast_from_ability_id;
    public CC_Type type;
    public CC(Unit _unit, Data.CC cc)
    {
        unit = _unit;
        unit.cc.Add(this);
        id = cc.id;
        max_cooldown = cc.max_cooldown;
        current_cooldown = cc.current_cooldown;
        cast_from_ability_id = cc.cast_ability_id;
    }
    public CC(Unit _unit, Ability ability)
    {
        unit = _unit;
        unit.cc.Add(this);
        max_cooldown = ability.cc_time;
        current_cooldown = max_cooldown;
        cast_from_ability_id = ability.id;
    }

    public virtual bool ReduceCooldown()
    {
        if (current_cooldown != 0)
        {
            if (id != 0)
            {
                current_cooldown -= 1;
                if (current_cooldown == 0)
                {
                    Database.DestroyCC(this);
                    return true;
                }
                else
                    Database.UpdateCC(this);
            }
        }
        return false;
    }

}

public class Stun : CC
{
    public Stun(Unit unit, Data.CC cc_data) : base(unit, cc_data)
    {
        type = CC_Type.STUN;
    }
    public Stun(Unit unit, Ability ability) : base(unit, ability)
    {
        type = CC_Type.STUN;
    }

}
public class Disarm : CC
{
    public Disarm(Unit unit, Data.CC cc_data) : base(unit, cc_data)
    {
        type = CC_Type.DISARM;
    }
    public Disarm(Unit unit, Ability ability) : base(unit, ability)
    {
        type = CC_Type.DISARM;
    }

}

