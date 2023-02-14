using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;

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
    public bool update_cc = true;

    public GameObject vfx;
    public CC(Unit _unit, Data.CC cc)
    {
        unit = _unit;
        unit.cc.Add(this);
        id = cc.id;
        max_cooldown = cc.max_cooldown;
        current_cooldown = cc.current_cooldown;
        cast_from_ability_id = cc.cast_ability_id;
        update_cc = true;
    }
    public CC(Unit _unit, Ability ability)
    {
        unit = _unit;
        unit.cc.Add(this);
        max_cooldown = ability.cc_time;
        current_cooldown = max_cooldown;
        cast_from_ability_id = ability.id;
        update_cc = false;

    }
    public virtual bool ReduceCooldown()
    {
        if (current_cooldown != 0)
        {        
            if (update_cc)
            {
                current_cooldown -= 1;
                if (current_cooldown == 0)
                {
                    Object.Destroy(vfx);
                    return true;
                }
            }
            else
                update_cc = true;
        }
        return false;
    }
}

public class Stun : CC
{
    public Stun(Unit unit, Data.CC cc_data) : base(unit, cc_data)
    {
        type = CC_Type.STUN;
        vfx = Object.Instantiate(Resources.Load<GameObject>("CC/Stun/prefab"), unit.game_object.transform.position, Quaternion.identity);
        vfx.transform.SetParent(unit.game_object.transform);
    }
    public Stun(Unit unit, Ability ability) : base(unit, ability)
    {
        type = CC_Type.STUN;
        vfx = Object.Instantiate(Resources.Load<GameObject>("CC/Stun/prefab"), unit.game_object.transform.position, Quaternion.identity);
        vfx.transform.SetParent(unit.game_object.transform);
    }
}
public class Disarm : CC
{
    public Disarm(Unit unit, Data.CC cc_data) : base(unit, cc_data)
    {
        type = CC_Type.DISARM;
        vfx = Object.Instantiate(Resources.Load<GameObject>("CC/Disarm/prefab"), unit.game_object.transform.position, Quaternion.identity);
        vfx.transform.SetParent(unit.game_object.transform);
    }
    public Disarm(Unit unit, Ability ability) : base(unit, ability)
    {
        type = CC_Type.DISARM;
        vfx = Object.Instantiate(Resources.Load<GameObject>("CC/Disarm/prefab"), unit.game_object.transform.position, Quaternion.identity);
        vfx.transform.SetParent(unit.game_object.transform);
    }
}




