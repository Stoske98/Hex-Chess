using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
using UnityEngine.UI;
public class Unit
{
    public const float MOVEMENTSPEED = 3F;
    public const float ROTATESPEED = 7.5F;
    public int special_id;
    public int column { get; set; }
    public int row { get; set; }
    public GameObject game_object { get; set; }

    public UnitStats stats;

    public float delta_time { get; set; }
    public float current_time { get; set; }
    public UnitType unit_type { get; set; }
    public ClassType class_type { get; set; }

    public Animator animator { get; set; }

    public List<Hex> path;

    public Quaternion target_rotation;

    public Unit enemy_unit { get; set; }
    public Ability ability;
    protected Hex ability_hex;

    public UnitState current_state;
    public readonly IdleState idle_state = new IdleState();
    public readonly MoveState move_state = new MoveState();
    public readonly AttackState attack_state = new AttackState();
    public readonly AbilityState ability_state = new AbilityState();
    public readonly DeathState death_state = new DeathState();

    public Ability[] abilities;
    public Ability special_ability;
    public List<CC> cc;

    public HealthBar health_bar;
    public Sprite sprite;
    public Unit(Hex hex, UnitType unitType, ClassType classType)
    {
        column = hex.column;
        row = hex.row;
        unit_type = unitType;
        class_type = classType;
        current_state = idle_state;
        hex.walkable = false;


        game_object = Object.Instantiate(Resources.Load<GameObject>(class_type.ToString() + "/" + unit_type.ToString() + "/prefab"),
            hex.game_object.transform.position,
            Quaternion.Euler(0, 0, 0));
        game_object.name = unit_type.ToString() + class_type.ToString();
    }

    public Unit(Data.Unit data)
    {
        special_id = data.id;
        column = data.hex_column;
        row = data.hex_row;
        unit_type = (UnitType)data.unit_type;
        class_type = (ClassType)data.class_type;
        path = new List<Hex>();
        cc = new List<CC>();
        stats = new UnitStats
        {
            max_health = data.max_health,
            current_health = data.current_health,

            damage = data.damage,
            attack_range = data.attack_range,
            attack_speed = data.attack_speed
        };

        if (stats.current_health <= 0)
        {
            current_state = death_state;

            game_object = Object.Instantiate(Resources.Load<GameObject>(class_type.ToString() + "/" + unit_type.ToString() + "/prefab"),
           new Vector3(-999, -999, -999),
           Quaternion.Euler(0, 0, 0));
            column = -999;
            row = -999;
        }
        else
        {

            current_state = idle_state;

            game_object = Object.Instantiate(Resources.Load<GameObject>(class_type.ToString() + "/" + unit_type.ToString() + "/prefab"),
            GameManager.Instance.map.GetHex(column, row).game_object.transform.position,
            Quaternion.Euler(0, 0, 0));
        }

        game_object.transform.Rotate(Vector2.up, data.rotation,Space.Self);
        game_object.name = unit_type.ToString() + class_type.ToString();
        game_object.transform.SetParent(GameManager.Instance.units_container);

        abilities = new Ability[data.abilities.Count];

        for (int i = 0; i < data.abilities.Count; i++)
            abilities[i] = Spawner.CreateAbility(this, data.abilities[i]);

        special_ability = Spawner.CreateSpecialAbility(this, data.special_ability);

        foreach (var cc_data in data.cc)
            cc.Add(Spawner.CreateCC(this, cc_data));

        if (current_state == death_state)
            game_object.SetActive(false);

        health_bar = new HealthBar(GameUIManager.Instance.health_bar_prefab);
        health_bar.Initialize(this);

        sprite = Resources.Load<Sprite>(class_type.ToString() + "/" + unit_type.ToString() + "/image");

        animator = game_object.GetComponent<Animator>();

    }

    public void Update(float deltaTime)
    {
        delta_time = deltaTime;
        current_state.Execute(this);
    }

    public void ChangeState(UnitState newState)
    {
        current_state.Exit(this);
        current_state = newState;
        current_state.Enter(this);
    }
    public virtual void SetAttack(Unit enemyUnit)
    {
        enemy_unit = enemyUnit;
        game_object.transform.rotation = Quaternion.LookRotation(enemyUnit.game_object.transform.position - game_object.transform.position, Vector3.up);
        ChangeState(attack_state);
    }
    public virtual void AttackUnit()
    {
        if(Time.time > current_time + stats.attack_speed)
        {
            int enemyColumn = enemy_unit.column;
            int enemyRow = enemy_unit.row;
            if (enemy_unit.current_state != enemy_unit.death_state)
            {
                enemy_unit.ReceiveDamage(stats.damage);
                if (enemy_unit.current_state == enemy_unit.death_state)
                {
                    if (current_state != death_state)
                        SetPath(MapGenerator.Instance.GetHex(enemyColumn, enemyRow));
                    else
                        ChangeState(death_state);
                }
                else
                    ChangeState(idle_state);               
            }else
                ChangeState(idle_state);
            enemy_unit = null;
           
        }
    }
    public virtual void MoveUnit()
    {
        if (path.Count != 0)
        {
            if ((path[0].game_object.transform.position - game_object.transform.position).magnitude > 0.1f)
            {
                game_object.transform.position += (path[0].game_object.transform.position - game_object.transform.position).normalized * MOVEMENTSPEED * delta_time;

                target_rotation = Quaternion.LookRotation(path[0].game_object.transform.position - game_object.transform.position, Vector3.up);
                game_object.transform.rotation = Quaternion.Slerp(game_object.transform.rotation, target_rotation, delta_time * ROTATESPEED);
            }
            else
            {
                if(path.Count == 1)
                {
                    column = path[0].column;
                    row = path[0].row;
                    path[0].walkable = false;
                }
                path.RemoveAt(0);
            }
        }
        else
            ChangeState(idle_state);
    }

    public virtual void SetPath(Hex hex)
    {
        Hex currentHex = MapGenerator.Instance.GetHex(column, row);
        path = PathFinding.PathFinder.FindPath_AStar(currentHex, hex);
        CheckIfTrapIsOnPath();
        currentHex.walkable = true;
        ChangeState(move_state);
    }

    public void CheckIfTrapIsOnPath()
    {
        int index = -1;
        ClassType class_type_that_activate_trap = ClassType.None;
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i].isHexContainTrap())
            {
                class_type_that_activate_trap = path[i].ClassThatActivateTrap();
                index = i;
                break;
            }
        }
        if (index != -1 && class_type_that_activate_trap != ClassType.None && class_type_that_activate_trap == class_type)
        {
            int path_range = path.Count - 1;
            while (index != path_range)
            {
                path.RemoveAt(path_range);
                path_range--;
            }
        }
    }
    public virtual void UseAbility()
    {
        switch (ability.ability_type)
        {
            case AbilityType.Passive:
                break;
            case AbilityType.Targetable:
                ability.UseAbility(ability_hex);
                break;
            case AbilityType.Instant:
                ability.UseAbility(); 
                break;
            default:
                break;
        }
    }
    public void UpdateAbilityCooldown()
    {
        ability.current_cooldown = ability.max_cooldown;
        GameUIManager.Instance.UpdateUnitUI(this);
    }
    public virtual void SetAbility(AbilityInput input, Hex to_hex)
    {
        ability = null;
        if (input == AbilityInput.S)
            ability = special_ability;
        else
            ability = abilities[(int)input];

        ability_hex = to_hex;
        if(ability == null || (ability.ability_type == AbilityType.Targetable && ability_hex == null))
            return;

        if (ability.ability_type == AbilityType.Targetable)
            game_object.transform.rotation = Quaternion.LookRotation(to_hex.game_object.transform.position - game_object.transform.position, Vector3.up);

        ChangeState(ability_state);
    }
    public virtual void ReceiveDamage(int damage)
    {
        OnRecieveDamage?.Invoke(this);
        if (stats.current_health - damage > 0)
            stats.current_health -= damage;
        else
        {
            stats.current_health = 0;
            GameUIManager.Instance.death_controller_ui.OnUnitDeath(this,false);
            MapGenerator.Instance.GetHex(column, row).walkable = true;
            health_bar.canvas_health_bar.SetActive(false);
            column = -999;
            row = -999;
            current_state = death_state;
            ChangeState(death_state);
            UnSubscibe();
            if (unit_type == UnitType.King)
            {
                NetEndGame request = new NetEndGame();
                Sender.TCP_SendToServer(request);
            }
        }

        GameUIManager.Instance.UpdateUnitUI(this);
           
    }


    public virtual void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves)
    {
         availableMoves.Clear();
         foreach (Hex h in hex.neighbors)
             if (h.walkable)
                availableMoves.Add(h);
    }
    public virtual void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves)
    {
        specialMoves.Clear();
    }
    public virtual void GetAttackMoves(Hex hex, ref List<Hex> attackMoves)
    {
        attackMoves.Clear();
        foreach (Hex h in MapGenerator.Instance.HexesInRange(hex,stats.attack_range))
        {
            Unit unit = GameManager.Instance.GetUnit(h);
            if (unit != null && unit.class_type != class_type)
                attackMoves.Add(h);
        }
    }

    protected List<Hex> GetDiagonalsAvailableMove(Hex center, int range, ref List<Hex> diagonal_hexes, bool countUnWalkableFields = false)
    {
        //center -> bottom
        AddToDiagonalHexes(0, -1, center, range, ref diagonal_hexes, countUnWalkableFields);
        // center -> top
        AddToDiagonalHexes(0, +1, center, range, ref diagonal_hexes, countUnWalkableFields);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, center, range, ref diagonal_hexes, countUnWalkableFields);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, center, range, ref diagonal_hexes, countUnWalkableFields);

        //center -> top left
        AddToDiagonalHexes(-1, 0, center, range, ref diagonal_hexes, countUnWalkableFields);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, center, range, ref diagonal_hexes, countUnWalkableFields);

        return diagonal_hexes;
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, int range, ref List<Hex> diagonal_hexes, bool countUnWalkableFields)
    {
        for (int i = 1; i < range + 1; i++)
        {
            Hex h = MapGenerator.Instance.GetHex(i * column + hex.column, i * row + hex.row);
            if (h != null)
            {
                if (countUnWalkableFields)
                    diagonal_hexes.Add(h);
                else
                {
                    if (h.walkable)
                        diagonal_hexes.Add(h);
                    else
                        break;
                }
            }
        }
    }
    public bool IsDisarmed()
    {
        for (int i = cc.Count - 1; i >= 0; i--)
        {
            if (cc[i].type == CC_Type.DISARM)
            {
                if (cc[i].current_cooldown == 0)
                    cc.RemoveAt(i);
                else
                    return true;

            }
        }
        return false;
    }
    public bool IsStunned()
    {
        for (int i = cc.Count - 1; i >= 0; i--)
        {
            if (cc[i].type == CC_Type.STUN)
            {
                if (cc[i].current_cooldown == 0)
                    cc.RemoveAt(i);
                else
                    return true;

            }
        }
        return false;
    }
    public virtual void UnSubscibe()
    {

    }
    #region Events
    public static System.Action<Unit> OnExitMovement;
    public static System.Action<Unit> OnStartMovement;
    public static System.Action<Unit> OnRecieveDamage;
    #endregion
}

public class Melee : Unit
{
    public Melee(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }

    public Melee(Data.Unit unit) : base(unit)
    {
    }
}

public class Range : Unit
{
    public GameObject projectil_prefab;
    public Range(Hex hex, UnitType unitType, ClassType classType) : base(hex, unitType, classType)
    {
    }

    public Range(Data.Unit unit) : base(unit)
    {
        projectil_prefab = Resources.Load<GameObject>(class_type.ToString() + "/" + unit_type.ToString() + "/Projectil/prefab");
    }

    public override void AttackUnit()
    {
        if (Time.time > current_time + stats.attack_speed)
        {
            GameObject projectil = Object.Instantiate(projectil_prefab);
            
            Missile missile = projectil.AddComponent<Missile>();
            missile.Initialization(this, enemy_unit, 2, 3);

            ChangeState(idle_state);

        }
    }
}

public class UnitStats
{
    public int max_health;
    public int current_health;

    public int attack_range;
    public float attack_speed;

    public int damage;

}
public enum ClassType
{
    None = 0,
    Light = 1,
    Dark = 2
}

public enum UnitType
{
    None = 0,
    Swordsman = 1,
    Tank = 2,
    Knight = 3,
    Archer = 4,
    Jester = 5,
    Wizzard = 6,
    Queen = 7,
    King = 8,
    JesterIllusion = 9,
}


