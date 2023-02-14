using Networking.Server;
using System;
using System.Collections.Generic;
using System.Text;
using Units;
using Map;
using System.Numerics;

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
    public Data.Ability AbilityToData()
    {
        Data.Ability data_ability = new Data.Ability();
        data_ability.id = id;
        data_ability.name = name;
        data_ability.ability_type = (int)ability_type;
        data_ability.max_cooldown = max_cooldown;
        data_ability.current_cooldown = current_cooldown;
        data_ability.quantity = quantity;
        data_ability.cc_time = cc_time;
        data_ability.range = range;
        return data_ability;
    }
    public virtual void UseAbility(Hex hex, Game game) {  }
    public virtual void UseAbility(Unit unit, Game game) { }
    public virtual void UseAbility(Game game) { }
    public virtual void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        List<Hex> hexes = new List<Hex>();
        List<Hex> allHexes = game.map.HexesInRange(hex, range);

        foreach (Hex h in allHexes)
        {
            if (game.GetUnit(h) != null)
                hexes.Add(h);
        }
    }

    public void ReduceCooldown()
    {
        if (current_cooldown != 0)
        {
            current_cooldown -= 1;
            Database.UpdateUnitAbility(this, true);
        }
    }
}

// TANK
public class Earthshaker : Ability
{
    public Earthshaker(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Game game)
    {
        Hex center_hex = game.map.GetHex(cast_unit.column, cast_unit.row);

        Unit unit = null;
        foreach (Hex h in game.map.HexesInRange(center_hex, range))
        {
            unit = game.GetUnit(h);
            if (unit != null && unit.class_type != cast_unit.class_type)
            {
                Stun stun = new Stun(unit, this);
                Database.CreateCC(game,stun);
                unit.RecieveDamage(quantity, game);
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        abilityMoves = game.map.HexesInRange(hex, range);
    }
}
public class Fear : Ability
{
    public Fear(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Game game)
    {
        Hex center_hex = game.map.GetHex(cast_unit.column, cast_unit.row);

        Unit unit = null;
        foreach (Hex h in game.map.HexesInRange(center_hex, range))
        {
            unit = game.GetUnit(h);
            if (unit != null && unit.class_type != cast_unit.class_type)
            {
                Disarm disarm = new Disarm(unit, this);
                Database.CreateCC(game, disarm);
                unit.RecieveDamage(quantity, game);
                if(unit.CurrentState != unit.death_state)
                {
                    Vector2 opposite_direction = new Vector2(unit.column, unit.row) - new Vector2(cast_unit.column, cast_unit.row);
                    Hex hex = game.map.GetHex(unit.column + (int)opposite_direction.X, unit.row + (int)opposite_direction.Y);
                    if (hex != null && hex.walkable)
                        unit.SetPath(game.map.GetHex(unit.column, unit.row), hex, game);
                }
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        abilityMoves = game.map.HexesInRange(hex, range);
    }
}

// ARCHER
public class Powershot : Ability
{
    public Powershot(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        if(CheckHexOnDiagonal(0, -1, hex, game))
            return;
        if (CheckHexOnDiagonal(0, 1, hex, game))
            return;
        if (CheckHexOnDiagonal(1, -1, hex, game))
            return;
        if (CheckHexOnDiagonal(-1, 1, hex, game))
            return;
        if (CheckHexOnDiagonal(1, 0, hex, game))
            return;
        if (CheckHexOnDiagonal(-1, 0, hex, game))
            return;

        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves, game);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves, game);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves, game);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves, game);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves, game);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves, game);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes, Game game)
    {
        Hex h = null;
        Unit unit = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = game.map.GetHex(i * column + hex.column, i * row + hex.row);
            if (h != null)
            {
                if (!h.walkable)
                {
                    unit = game.GetUnit(h);
                    if (unit.class_type != cast_unit.class_type)
                    {
                        diagonal_hexes.Add(h);
                        break;
                    }
                    else
                        diagonal_hexes.Add(h);
                }
                else
                    diagonal_hexes.Add(h);
            }
        }
    }
    private bool CheckHexOnDiagonal(int column, int row, Hex hex, Game game)
    {
        bool isOnDiagonal = false;
        Hex h = null;
        Unit enemy = null;

        for (int i = 1; i < range + 1; i++)
        {
            h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
            if (h != null && hex == h)
            {
                isOnDiagonal =  true;
                break;
            }
        }

        if(isOnDiagonal)
        {
            for (int i = 1; i < range + 1; i++)
            {
                h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    enemy = game.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                    {
                        enemy.RecieveDamage(quantity, game);
                        cast_unit.ChangeState(cast_unit.idle_state, game);
                        break;
                    }
                }
            }
        }

        return isOnDiagonal;
    }
    
}
public class Trap : Ability
{
    public Trap(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        ClassType not_visible_for = cast_unit.class_type == ClassType.Light ? ClassType.Dark : ClassType.Light;
        Modified_Hex modified_Hex = new Modified_Hex()
        {
            ability_id = id,
            type = Modifier.Trap,
            not_visible_for_the_class_type = not_visible_for,
        };

        hex.modified_hexes.Add(modified_Hex);
        Database.CreateModifiedHex(hex, modified_Hex);

        cast_unit.ChangeState(cast_unit.idle_state, game);

    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
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
    }

    public override void UseAbility(Hex hex, Game game)
    {
         Hex cast_unit_hex = game.map.GetHex(cast_unit.column, cast_unit.row);
        if (cast_unit_hex != null)
        {
            cast_unit_hex.walkable = true;

            Unit enemy = game.GetUnit(hex);
            if (enemy != null)
            {
                enemy.RecieveDamage(quantity,game);
                if (enemy.CurrentState != enemy.death_state)
                {
                    enemy.path.Clear();
                    enemy.path.Add(hex);
                    enemy.path.Add(cast_unit_hex);

                    enemy.ChangeState(enemy.move_state, game);
                }
            }
            hex.walkable = true;
            cast_unit.SetPath(cast_unit_hex,hex, game);
        }
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = game.map.HexesInRange(hex, 2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }
        bool hexes_between_are_walkable = true;
        foreach (Hex h in hexesInRangeOf2)
        {
            Unit enemy = game.GetUnit(h);
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
                if(hexes_between_are_walkable)
                    abilityMoves.Add(h);
            }
        }
    }
}
public class Warstrike : Ability
{
    public Warstrike(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit enemy = game.GetUnit(hex);
        if(enemy != null && enemy.class_type != cast_unit.class_type)
        {
            Stun stun = new Stun(enemy, this);
            Database.CreateCC(game, stun);
            enemy.RecieveDamage(quantity,game);
        }

        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = game.map.HexesInRange(hex,2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }
        foreach (Hex h in hexesInRangeOf2)
        {
            Unit enemy = game.GetUnit(h);
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
    }
}
public class TheFool : Ability
{
    public TheFool(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
}

// Wizzard
public class Blessing : Ability
{
    public Blessing(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit aliance_unit = game.GetUnit(hex);

        if (aliance_unit.stats.CurrentHealth + quantity > aliance_unit.stats.MaxHealth)
            aliance_unit.stats.CurrentHealth = aliance_unit.stats.MaxHealth;
        else
            aliance_unit.stats.CurrentHealth += quantity;

        Database.UpdateUnitHealth(aliance_unit);

        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        Unit aliance_unit = null;
        foreach (Hex h in game.map.HexesInRange(hex, range))
        {
            aliance_unit = game.GetUnit(h);
            if (aliance_unit != null && aliance_unit.class_type == cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Skyfall : Ability
{
    public Skyfall(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit enemy_unit = game.GetUnit(hex);

        Stun stun = new Stun(enemy_unit, this);
        Database.CreateCC(game, stun);

        enemy_unit.RecieveDamage(quantity,game);

        foreach (Hex h in hex.neighbors)
        {
            enemy_unit = game.GetUnit(h);
            if (enemy_unit != null && enemy_unit.class_type != cast_unit.class_type)
                enemy_unit.RecieveDamage(quantity, game);
        }
        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
        hexes.Remove(hex);

        Unit enemy = null;
        foreach (Hex h in hexes)
        {
            enemy = game.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Fireball : Ability
{
    public Fireball(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        if (CheckHexOnDiagonal(0, -1, hex, game))
            return;
        if (CheckHexOnDiagonal(0, 1, hex, game))
            return;
        if (CheckHexOnDiagonal(1, -1, hex, game))
            return;
        if (CheckHexOnDiagonal(-1, 1, hex, game))
            return;
        if (CheckHexOnDiagonal(1, 0, hex, game))
            return;
        if (CheckHexOnDiagonal(-1, 0, hex, game))
            return;

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves, game);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves, game);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves, game);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves, game);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves, game);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves, game);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes, Game game)
    {
        Hex h = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = game.map.GetHex(i * column + hex.column, i * row + hex.row);
            if (h != null)
                diagonal_hexes.Add(h);
        }
    }
    private bool CheckHexOnDiagonal(int column, int row, Hex hex, Game game)
    {
        bool isOnDiagonal = false;
        Hex h = null;
        Unit enemy = null;
        for (int i = 1; i < range + 1; i++)
        {
            h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
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
                h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    enemy = game.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                        enemy.RecieveDamage(quantity,game);
                }
            }
            cast_unit.ChangeState(cast_unit.idle_state, game);
        }

        return isOnDiagonal;
    }
    
}
public class Necromancy : Ability
{
    public Necromancy(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit unit = game.GetUnit(hex);
        if(unit != null)
        {
            if(unit.class_type == cast_unit.class_type)
            {
                cast_unit.RecieveDamage(quantity,game);
                if (unit.stats.CurrentHealth + quantity > unit.stats.MaxHealth)
                    unit.stats.CurrentHealth = unit.stats.MaxHealth;
                else
                    unit.stats.CurrentHealth += quantity;
                Database.UpdateUnitHealth(unit);
            }
            else
            {
                unit.RecieveDamage(quantity, game);
                if (cast_unit.stats.CurrentHealth + quantity > cast_unit.stats.MaxHealth)
                    cast_unit.stats.CurrentHealth = cast_unit.stats.MaxHealth;
                else
                    cast_unit.stats.CurrentHealth += quantity;
                Database.UpdateUnitHealth(cast_unit);
            }

        }
        cast_unit.ChangeState(cast_unit.idle_state, game);

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit unit = game.GetUnit(h);
            if (unit != null)
                abilityMoves.Add(h);
        }
    }
}
public class Curse : Ability
{
    public Curse(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit unit = game.GetUnit(hex);

        Disarm disarm = new Disarm(unit, this);
        Database.CreateCC(game, disarm);

        unit.RecieveDamage(quantity, game);

        foreach (Hex h in hex.neighbors)
        {
            unit = game.GetUnit(h);
            if (unit != null && unit.class_type != cast_unit.class_type)
            {
                unit.RecieveDamage(quantity, game);
                if (unit.CurrentState != unit.death_state)
                {
                    Vector2 opposite_direction = new Vector2(unit.column, unit.row) - new Vector2(hex.column, hex.row);
                    Hex hex_direction = game.map.GetHex(unit.column + (int)opposite_direction.X, unit.row + (int)opposite_direction.Y);
                    if (hex_direction != null && hex_direction.walkable)
                        unit.SetPath(game.map.GetHex(unit.column, unit.row), hex_direction, game);
                }
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
        hexes.Remove(hex);

        foreach (Hex h in hexes)
        {
            Unit enemy = game.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}
public class Vampirism : Ability
{
    public Vampirism(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        List<Hex> longestEnemyPath = PathFinding.PathFinder.BFS_LongestPath(hex, cast_unit, ref game);
        Unit enemy = null;
        for (int i = 0; i < 5; i++)
        {
            if (i < longestEnemyPath.Count)
            {
                enemy = game.GetUnit(longestEnemyPath[i]);
                if (enemy != null)
                    enemy.RecieveDamage(quantity,game);

                if (cast_unit.stats.CurrentHealth + quantity > cast_unit.stats.MaxHealth)
                    cast_unit.stats.CurrentHealth = cast_unit.stats.MaxHealth;
                else
                    cast_unit.stats.CurrentHealth += quantity;

                Database.UpdateUnitHealth(cast_unit);
            }
        }
        cast_unit.ChangeState(cast_unit.idle_state, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
        hexes.Remove(hex);

        Unit enemy = null;
        foreach (Hex h in hexes)
        {
            enemy = game.GetUnit(h);
            if (enemy != null && enemy.class_type != cast_unit.class_type)
                abilityMoves.Add(h);
        }
    }
}

public class TricsOfTheTradeIllusion : Ability
{
    public TricsOfTheTradeIllusion(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
}
public class TheFoolIllusion : Ability
{
    public TheFoolIllusion(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Game game)
    {
        Hex unit_hex = game.map.GetHex(cast_unit.column, cast_unit.row);
        Unit enemy_unit = null;
        foreach (var neighbor in unit_hex.neighbors)
        {
            enemy_unit = game.GetUnit(neighbor);
            if (enemy_unit != null && enemy_unit.class_type != cast_unit.class_type)
                enemy_unit.RecieveDamage(quantity,game);
        }
    }
}

public class Swordsman_SA : Ability
{
    public Swordsman_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Unit enemy, Game game)
    {
        if(enemy.class_type != cast_unit.class_type)
        {
            Hex hex = game.map.GetHex(cast_unit.column, cast_unit.row);
            Hex enemy_hex = game.map.GetHex(enemy.column, enemy.row);
            if (enemy_hex != null && hex != null && hex.neighbors.Contains(enemy_hex))
                if (Vector2.Dot(cast_unit.TransformForward(), enemy_hex.position - hex.position) > 0)
                {
                    cast_unit.SetAttack(hex, enemy, game);

                    NetSwordsmanSpecialAbility responess = new NetSwordsmanSpecialAbility();
                    responess.unit_id = cast_unit.id;
                    responess.enemy_id = enemy.id;
                    game.SendMessageToPlayers(responess);
                }
        }
    }
}
public class Tank_SA : Ability
{
    public Tank_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }

    public override void UseAbility(Hex hex, Game game)
    {
        Unit enemy_unit = game.GetUnit(hex);

        Vector2 opposite_direction = new Vector2(enemy_unit.column, enemy_unit.row) - new Vector2(cast_unit.column, cast_unit.row);
        Hex desired_hex = game.map.GetHex(enemy_unit.column + (int)opposite_direction.X, enemy_unit.row + (int)opposite_direction.Y);
        if (desired_hex != null && desired_hex.walkable)
        {
            enemy_unit.SetPath(hex, desired_hex, game);

            Stun stun = new Stun(enemy_unit, this);
            Database.CreateCC(game, stun);

            Hex cast_unit_hex = game.map.GetHex(cast_unit.column, cast_unit.row);
            hex.walkable = true;
            cast_unit.SetPath(cast_unit_hex, hex, game);
        }

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        foreach (Hex neighbor in hex.neighbors)
        {
            Unit enemy_unit = game.GetUnit(neighbor);
            if (enemy_unit != null && cast_unit.class_type != enemy_unit.class_type)
            {
                Vector2 opposite_direction = new Vector2(enemy_unit.column, enemy_unit.row) - new Vector2(cast_unit.column, cast_unit.row);
                Hex desired_hex = game.map.GetHex(enemy_unit.column + (int)opposite_direction.X, enemy_unit.row + (int)opposite_direction.Y);
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
    public override void UseAbility(Hex hex, Game game)
    {
        Hex cast_unit_hex = game.map.GetHex(cast_unit.column, cast_unit.row);
        List<Unit> enemy_units = new List<Unit>();

        foreach (Hex hex_neighbor in hex.neighbors)
        {
            if (cast_unit_hex.neighbors.Contains(hex_neighbor) && !hex_neighbor.walkable)
            {
                Unit enemy = game.GetUnit(hex_neighbor);
                if (enemy != null && enemy.class_type != cast_unit.class_type)
                    enemy_units.Add(enemy);
            }
        }

        foreach (Unit enemy in enemy_units)
            enemy.RecieveDamage(quantity,game);

        cast_unit.SetPath(cast_unit_hex, hex, game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexesInRangeOf2 = game.map.HexesInRange(hex, 2);
        hexesInRangeOf2.Remove(hex);
        foreach (Hex neighbor in hex.neighbors)
        {
            if (hexesInRangeOf2.Contains(neighbor))
                hexesInRangeOf2.Remove(neighbor);
        }

        foreach (Hex h in hexesInRangeOf2)
        {
            if (h.walkable)
            {
                foreach (Hex hex_neighbor in h.neighbors)
                {
                    if (hex.neighbors.Contains(hex_neighbor) && !hex_neighbor.walkable)
                    {
                        Unit enemy = game.GetUnit(hex_neighbor);
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

    public override void UseAbility(Hex hex, Game game)
    {
        Hex startHex = game.map.GetHex(cast_unit.column, cast_unit.row);
        if(startHex != null)
        {
            cast_unit.SetPath(startHex,hex, game);
            List<Hex> hexes = PathFinding.PathFinder.BFS_HexesMoveRange(startHex, range, ref game.map.hexes); 

            hexes.Remove(startHex);
            if(cast_unit.path.Count != 0)
            {
                hexes.Remove(cast_unit.path[cast_unit.path.Count - 1]);

                List<Hex> walkable_hexes = new List<Hex>();
                foreach (Hex h in hexes)
                    if (h.walkable)
                        walkable_hexes.Add(h);

                Random random = new Random();

                if (walkable_hexes.Count != 0)
                {                 

                    int rand = random.Next(walkable_hexes.Count);
                    Hex first_hex = walkable_hexes[rand];
                    walkable_hexes.RemoveAt(rand);

                    Data.Unit illusion_data = JesterToIllusion();
                    illusion_data.id = Database.CreateJester(game.player1, illusion_data);
                    MoveJesterIlusion(illusion_data, startHex, first_hex, game);


                    if (walkable_hexes.Count != 0)
                    {
                        rand = random.Next(walkable_hexes.Count);
                        Hex second_hex = walkable_hexes[rand];
                        walkable_hexes.RemoveAt(rand);

                        illusion_data = JesterToIllusion();
                        illusion_data.id = Database.CreateJester(game.player1, illusion_data);
                        MoveJesterIlusion(illusion_data, startHex, second_hex, game);
                    }

                }
            }
        }

    }   
    private void MoveJesterIlusion(Data.Unit illusion_data, Hex start_hex,Hex desired_hex, Game game)
    {
        Spawner spawner = new Spawner();
        Unit illusion = spawner.SpawnUnit(illusion_data);
        game.units_to_add.Add(illusion);
        illusion.SetPath(start_hex, desired_hex, game);

        NetJesterSpecialAbility responess = new NetJesterSpecialAbility();
        responess.xml_unit_data = Data.Serialize_Test<Data.Unit>(illusion_data);
        responess.to_hex = desired_hex.HexToData();
        game.SendMessageToPlayers(responess);
    }
    private Data.Unit JesterToIllusion()
    {
        Data.Unit unit_data = cast_unit.UnitToData();
        unit_data.unit_type = (int)UnitType.JesterIllusion;
        unit_data.special_ability.globalID = (int)UnitType.JesterIllusion;
        if (unit_data.class_type == (int)ClassType.Light)
        {
            unit_data.id = 17;
            unit_data.abilities[0].globalID = (int)AbilityClass.Illu_Trics_of_the_trade;
        }
        else
        {
            unit_data.id = 18;
            unit_data.abilities[0].globalID = (int)AbilityClass.Illu_The_Fool;
        }
        return unit_data;
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        List<Hex> hexes = game.map.HexesInRange(hex, range);
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

    public override void UseAbility(Hex hex, Game game)
    {
        cast_unit.SetPath(game.map.GetHex(cast_unit.column,cast_unit.row), hex,game);
    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();

        foreach (Hex h in game.map.HexesInRange(hex, 2))
        {
            if (h.walkable)
                abilityMoves.Add(h);
        }
    }
}
public class Queen_SA : Ability
{
    bool desired_hex_is_found = false;
    Unit enemy;
    public Queen_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
    public override void UseAbility(Hex hex, Game game)
    {
        WalkAndAttack(hex, game);

    }

    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        desired_hex_is_found = false;
        enemy = null;
        //center -> bottom
        AddToDiagonalHexes(0, -1, hex, ref abilityMoves, game);
        // center -> top
        AddToDiagonalHexes(0, +1, hex, ref abilityMoves, game);

        //center -> top righ
        AddToDiagonalHexes(+1, -1, hex, ref abilityMoves, game);
        // center ->  bottom left
        AddToDiagonalHexes(-1, +1, hex, ref abilityMoves, game);

        //center -> top left
        AddToDiagonalHexes(-1, 0, hex, ref abilityMoves, game);
        // center -> bottom right
        AddToDiagonalHexes(1, 0, hex, ref abilityMoves, game);
    }

    private void AddToDiagonalHexes(int column, int row, Hex hex, ref List<Hex> diagonal_hexes, Game game)
    {
        Hex h = game.map.GetHex(column + hex.column, row + hex.row); 
        bool hex_is_found = false;
        if (h != null && h.walkable)
        {
            for (int i = 1; i < range + 1; i++)
            {
                h = game.map.GetHex(i * column + hex.column, i * row + hex.row);
                if (h != null)
                {
                    if (!h.walkable)
                    {
                        Unit unit = game.GetUnit(h);
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
                    h = game.map.GetHex(i * column + hex.column, i * row + hex.row);
                    if (h != null)
                    {
                        if (!h.walkable)
                        {
                            Unit unit = game.GetUnit(h);
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
                       /* else
                            diagonal_hexes.Add(h);*/
                    }
                }
            }
        }
       
    }

    private void WalkAndAttack(Hex hex, Game game)
    {
        if(!desired_hex_is_found)
        {
            if (CheckHexOnDiagonal(0, 1, hex, game))
                return;
            if (CheckHexOnDiagonal(0, -1, hex, game))
                return;
            if (CheckHexOnDiagonal(1, -1, hex, game))
                return;
            if (CheckHexOnDiagonal(-1, 1, hex, game))
                return;
            if (CheckHexOnDiagonal(1, 0, hex, game))
                return;
            if (CheckHexOnDiagonal(-1, 0, hex, game))
                return;

            cast_unit.ChangeState(cast_unit.idle_state, game);
        }
        else
        {
            if (cast_unit.path.Count != 0)
            {
                if (cast_unit.path.Count == 2)
                {
                    cast_unit.rotationAngle = cast_unit.RotationInWorldSpace(cast_unit.path[0].position, cast_unit.path[1].position);
                    cast_unit.column = cast_unit.path[1].column;
                    cast_unit.row = cast_unit.path[1].row;
                    cast_unit.path[1].walkable = false;

                    Database.UpdateHex(game, cast_unit.path[1]);
                    Database.UpdateUnitPosition(cast_unit);
                }
                cast_unit.path.RemoveAt(0);
            }
            else
            {
                cast_unit.SetAttack(game.map.GetHex(cast_unit.column, cast_unit.row), cast_unit.enemyUnit, game);
                desired_hex_is_found = false;
            }
        }        
    }

    private bool CheckHexOnDiagonal(int column, int row, Hex hex, Game game)
    {
        bool isOnDiagonal = false;
        for (int i = 1; i < range + 1; i++)
        {
            Hex h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
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
                Hex h = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);
                if (h != null)
                {
                    Unit enemy = game.GetUnit(h);
                    if (enemy != null && enemy.class_type != cast_unit.class_type)
                    {
                        Hex from_hex = game.map.GetHex(cast_unit.column, cast_unit.row);
                        i -= 1;
                        Hex to_hex = game.map.GetHex(i * column + cast_unit.column, i * row + cast_unit.row);

                        cast_unit.path.Clear();
                        cast_unit.path.Add(from_hex);
                        cast_unit.path.Add(to_hex);

                        from_hex.walkable = true;
                        Database.UpdateHex(game, from_hex);

                        desired_hex_is_found = true;
                        cast_unit.enemyUnit = enemy;

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
    }
    public override void UseAbility(Hex hex, Game game)
    {
        WalkToTheMiddle(hex, game);
    }
    public override void GetAbilityHexes(Hex hex, ref List<Hex> abilityMoves, Game game)
    {
        abilityMoves.Clear();
        if (cast_unit.stats.Damage == 0)
        {
            desired_hex_is_found = false;
            Hex middle_hex = game.map.GetHex(0, 0);
            foreach (var neighbor_hex in hex.neighbors)
            {
                if (neighbor_hex == game.map.GetHex(0, 0) && neighbor_hex.walkable)
                    abilityMoves.Add(middle_hex);
            }
        }    
    }

    private void WalkToTheMiddle(Hex hex, Game game)
    {
        if(!desired_hex_is_found)
        {
            Hex from_hex = game.map.GetHex(cast_unit.column,cast_unit.row);

            cast_unit.path.Clear();
            cast_unit.path.Add(from_hex);
            cast_unit.path.Add(hex);

            from_hex.walkable = true;            
            Database.UpdateHex(game, from_hex);

            desired_hex_is_found = true;
        }
        else
        {
            if (cast_unit.path.Count != 0)
            {
                if (cast_unit.path.Count == 2)
                {
                    cast_unit.rotationAngle = cast_unit.RotationInWorldSpace(cast_unit.path[0].position, cast_unit.path[1].position);
                    cast_unit.column = cast_unit.path[1].column;
                    cast_unit.row = cast_unit.path[1].row;
                    cast_unit.path[1].walkable = false;

                    Database.UpdateHex(game, cast_unit.path[1]);
                    Database.UpdateUnitPosition(cast_unit);
                }
                cast_unit.path.RemoveAt(0);
            }
            else
            {
                UpdateKingStats(game);

                game.challenge_royal_activated = true;
                Database.ActivateChallengeRoyal(game);

                cast_unit.ChangeState(cast_unit.idle_state, game);
                game.events.OnExitMovement?.Invoke(cast_unit, game);
            }
        }          
    }

    private void UpdateKingStats(Game game_data)
    {
        int death_units = 0;
        foreach (var unit in game_data.units)
        {
            if (unit.class_type == cast_unit.class_type && unit.CurrentState == unit.death_state)
                if (!unit.GetType().Equals(typeof(JesterLightIllusion)) && !unit.GetType().Equals(typeof(JesterDarkIllusion)))
                    death_units++;
        }

        cast_unit.stats.CurrentHealth = cast_unit.stats.MaxHealth;
        cast_unit.stats.AttackRange = 1;

        if (death_units > 9)
            cast_unit.stats.Damage = 3;
        else if (death_units > 5)
            cast_unit.stats.Damage = 2;
        else if (death_units > 2)
            cast_unit.stats.Damage = 1;

        Console.WriteLine("UPDATED KING: DAMAGE: " + cast_unit.stats.Damage + " DEATH UNITS: " + death_units);
        Database.UpdateUnitStats(cast_unit);
    }

}
public class Jester_Illusion_SA : Ability
{
    public Jester_Illusion_SA(Unit unit, Data.Ability ability_data) : base(unit, ability_data)
    {
    }
}

