using Networking.Server;
using System.Collections.Generic;
using Map;
using System;
using System.Numerics;

namespace Units
{
    public class Jester : Melee
    {
        public Jester(Data.Unit unit) : base(unit)
        {
        }
        public override void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            RemoveIllusions(game_data);
            path.Clear();
            path.Add(from_hex);
            path.Add(to_hex);

            from_hex.walkable = true;
            Database.UpdateHex(game_data, from_hex);

            ChangeState(move_state, game_data);            
        }
        public override void RecieveDamage(int damage, Game game_data)
        {
            RemoveIllusions(game_data);
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
            }
        }
        public override void GetAvailableMoves(Hex hex, ref List<Hex> availableMoves, Game game)
        {
            availableMoves.Clear();
            foreach (Hex h in PathFinding.PathFinder.BFS_HexesMoveRange(hex, 2, ref game.map.hexes))
                availableMoves.Add(h);
        }
        public void RemoveIllusions(Game game_data)
        {
            foreach (var unit in game_data.units)
            {
                if (unit.CurrentState != death_state && unit.unit_type == UnitType.JesterIllusion && unit.class_type == class_type)
                {
                    Jester illusion = unit as Jester;
                    illusion.RemoveIllusion(illusion, game_data);
                }
            }
        }
        private void RemoveIllusion(Jester illusion, Game game_data)
        {
            if (illusion.CurrentState != illusion.death_state)
            {
                game_data.map.GetHex(illusion.column, illusion.row).walkable = true;
                Database.UpdateHex(game_data, game_data.map.GetHex(illusion.column, illusion.row));

                stats.CurrentHealth = 0;
                ChangeState(death_state, game_data);
                UnSubscribe(game_data.events);
                Database.DestroyUnit(this);
                illusion.column = -500;
                illusion.row = -500;

            }
        }
    }

    public class JesterLight : Jester
    {
        public JesterLight(Data.Unit unit) : base(unit)
        {
        }
    }


    public class JesterDark : Jester
    {
        public JesterDark(Data.Unit unit) : base(unit)
        {
        }
    }

    public class JesterLightIllusion : JesterLight
    {
        public JesterLightIllusion(Data.Unit unit) : base(unit)
        {
        }
        public override void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            path.Clear();
            path.Add(from_hex);
            path.Add(to_hex);

            from_hex.walkable = true;
            Database.UpdateHex(game_data, from_hex);

            ChangeState(move_state, game_data);
        }
        public override void RecieveDamage(int damage, Game game_data)
        {
            if (CurrentState != death_state)
            {
                game_data.map.GetHex(column, row).walkable = true;
                Database.UpdateHex(game_data, game_data.map.GetHex(column, row));


                stats.CurrentHealth = 0;
                ChangeState(death_state, game_data);
                UnSubscribe(game_data.events);

                Database.DestroyUnit(this);

                column = -500;
                row = -500;
            }

        }
       /* public override void Subscribe(UnitEvents _events)
        {
            _events.OnStartMovement += OnStartMovement;
            _events.OnRecieveDamage += OnRecieveDamage;
        }

        public override void UnSubscribe(UnitEvents _events)
        {
            _events.OnStartMovement -= OnStartMovement;
            _events.OnRecieveDamage -= OnRecieveDamage;
        }

        private void OnStartMovement(Unit unit, Game game_data)
        {
            //Destroy
            if (unit.unit_type == UnitType.Jester && unit.class_type == class_type)
                RecieveDamage(stats.CurrentHealth, game_data);
        }
        private void OnRecieveDamage(Unit unit, Game game_data)
        {
            //Destroy
            if (unit.unit_type == UnitType.Jester && unit.class_type == class_type)
                RecieveDamage(stats.CurrentHealth, game_data);
        }*/
    }
    public class JesterDarkIllusion : JesterLight
    {
        public JesterDarkIllusion(Data.Unit unit) : base(unit)
        {
        }
        public override void SetPath(Hex from_hex, Hex to_hex, Game game_data)
        {
            path.Clear();
            path.Add(from_hex);
            path.Add(to_hex);

            from_hex.walkable = true;
            Database.UpdateHex(game_data, from_hex);

            ChangeState(move_state, game_data);
        }
        public override void GetAttackMoves(Hex hex, ref List<Hex> attackMoves, Game game)
        {
            attackMoves.Clear();
        }
        public override void RecieveDamage(int damage, Game game_data)
        {
            if (CurrentState != death_state) 
            { 
                abilities[0].UseAbility(game_data);
                game_data.map.GetHex(column, row).walkable = true;
                Database.UpdateHex(game_data, game_data.map.GetHex(column, row));


                stats.CurrentHealth = 0;
                ChangeState(death_state, game_data);
                UnSubscribe(game_data.events);
                
                Database.DestroyUnit(this);

                column = -500;
                row = -500;
            }
        }
    }
}

