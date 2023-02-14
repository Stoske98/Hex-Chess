using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Map;
using Networking.Server;

namespace Units
{
    public class Unit
    {
        public int id;
        public int column { get; set; }
        public int row { get; set; }

       // public Vector2 startFromHexPosition { get; set; }

        public UnitStats stats;
        public UnitType unit_type { get; set; }
        public ClassType class_type { get; set; }

        public float rotationAngle;

        public List<Hex> path;

        protected float deltaTime;
        protected float currentTime;

        public UnitState CurrentState;
        public readonly IdleState idle_state = new IdleState();
        public readonly MoveState move_state = new MoveState();
        public readonly AttackState attack_state = new AttackState();
        public readonly AbilityState ability_state = new AbilityState();
        public readonly DeathState death_state = new DeathState();

        public Ability[] abilities;
        public Ability special_ability;
        public List<CC> cc;

        public Unit enemyUnit;
        protected Ability ability;
        protected Hex ability_hex;

        public Unit(Data.Unit data)
        {
            id = data.id;
            column = data.hex_column;
            row = data.hex_row;
            unit_type = (UnitType)data.unit_type;
            class_type = (ClassType)data.class_type;
            path = new List<Hex>();
            cc = new List<CC>();
            stats = new UnitStats
            {
                MaxHealth = data.max_health,
                CurrentHealth = data.current_health,

                Damage = data.damage,
                AttackRange = data.attack_range,
                AttackSpeed = data.attack_speed
            };
            rotationAngle = data.rotation;

            if (stats.CurrentHealth <= 0)
            {
                CurrentState = death_state;
                column = -500;
                row = -500;
            }
            else
                CurrentState = idle_state;

            abilities = new Ability[data.abilities.Count];

            for (int i = 0; i < data.abilities.Count; i++)
                abilities[i] = Spawner.CreateAbility(this, data.abilities[i]);

            special_ability = Spawner.CreateSpecialAbility(this, data.special_ability);

            foreach (var cc_data in data.cc)
                cc.Add(Spawner.CreateCC(this, cc_data));
        }

        public Data.Unit UnitToData()
        {
            Data.Unit data_unit = new Data.Unit();
            data_unit.id = id;
            data_unit.class_type = (int)class_type;
            data_unit.unit_type = (int)unit_type;
            data_unit.hex_column = column;
            data_unit.hex_row = row;
            data_unit.max_health = stats.MaxHealth;
            data_unit.current_health = stats.CurrentHealth;
            data_unit.damage = stats.Damage;
            data_unit.attack_range = stats.AttackRange;
            data_unit.attack_speed = stats.AttackSpeed;
            data_unit.rotation = rotationAngle;
            data_unit.special_ability = special_ability.AbilityToData();
            foreach (Ability ability in abilities)
                data_unit.abilities.Add(ability.AbilityToData());
            return data_unit;
        }

        public void Update(float _deltaTime,Game game)
        {
            deltaTime = _deltaTime;
            CurrentState.Execute(this, game);
        }
        public void ChangeState(UnitState newState, Game game_data)
        {
            CurrentState.Exit(this, game_data);
            CurrentState = newState;
            CurrentState.Enter(this, game_data);
        }
        public virtual void MoveUnit(Game game_data)
        {
            if (path.Count != 0)
            {                
               if(path.Count == 2)
                {
                    rotationAngle = RotationInWorldSpace(path[0].position, path[1].position);
                    column = path[1].column;
                    row = path[1].row;
                    path[1].walkable = false;

                    Database.UpdateHex(game_data, path[1]);
                    Database.UpdateUnitPosition(this);
                }
                path.RemoveAt(0);
            }
            else
                ChangeState(idle_state, game_data);
        }
        public virtual void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            path = PathFinding.PathFinder.FindPath_AStar(from_hex, to_hex, ref game_data.map.hexes);
            CheckIfTrapIsOnPath();
            from_hex.walkable = true;
            Database.UpdateHex(game_data, from_hex);

            ChangeState(move_state, game_data);
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
        public virtual void SetAttack(Hex from_hex, Unit _enemyUnit, Game game_data)
        {
            enemyUnit = _enemyUnit;
            currentTime = DateTime.Now.Millisecond / 100f;

            rotationAngle = RotationInWorldSpace(from_hex.position, game_data.map.GetHex(enemyUnit.column, enemyUnit.row).position);
            Database.UpdateUnitRotation(this);
            ChangeState(attack_state, game_data);
        }
        public virtual void UseAbility(Game game_data)
        {
            switch (ability.ability_type)
            {
                case AbilityType.Passive:
                    break;
                case AbilityType.Targetable:
                    ability.UseAbility(ability_hex, game_data);
                    break;
                case AbilityType.Instant:
                    ability.UseAbility(game_data);
                    break;
                default:
                    break;
            }
        }

        public void UpdateAbilityCooldown()
        {
            ability.current_cooldown = ability.max_cooldown;
            Database.UpdateUnitAbility(ability);
        }
        public virtual void SetAbility(Ability _ability, Hex hex, Game game_data)
        {
            ability = _ability;
            ability_hex = hex;
            currentTime = DateTime.Now.Millisecond / 100f;

            if (ability.ability_type == AbilityType.Targetable)
            {
                Hex unit_hex = game_data.map.GetHex(column,row);
                if(unit_hex != null)
                {
                    rotationAngle = RotationInWorldSpace(unit_hex.position, hex.position);
                    Database.UpdateUnitRotation(this);
                }
            }

            ChangeState(ability_state, game_data);
        }
        public virtual void AttackUnit(Game game_data)
        {
            if (DateTime.Now.Millisecond / 100f > currentTime + stats.AttackSpeed)
            {
                int enemyColumn = enemyUnit.column;
                int enemyRow = enemyUnit.row;
                if (enemyUnit.CurrentState != enemyUnit.death_state)
                {
                    enemyUnit.RecieveDamage(stats.Damage, game_data);
                    if (enemyUnit.CurrentState == enemyUnit.death_state)
                    {
                        if (CurrentState != death_state)
                            SetPath(game_data.map.GetHex(column, row), game_data.map.GetHex(enemyColumn, enemyRow), game_data);
                        else
                            ChangeState(death_state, game_data);
                    }
                    else
                        ChangeState(idle_state, game_data);
                }
                else
                    ChangeState(idle_state, game_data);
                enemyUnit = null;
            }
        }
        public virtual void RecieveDamage(int damage, Game game_data)
        {
            game_data.events.OnRecieveDamage?.Invoke(this, game_data);
            if (stats.CurrentHealth - damage > 0)
            {
                stats.CurrentHealth -= damage;
                Database.UpdateUnitHealth(this);
            }
            else
            {
                game_data.map.GetHex(column, row).walkable = true;

                stats.CurrentHealth = 0;
                Database.UpdateUnitHealth(this);
                CurrentState = death_state;
                ChangeState(death_state, game_data);

                UnSubscribe(game_data.events);
                Database.UpdateHex(game_data, game_data.map.GetHex(column, row));
                column = -999;
                row = -999;

                if (unit_type == UnitType.King)
                    game_data.OnKingDeath(class_type);
            }            
        }

        public float RotationInWorldSpace(Vector2 from, Vector2 to)
        {
            return (float)AngleBetween(Vector2.UnitY, to - from) * -1;
        }
        public Vector2 TransformForward()
        {
            rotationAngle *= -1;
            rotationAngle /= (float)(180 / Math.PI);
            float v2x = -(float)Math.Sin(rotationAngle);
            float v2y = (float)Math.Cos(rotationAngle);
            return new Vector2(v2x, v2y);
        }

        public double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        public virtual void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();
            foreach (Hex h in hex.neighbors)
                if (h.walkable)
                    availableMoves.Add(h);
        }

        public virtual void GetSpecialMoves(Hex hex, ref List<Hex> specialMoves, Game game)
        {
            specialMoves.Clear();
        }
        public virtual void GetAttackMoves(Hex hex, ref List<Hex> attackMoves, Game game)
        {
            attackMoves.Clear();
            foreach (Hex h in game.map.HexesInRange(hex,stats.AttackRange))
            {
                Unit unit = game.GetUnit(h);
                if (unit != null && unit.class_type != class_type)
                    attackMoves.Add(h);
            }
        }

        protected List<Hex> GetDiagonalsAvailableMove(Hex center, int range, ref List<Hex> diagonal_hexes, ref Map.Map map, bool countUnWalkableFields = false)
        {
            //center -> bottom
            AddToDiagonalHexes(0, -1, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);
            // center -> top
            AddToDiagonalHexes(0, +1, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);

            //center -> top righ
            AddToDiagonalHexes(+1, -1, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);
            // center ->  bottom left
            AddToDiagonalHexes(-1, +1, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);

            //center -> top left
            AddToDiagonalHexes(-1, 0, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);
            // center -> bottom right
            AddToDiagonalHexes(1, 0, center, range, ref diagonal_hexes, ref map, countUnWalkableFields);

            return diagonal_hexes;
        }

        private void AddToDiagonalHexes(int column, int row, Hex hex, int range, ref List<Hex> diagonal_hexes, ref Map.Map map, bool countUnWalkableFields)
        {
            for (int i = 1; i < range + 1; i++)
            {
                Hex h = map.GetHex(i * column + hex.column, i * row + hex.row);
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

        public virtual void Subscribe(UnitEvents _events)
        {
        }
        public virtual void UnSubscribe(UnitEvents events)
        {

        }
    }

    public class UnitEvents
    {
        public Action<Unit, Game> OnExitMovement;
        public Action<Unit, Game> OnStartMovement;
        public Action<Unit, Game> OnRecieveDamage;
    }


    public class Melee : Unit
    {
        public Melee(Data.Unit unit) : base(unit)
        {
        }
    }

    public class Range : Unit
    {
        public Range(Data.Unit unit) : base(unit)
        {
        }
        public override void AttackUnit(Game game_data)
        {
            if (DateTime.Now.Millisecond / 100f > currentTime + stats.AttackSpeed)
            {
                enemyUnit.RecieveDamage(stats.Damage, game_data);
                enemyUnit = null;
                ChangeState(idle_state, game_data);
            }
        }
    }

    public class UnitStats
    {
        public int MaxHealth;
        public int CurrentHealth;

        public int AttackRange;
        public float AttackSpeed;

        public int Damage;

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
}




