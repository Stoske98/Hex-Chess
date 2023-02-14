using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Networking.Client;
public enum AbilityType
{
    Passive = 0,
    Targetable = 1,
    Instant = 2,
}

public enum AbilityClass
{
    Earthshaker = 1,
    Fear = 2,
    Joust = 3,
    Warstrike = 4,
    Powershot = 5,
    Trap = 6,
    Trics_of_the_trade = 7,
    The_Fool = 8,
    Blessing = 9,
    Skyfall = 10,
    Fireball = 11,
    Necromancy = 12,
    Curse = 13,
    Vampirism = 14,
    Illu_Trics_of_the_trade = 15,
    Illu_The_Fool = 16,

}
public class Ability
{
    public int id;
    public string name { get; set; }
    public AbilityType ability_type { get; set; }

    public int max_cooldown;
    public int current_cooldown;
    public int quantity;
    public int cc_time;
    public int range;
    public Unit cast_unit;
    public GameObject prefab;
    public Sprite sprite;
    public Ability(Unit unit, Data.Ability ability_data)
    {
        id = ability_data.id;
        name = ability_data.name;
        ability_type = (AbilityType)ability_data.ability_type;
        max_cooldown = ability_data.max_cooldown;
        current_cooldown = ability_data.current_cooldown;
        quantity = ability_data.quantity;
        cc_time = ability_data.cc_time;
        range = ability_data.range;
        cast_unit = unit;
    }
    public virtual void UseAbility(Hex hex) {  }
    public virtual void UseAbility(Unit unit) { }
    public virtual void UseAbility() { }
    public virtual void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
    }
    public void ReduceCooldown()
    {
        if (current_cooldown != 0)
            current_cooldown -= 1;
    }
}

// TANK
public class Earthshaker : Ability
{
    public Earthshaker(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility()
    {
        Hex center_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
        Object.Instantiate(prefab, cast_unit.game_object.transform.position, Quaternion.identity);
        Unit unit = null;
        foreach (Hex h in MapGenerator.Instance.HexesInRange(center_hex, range))
        {
            unit = GameManager.Instance.GetUnit(h);
            if (unit != null && unit.class_type != cast_unit.class_type)
            {
                new Stun(unit,this);
                unit.ReceiveDamage(quantity);
            }
        }

        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        abilityMoves = MapGenerator.Instance.HexesInRange(hex, range);
    }
}
public class Fear : Ability
{
    public Fear(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility()
    {
        Hex center_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
        Object.Instantiate(prefab, cast_unit.game_object.transform.position, Quaternion.identity);
        Unit unit = null;
        foreach (Hex h in MapGenerator.Instance.HexesInRange(center_hex, range))
        {
            unit = GameManager.Instance.GetUnit(h);
            if (unit != null && unit.class_type != cast_unit.class_type)
            {
                new Disarm(unit, this);
                unit.ReceiveDamage(quantity);
                if (unit.current_state != unit.death_state)
                {
                    Vector2 opposite_direction = new Vector2(unit.column, unit.row) - new Vector2(cast_unit.column, cast_unit.row);
                    Hex hex = MapGenerator.Instance.GetHex(unit.column + (int)opposite_direction.x, unit.row + (int)opposite_direction.y);
                    if (hex != null && hex.walkable)
                        unit.SetPath(hex);
                }
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        abilityMoves = MapGenerator.Instance.HexesInRange(hex, range);
    }
}

// ARCHER
public class Powershot : Ability
{
    public Powershot(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility(Hex hex)
    {
        if (CheckHexOnDiagonal(0, -1, hex))
            return;
        if (CheckHexOnDiagonal(0, 1, hex))
            return;
        if (CheckHexOnDiagonal(1, -1, hex))
            return;
        if (CheckHexOnDiagonal(-1, 1, hex))
            return;
        if (CheckHexOnDiagonal(1, 0, hex))
            return;
        if (CheckHexOnDiagonal(-1, 0, hex))
            return;

        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes)
    {
        Hex h = null;
        Unit unit = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = MapGenerator.Instance.GetHex(i * column + hex.column, i * row + hex.row);
            if (h != null)
            {
                if (!h.walkable)
                {
                    unit = GameManager.Instance.GetUnit(h);
                    if(unit != null)
                    {
                        if (unit.class_type != cast_unit.class_type)
                        {
                            diagonal_hexes.Add(h);
                            break;
                        }
                        else
                            diagonal_hexes.Add(h);

                    }
                }
                else
                    diagonal_hexes.Add(h);
            }
        }
    }
    private bool CheckHexOnDiagonal(int column, int row, Hex hex)
    {
        bool isOnDiagonal = false;
        Hex h = null;
        Unit enemy = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
            if (h != null && hex == h)
            {
                isOnDiagonal = true;
                break;
            }
        }

        if(isOnDiagonal)
        {
            for (int i = 1; i < range + 1; i++)
            {
                h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    enemy = GameManager.Instance.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                    {
                        cast_unit.enemy_unit = enemy;
                        GameObject projectil = Object.Instantiate(prefab,cast_unit.game_object.transform.position + Vector3.up, Quaternion.identity);

                        MoveTowardGameObject move_towards = projectil.AddComponent<MoveTowardGameObject>();
                        move_towards.OnArriveDestination += OnArriveDestination;
                        move_towards.Initialization(enemy.game_object.transform.position + Vector3.up, 10);

                        cast_unit.ChangeState(cast_unit.idle_state);
                        break;
                    }
                }
            }
        }

        return isOnDiagonal;
    }

    private void OnArriveDestination(MoveTowardGameObject move_towards, Vector3 destination)
    {
        cast_unit.enemy_unit.ReceiveDamage(quantity);
        cast_unit.enemy_unit = null;
        move_towards.OnArriveDestination -= OnArriveDestination;
    }
}
public class Trap : Ability
{
    public Trap(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }
    public override void UseAbility(Hex hex)
    {
        ClassType not_visible_for = cast_unit.class_type == ClassType.Light ? ClassType.Dark : ClassType.Light;
        Modified_Hex modified_Hex = new Modified_Hex()
        {
            ability_id = id,
            type = Modifier.Trap,
            not_visible_for_the_class_type = not_visible_for,
        };

        hex.modified_hexes.Add(modified_Hex);
        modified_Hex.Spawn(hex,not_visible_for);

        cast_unit.ChangeState(cast_unit.idle_state);
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        foreach (var h in hexes)
        {
            if (!h.isHexContainTrap() && h.walkable)
                abilityMoves.Add(h);
        }
    }
}

// Knight
public class Joust : Ability
{
    public Joust(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Hex cast_unit_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
        if (cast_unit_hex != null)
        {
            cast_unit_hex.walkable = true;

            Unit enemy = GameManager.Instance.GetUnit(hex);
            if (enemy != null)
            {
                enemy.ReceiveDamage(quantity);
                if (enemy.current_state != enemy.death_state)
                {
                    enemy.path.Clear();

                    enemy.path.Add(hex);
                    enemy.path.Add(cast_unit_hex);

                    enemy.ChangeState(enemy.move_state);
                }
            }
            hex.walkable = true;
            cast_unit.SetPath(hex);
        }
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = MapGenerator.Instance.HexesInRange(hex, 2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }
        bool hexes_between_are_walkable = true;
        foreach (Hex h in hexesInRangeOf2)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
            {
                hexes_between_are_walkable = true;
                foreach (Hex hex_between in h.neighbors)
                {
                    if (hex.neighbors.Contains(hex_between) && !hex_between.walkable)
                    {
                        hexes_between_are_walkable = false;
                        break;
                    }
                }
                if (hexes_between_are_walkable)
                    abilityMoves.Add(h);
            }
        }
    }
}
public class Warstrike : Ability
{
    public Warstrike(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Unit enemy = GameManager.Instance.GetUnit(hex);
        Object.Instantiate(prefab, enemy.game_object.transform.position, Quaternion.identity);
        if (enemy != null && enemy.class_type != cast_unit.class_type)
        {
            new Stun(enemy, this);
            enemy.ReceiveDamage(quantity);
        }
        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = MapGenerator.Instance.HexesInRange(hex, 2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }
        foreach (Hex h in hexesInRangeOf2)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}

// Jester
public class TricsOfTheTrade : Ability
{
    public TricsOfTheTrade(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }
}
public class TheFool : Ability
{
    public TheFool(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }
}

// Wizzard
public class Blessing : Ability
{
    public Blessing(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Unit aliance_unit = GameManager.Instance.GetUnit(hex);
        Object.Instantiate(prefab, aliance_unit.game_object.transform.position, Quaternion.identity);

        if (aliance_unit.stats.current_health + quantity > aliance_unit.stats.max_health)
            aliance_unit.stats.current_health = aliance_unit.stats.max_health;
        else
            aliance_unit.stats.current_health += quantity;

        GameUIManager.Instance.UpdateUnitUI(aliance_unit);
        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        Unit aliance_unit = null;
        foreach (Hex h in MapGenerator.Instance.HexesInRange(hex, range))
        {
            aliance_unit = GameManager.Instance.GetUnit(h);
            if (aliance_unit != null && aliance_unit.class_type == cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Skyfall : Ability
{
    public Skyfall(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability2/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability2/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Unit enemy_unit = GameManager.Instance.GetUnit(hex);
        new Stun(enemy_unit, this);
        enemy_unit.ReceiveDamage(quantity);
        Object.Instantiate(prefab, enemy_unit.game_object.transform.position, Quaternion.identity);

        foreach (Hex h in hex.neighbors)
        {
            enemy_unit = GameManager.Instance.GetUnit(h);
            if (enemy_unit != null && enemy_unit.class_type != cast_unit.class_type)
                enemy_unit.ReceiveDamage(quantity);
        }
        cast_unit.ChangeState(cast_unit.idle_state);

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Fireball : Ability
{
    public Fireball(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability3/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability3/spell");
    }

    public override void UseAbility(Hex hex)
    {
        if (CheckHexOnDiagonal(0, -1, hex))
            return;
        if (CheckHexOnDiagonal(0, 1, hex))
            return;
        if (CheckHexOnDiagonal(1, -1, hex))
            return;
        if (CheckHexOnDiagonal(-1, 1, hex))
            return;
        if (CheckHexOnDiagonal(1, 0, hex))
            return;
        if (CheckHexOnDiagonal(-1, 0, hex))
            return;

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes)
    {
        Hex h = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = MapGenerator.Instance.GetHex(i * column + hex.column, i * row + hex.row);
            if (h != null)
                diagonal_hexes.Add(h);
        }
    }
    private bool CheckHexOnDiagonal(int column, int row, Hex hex)
    {
        bool isOnDiagonal = false;
        Hex h = null;
        Unit enemy = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
            if (h != null && hex == h)
            {
                isOnDiagonal = true;
                break;
            }
        }

        if (isOnDiagonal)
        {
            for (int i = 1; i < range + 1; i++)
            {
                h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    enemy = GameManager.Instance.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                    {
                        enemy.ReceiveDamage(quantity);
                        GameObject projectil = Object.Instantiate(prefab, cast_unit.game_object.transform.position + Vector3.up, Quaternion.identity);

                        MoveTowardGameObject move_towards = projectil.AddComponent<MoveTowardGameObject>();
                        move_towards.Initialization(cast_unit.game_object.transform.forward * 100 + Vector3.up, 20);
                    }
                }
            }
            cast_unit.ChangeState(cast_unit.idle_state);
        }

        return isOnDiagonal;
    }
}
public class Necromancy : Ability
{
    public Necromancy(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Unit enemy = GameManager.Instance.GetUnit(hex);
        if (enemy != null)
        {
            cast_unit.enemy_unit = enemy;
            GameObject projectil = Object.Instantiate(prefab);

            MoveTowardGameObject move_towards = projectil.AddComponent<MoveTowardGameObject>();
            move_towards.OnArriveDestination += OnArriveDestination;

            ParticleSystem[] particles = projectil.GetComponentsInChildren<ParticleSystem>();
            Color color;

            if (enemy.class_type == cast_unit.class_type)
            {
                color = Color.green;
                projectil.transform.position = cast_unit.game_object.transform.position + Vector3.up;
                move_towards.Initialization(enemy.game_object.transform.position + Vector3.up, 10);
            }
            else
            {
                color = Color.red;
                projectil.transform.position = enemy.game_object.transform.position + Vector3.up;
                move_towards.Initialization(cast_unit.game_object.transform.position + Vector3.up, 10);
            }

            foreach (ParticleSystem particle in particles)
                particle.startColor = color;

        }
        cast_unit.ChangeState(cast_unit.idle_state);
    }
    private void OnArriveDestination(MoveTowardGameObject move_towards, Vector3 destination)
    {
        if (cast_unit.enemy_unit.class_type == cast_unit.class_type)
        {
            cast_unit.ReceiveDamage(quantity);
            if (cast_unit.enemy_unit.stats.current_health + quantity > cast_unit.enemy_unit.stats.max_health)
                cast_unit.enemy_unit.stats.current_health = cast_unit.enemy_unit.stats.max_health;
            else
                cast_unit.enemy_unit.stats.current_health += quantity;

            GameUIManager.Instance.UpdateUnitUI(cast_unit.enemy_unit);
        }
        else
        {
            cast_unit.enemy_unit.ReceiveDamage(quantity);
            if (cast_unit.stats.current_health + quantity > cast_unit.stats.max_health)
                cast_unit.stats.current_health = cast_unit.stats.max_health;
            else
                cast_unit.stats.current_health += quantity;

            GameUIManager.Instance.UpdateUnitUI(cast_unit);
        }

        cast_unit.enemy_unit = null;
        move_towards.OnArriveDestination -= OnArriveDestination;
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null)
                abilityMoves.Add(h);
        }
    }
}
public class Curse : Ability
{
    public Curse(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability2/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability2/spell");
    }

    public override void UseAbility(Hex hex)
    {
        Unit enemy_unit = GameManager.Instance.GetUnit(hex);
        new Disarm(enemy_unit, this);
        enemy_unit.ReceiveDamage(quantity);
        Object.Instantiate(prefab, enemy_unit.game_object.transform.position, Quaternion.identity);

        foreach (Hex h in hex.neighbors)
        {
            enemy_unit = GameManager.Instance.GetUnit(h);
            if (enemy_unit != null && enemy_unit.class_type != cast_unit.class_type)
            {
                enemy_unit.ReceiveDamage(quantity);
                if (enemy_unit.current_state != enemy_unit.death_state)
                {
                    Vector2 opposite_direction = new Vector2(enemy_unit.column, enemy_unit.row) - new Vector2(hex.column, hex.row);
                    Hex hex_direction = MapGenerator.Instance.GetHex(enemy_unit.column + (int)opposite_direction.x, enemy_unit.row + (int)opposite_direction.y);
                    if (hex_direction != null && hex_direction.walkable)
                        enemy_unit.SetPath(hex_direction);
                }
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Vampirism : Ability
{
    public Vampirism(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability3/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability3/spell");
    }

    public override void UseAbility(Hex hex)
    {
        List<Hex> longestEnemyPath = PathFinding.PathFinder.BFS_LongestPath(hex, cast_unit);
        Transform trail = Object.Instantiate(prefab, cast_unit.game_object.transform.position + Vector3.up / 2, Quaternion.identity).transform.GetChild(0);


        for (int i = 0; i < longestEnemyPath.Count; i++)
        {
            if(i == 0)
                SetEffectPath(trail.transform.GetChild(i).gameObject, cast_unit, GameManager.Instance.GetUnit(longestEnemyPath[i]));
            else if(i < 5)
                SetEffectPath(trail.transform.GetChild(i).gameObject, GameManager.Instance.GetUnit(longestEnemyPath[i - 1]), GameManager.Instance.GetUnit(longestEnemyPath[i]));
        }

        Unit enemy = null;
        for (int i = 0; i < 5; i++)
        {
            if (i < longestEnemyPath.Count)
            {
                enemy = GameManager.Instance.GetUnit(longestEnemyPath[i]);
                if (enemy != null && enemy.current_state != enemy.death_state)
                    enemy.ReceiveDamage(quantity);

                if (cast_unit.stats.current_health + quantity > cast_unit.stats.max_health)
                    cast_unit.stats.current_health = cast_unit.stats.max_health;
                else
                    cast_unit.stats.current_health += quantity;
            }
        }
        GameUIManager.Instance.UpdateUnitUI(cast_unit);
        cast_unit.ChangeState(cast_unit.idle_state);
    }

    private void SetEffectPath(GameObject laser_go, Unit from, Unit to)
    {
        Vector3 from_position = from.game_object.transform.position + Vector3.up;
        Vector3 to_position = to.game_object.transform.position + Vector3.up;

        LineRenderer lineRenderer = laser_go.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from_position);
        lineRenderer.SetPosition(1, to_position);


    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit enemy = GameManager.Instance.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}

//illusion
public class TricsOfTheTradeIllusion : Ability
{
    public TricsOfTheTradeIllusion(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }
}
public class TheFoolIllusion : Ability
{
    public TheFoolIllusion(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/prefab");
        sprite = Resources.Load<Sprite>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/Ability1/spell");
    }
    public override void UseAbility()
    {
        Hex unit_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
        Object.Instantiate(prefab, cast_unit.game_object.transform.position, Quaternion.identity);
        Unit enemy_unit = null;
        foreach (var neighbor in unit_hex.neighbors)
        {
            enemy_unit = GameManager.Instance.GetUnit(neighbor);
            if (enemy_unit != null && enemy_unit.class_type != cast_unit.class_type)
                enemy_unit.ReceiveDamage(quantity);
        }
    }
}



public class Swordsman_SA : Ability
{
    public Swordsman_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Unit enemy)
    {
        if(Swordsman.swordsman_passive.Count != 0)
        {
            Vector2Int swordsmans = Swordsman.swordsman_passive.Dequeue();
            Unit aliance_unit = null;
            Unit enemy_unit = null;

            foreach (var unit in GameManager.Instance.units)
            {
                if (unit.special_id == swordsmans.x)
                    aliance_unit = unit;
                else if (unit.special_id == swordsmans.y)
                    enemy_unit = unit;
            }
            if (aliance_unit != null && enemy_unit != null && aliance_unit.class_type != enemy_unit.class_type)
                aliance_unit.SetAttack(enemy_unit);
        }
    }
}
public class Tank_SA : Ability
{
    public Tank_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
    public override void UseAbility(Hex hex)
    {
        Unit enemy_unit = GameManager.Instance.GetUnit(hex);

        Vector2 opposite_direction = new Vector2(enemy_unit.column, enemy_unit.row) - new Vector2(cast_unit.column, cast_unit.row);
        Hex desired_hex = MapGenerator.Instance.GetHex(enemy_unit.column + (int)opposite_direction.x, enemy_unit.row + (int)opposite_direction.y);
        if (desired_hex != null && desired_hex.walkable)
        {
            enemy_unit.SetPath(desired_hex);
            new Stun(enemy_unit, this);

            hex.walkable = true;
            cast_unit.SetPath(hex);
        }
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        foreach (Hex neighbor in hex.neighbors)
        {
            Unit enemy_unit = GameManager.Instance.GetUnit(neighbor);
            if (enemy_unit != null && cast_unit.class_type != enemy_unit.class_type)
            {
                Vector2 opposite_direction = new Vector2(enemy_unit.column, enemy_unit.row) - new Vector2(cast_unit.column, cast_unit.row);
                Hex desired_hex = MapGenerator.Instance.GetHex(enemy_unit.column + (int)opposite_direction.x, enemy_unit.row + (int)opposite_direction.y);
                if(desired_hex != null && desired_hex.walkable)
                    abilityMoves.Add(neighbor);
            }
        }
    }
}
public class Knight_SA : Ability
{
    public Knight_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex)
    {
        Hex cast_unit_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
        List<Unit> enemy_units = new List<Unit>();

        foreach (Hex hex_neighbor in hex.neighbors)
        {
            if(cast_unit_hex.neighbors.Contains(hex_neighbor) && !hex_neighbor.walkable)
            {
                Unit enemy = GameManager.Instance.GetUnit(hex_neighbor);
                if (enemy != null && enemy.class_type != cast_unit.class_type)
                    enemy_units.Add(enemy);
            }
        }

        foreach (Unit enemy in enemy_units)
            enemy.ReceiveDamage(quantity);

        cast_unit.SetPath(hex);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = MapGenerator.Instance.HexesInRange(hex, 2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }

        foreach (Hex h in hexesInRangeOf2)
        {
            if(h.walkable)
            {
                foreach (Hex hex_neighbor in h.neighbors)
                {
                    if (hex.neighbors.Contains(hex_neighbor) && !hex_neighbor.walkable)
                    {
                        Unit enemy = GameManager.Instance.GetUnit(hex_neighbor);
                        if (enemy != null && enemy.class_type != cast_unit.class_type)
                            abilityMoves.Add(h);
                    }
                }
            }
        }
    }
}
public class Archer_SA : Ability
{
    public Archer_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
}
public class Jester_SA : Ability
{
    public Jester_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
    public override void UseAbility(Hex hex)
    {
        cast_unit.SetPath(hex);
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        List<Hex> hexes = MapGenerator.Instance.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            if (h.walkable)
                abilityMoves.Add(h);
        }
    }

}
public class Wizzard_SA : Ability
{
    public Wizzard_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex)
    {
        if(Time.time >= cast_unit.current_time + 1.5f)
        {
            MapGenerator.Instance.GetHex(cast_unit.column,cast_unit.row).walkable = true;
            cast_unit.game_object.transform.position = hex.game_object.transform.position;
            
            cast_unit.column = hex.column;
            cast_unit.row = hex.row;
            hex.walkable = false;

            // Unit.OnExitMovement?.Invoke(cast_unit);
            cast_unit.path.Clear();
            cast_unit.ChangeState(cast_unit.move_state);
        }
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> specialMoves)
    {
        foreach (Hex h in MapGenerator.Instance.HexesInRange(hex, 2))
            if (h.walkable)
                specialMoves.Add(h);
    }
}
public class Queen_SA : Ability
{
    bool desired_hex_is_found = false;
    public Queen_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
    public override void UseAbility(Hex hex)
    {
        WalkAndAttack(hex);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        desired_hex_is_found = false;
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes)
    {
        Hex h = MapGenerator.Instance.GetHex(column + hex.column, row + hex.row);
        bool hex_is_found = false;

        if (h!= null && h.walkable)
        {
            for (int i = 1; i < range + 1; i++)
            {
                h = MapGenerator.Instance.GetHex(i * column + hex.column, i * row + hex.row);
                if (h != null)
                {
                    if (!h.walkable)
                    {
                        Unit unit = GameManager.Instance.GetUnit(h);
                        if (unit != null)
                        {
                            if (unit.class_type != cast_unit.class_type)
                            {
                                hex_is_found = true;
                                break;
                            }
                            else break;

                        }
                    }
                }
            }

            if (hex_is_found)
            {
                for (int i = 1; i < range + 1; i++)
                {
                    h = MapGenerator.Instance.GetHex(i * column + hex.column, i * row + hex.row);
                    if (h != null)
                    {
                        if (!h.walkable)
                        {
                            Unit unit = GameManager.Instance.GetUnit(h);
                            if (unit != null)
                            {
                                if (unit.class_type != cast_unit.class_type)
                                {
                                    diagonal_hexes.Add(h);
                                    break;
                                }
                                else break;

                            }
                        }
                      /*  else
                            diagonal_hexes.Add(h);*/
                    }
                }
            }
        }
    }
    private void WalkAndAttack(Hex hex)
    {
        if (!desired_hex_is_found)
        {
            if (CheckHexOnDiagonal(0, 1, hex))
                return;
            if (CheckHexOnDiagonal(0, -1, hex))
                return;
            if (CheckHexOnDiagonal(1, -1, hex))
                return;
            if (CheckHexOnDiagonal(-1, 1, hex))
                return;
            if (CheckHexOnDiagonal(1, 0, hex))
                return;
            if (CheckHexOnDiagonal(-1, 0, hex))
                return;
            cast_unit.ChangeState(cast_unit.idle_state);
        }
        else
        {
            if (cast_unit.path.Count != 0)
            {
                if ((cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position).magnitude > 0.1f)
                {
                    cast_unit.game_object.transform.position += (cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position).normalized * Unit.MOVEMENTSPEED * cast_unit.delta_time;

                    cast_unit.target_rotation = Quaternion.LookRotation(cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position, Vector3.up);
                    cast_unit.game_object.transform.rotation = Quaternion.Slerp(cast_unit.game_object.transform.rotation, cast_unit.target_rotation, cast_unit.delta_time * Unit.ROTATESPEED);
                }
                else
                {
                    if (cast_unit.path.Count == 1)
                    {
                        cast_unit.column = cast_unit.path[0].column;
                        cast_unit.row = cast_unit.path[0].row;
                        cast_unit.path[0].walkable = false;
                    }
                    cast_unit.path.RemoveAt(0);
                }
            }
            else
            {
                cast_unit.SetAttack(cast_unit.enemy_unit);
                desired_hex_is_found = false;
            }               
        }
      
    }

    private bool CheckHexOnDiagonal(int column, int row, Hex hex)
    {
        bool isOnDiagonal = false;
        for (int i = 1; i < range + 1; i++)
        {
            Hex h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
            if (h != null && hex == h)
            {
                isOnDiagonal = true;
                break;
            }
        }

        if (isOnDiagonal)
        {
            for (int i = 1; i < range + 1; i++)
            {
                Hex h = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    Debug.Log(column + "," + row);
                    Unit enemy = GameManager.Instance.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                    {
                        Debug.Log(enemy.class_type.ToString() + enemy.unit_type.ToString());
                        Hex from_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);
                        i -= 1;
                        Hex to_hex = MapGenerator.Instance.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);

                        cast_unit.path.Clear();
                        cast_unit.path.Add(from_hex);
                        cast_unit.path.Add(to_hex);

                        from_hex.walkable = true;

                        desired_hex_is_found = true;
                        cast_unit.enemy_unit = enemy;
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
public class King_SA : Ability
{
    bool desired_hex_is_found = false;
    public King_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
        prefab = Resources.Load<GameObject>(unit.class_type.ToString() + "/" + unit.unit_type.ToString() + "/SpecialAbility/prefab");
    }
    public override void UseAbility(Hex hex)
    {
        WalkToTheMiddle(hex);
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves)
    {
        abilityMoves.Clear();
        if(cast_unit.stats.damage == 0)
        {
            desired_hex_is_found = false;
            Hex middle_hex = MapGenerator.Instance.GetHex(0, 0);
            foreach (var neighbor_hex in hex.neighbors)
            {
                if (neighbor_hex == middle_hex && neighbor_hex.walkable)
                    abilityMoves.Add(middle_hex);
            }
        }
    }
    private void WalkToTheMiddle(Hex hex)
    {
        if(!desired_hex_is_found)
        {
            Hex from_hex = MapGenerator.Instance.GetHex(cast_unit.column, cast_unit.row);

            cast_unit.path.Clear();
            cast_unit.path.Add(from_hex);
            cast_unit.path.Add(hex);

            Object.Instantiate(prefab, hex.game_object.transform.position, Quaternion.identity);
            from_hex.walkable = true;
            desired_hex_is_found = true;
        }
        else
        {
            if (cast_unit.path.Count != 0)
            {
                if ((cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position).magnitude > 0.1f)
                {
                    cast_unit.game_object.transform.position += (cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position).normalized * Unit.MOVEMENTSPEED * cast_unit.delta_time;

                    cast_unit.target_rotation = Quaternion.LookRotation(cast_unit.path[0].game_object.transform.position - cast_unit.game_object.transform.position, Vector3.up);
                    cast_unit.game_object.transform.rotation = Quaternion.Slerp(cast_unit.game_object.transform.rotation, cast_unit.target_rotation, cast_unit.delta_time * Unit.ROTATESPEED);
                }
                else
                {
                    if (cast_unit.path.Count == 1)
                    {
                        cast_unit.column = cast_unit.path[0].column;
                        cast_unit.row = cast_unit.path[0].row;
                        cast_unit.path[0].walkable = false;
                    }
                    cast_unit.path.RemoveAt(0);
                }
            }
            else
            {
               
                if(!GameManager.Instance.challenge_royal_activated)
                {
                    GameUIManager.Instance.ActivateChallengeRoyale();
                    GameUIManager.Instance.PlayChallengeRoyaleAnimation(cast_unit.class_type);
                }

                UpdateKingStats();

                Object.Destroy(cast_unit.health_bar.canvas_health_bar);
                cast_unit.health_bar = new HealthBar(GameUIManager.Instance.health_bar_prefab);
                cast_unit.health_bar.Initialize(cast_unit);

                cast_unit.ChangeState(cast_unit.idle_state);
                Unit.OnExitMovement?.Invoke(cast_unit);
            }
        }   
    }

    private void UpdateKingStats()
    {
        int death_units = 0;
        switch (cast_unit.class_type)
        {
            case ClassType.Light:
                death_units = GameUIManager.Instance.death_controller_ui.light_dead_units.Count;
                break;
            case ClassType.Dark:
                death_units = GameUIManager.Instance.death_controller_ui.dark_dead_units.Count;
                break;
            default:
                break;
        }

        cast_unit.stats.current_health = cast_unit.stats.max_health;
        cast_unit.stats.attack_range = 1;

        if (death_units > 9)
            cast_unit.stats.damage = 3;
        else if (death_units > 5)
            cast_unit.stats.damage = 2;
        else if (death_units > 2)
            cast_unit.stats.damage = 1;

        Debug.Log("UPDATED KING: DAMAGE: " + cast_unit.stats.damage + " DEATH UNITS: " + death_units);
    }
}
public class Jester_Illusion_SA : Ability
{
    public Jester_Illusion_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
}


