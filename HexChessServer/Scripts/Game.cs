using System;
using System.Collections.Generic;
using System.Text;
using Units;
using Map;
using Networking.Server;
using System.Linq;
using System.Threading.Tasks;

public class Game
{
    public int game_id;
    public Player player1;
    public Player player2;
    public ClassType class_on_turn;

    public int move;
    public int challenge_royal_move;
    public bool challenge_royal_activated;
    public int win = 0;

    public Map.Map map;
    public List<Unit> units = new List<Unit>();
    public List<Unit> units_to_add = new List<Unit>();
    public UnitEvents events = new UnitEvents();

    public bool Update(float deltaTime)
    {
        foreach (Unit unit in units)
            unit.Update(deltaTime,this);

        if(units_to_add.Count != 0)
            for (int i = units_to_add.Count - 1; i >= 0; i--)
            {
                units.Add(units_to_add[i]);
                units_to_add.RemoveAt(i);
                return false;
            }

        foreach (Unit unit in units)
            if(unit.CurrentState != unit.idle_state && unit.CurrentState != unit.death_state)
                return false;

        return true;
    }
    public void Initialize(List<Data.Hex> hexes_data, List<Data.Unit> units_data)
    {
        map = new Map.Map(hexes_data);
        units = SpawnUnits(units_data);      
    }
    public List<Unit> SpawnUnits(List<Data.Unit> units_data)
    {
        Spawner spawner = new Spawner();
        List<Unit> units = new List<Unit>();

        foreach (var unit_data in units_data)
        {
            units.Add(spawner.SpawnUnit(unit_data));
            units[units.Count - 1].Subscribe(events);
        }

        return units;
    }

    public void MoveUnit(Player player, int unit_id, Data.Hex to_data_hex)
    {
        if (player.player_data.class_type == class_on_turn)
        {
            Unit unit = null;
            foreach (var u in units)
            {
                if (u.id == unit_id)
                {
                    unit = u;
                    break;
                }
            }
            if (unit != null && unit.class_type == class_on_turn)
            {
                Hex hex_from = map.GetHex(unit.column, unit.row);
                Hex hex_to = map.GetHex(to_data_hex.column, to_data_hex.row);
                if (hex_from != null && hex_to != null)
                {
                    List<Hex> availableMoves = new List<Hex>();
                    unit.GetAvailableMoves(hex_from, ref availableMoves, this);

                    if (availableMoves.Contains(hex_to))
                    {
                        Console.WriteLine("Move unit " + unit.id + ":" + unit.unit_type.ToString() + unit.class_type.ToString() +
                        " from [" + hex_from.column + "," + hex_from.row + "] to " +
                        "[" + hex_to.column + "," + hex_to.row + "] ");

                        unit.SetPath(hex_from, hex_to, this);
                        NetMove responess = new NetMove
                        {
                            unit_id = unit_id,
                            to_hex = to_data_hex
                        };
                        SendMessageToPlayers(responess);
                    }
                }
            }
        }
    }

    public void UseAbilityUnit(Player player, int unit_id, Data.Hex to_data_hex, AbilityInput input)
    {
        if (player.player_data.class_type == class_on_turn)
        {
            Unit unit = null;
            foreach (var u in units)
            {
                if (u.id == unit_id)
                {
                    unit = u;
                    break;
                }
            }
            if (unit != null && unit.class_type == class_on_turn)
            {
                Hex hex_from = map.GetHex(unit.column, unit.row);
                Hex hex_to = map.GetHex(to_data_hex.column, to_data_hex.row);
                if (hex_from != null && unit.abilities != null)
                {
                    Ability ability = null;
                    if (input == AbilityInput.S)
                        ability = unit.special_ability;
                    else
                        ability = unit.abilities[(int)input];
                    if (ability != null && ability.current_cooldown == 0)
                    {
                        NetAbility responess = null;
                        switch (ability.ability_type)
                        {
                            case AbilityType.Targetable:
                                Console.WriteLine("Ability used unit " + unit.id + ":" + unit.unit_type.ToString() + unit.class_type.ToString() +
                     " from [" + hex_from.column + "," + hex_from.row + "] to [" + to_data_hex.column + "," + to_data_hex.row + "]");

                                unit.SetAbility(ability, hex_to, this);
                                responess = new NetAbility
                                {
                                    input = input,
                                    unit_id = unit_id,
                                    to_hex = to_data_hex
                                };
                                SendMessageToPlayers(responess);
                                break;
                            case AbilityType.Instant:

                                Console.WriteLine("Ability used unit " + unit.id + ":" + unit.unit_type.ToString() + unit.class_type.ToString() +
                      " from [" + hex_from.column + "," + hex_from.row + "]");

                                unit.SetAbility(ability, hex_to, this);
                                responess = new NetAbility
                                {
                                    input = input,
                                    unit_id = unit_id,
                                    to_hex = to_data_hex
                                };
                                SendMessageToPlayers(responess);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            }
    }

    public void AttackUnit(Player player, int unit_id, int enemy_id)
    {
            if (player.player_data.class_type == class_on_turn)
            {
            Unit unit = null;
            Unit enemy_unit = null;
            foreach (var u in units)
            {
                if (u.id == unit_id)
                    unit = u;
                else if (u.id == enemy_id)
                    enemy_unit = u;
            }
            if (unit != null && unit.class_type == class_on_turn && enemy_unit != null && unit.class_type != enemy_unit.class_type)
            {
                Hex hex_from = map.GetHex(unit.column, unit.row);
                Hex hex_to = map.GetHex(enemy_unit.column, enemy_unit.row);
                if (hex_from != null && hex_to != null)
                {
                    List<Hex> attackMoves = new List<Hex>();
                    unit.GetAttackMoves(hex_from, ref attackMoves, this);
                    if (attackMoves.Contains(hex_to))
                    {
                        Console.WriteLine("Attack unit " + unit.id + ":" + unit.unit_type.ToString() + unit.class_type.ToString() +
                    " from [" + hex_from.column + "," + hex_from.row + "] to " +
                    "[" + hex_to.column + "," + hex_to.row + "] Enemy unit " + enemy_unit.id + ":" + enemy_unit.unit_type.ToString() + enemy_unit.class_type.ToString());

                        unit.SetAttack(hex_from, enemy_unit, this);
                        NetAttack responess = new NetAttack
                        {
                            unit_id = unit_id,
                            enemy_id = enemy_id
                        };
                        SendMessageToPlayers(responess);
                    }
                }
            }
        }
    }
    public void OnKingDeath(ClassType classType)
    {
        if (player1.player_data.class_type == classType)
            if (player2 != null)
            {
                win = (int)player2.player_data.account_id;
                ELO.EloRating(player1, player2, 30, false);
            }
        if (player2.player_data.class_type == classType)
            if (player1 != null)
            {
                win = (int)player1.player_data.account_id;
                ELO.EloRating(player1, player2, 30, true);

            }
        Database.UpdateWinner(this);
    }
    public Unit GetUnit(Hex hex)
    {
        if (hex == null)
            return null;
        foreach (Unit unit in units)
        {
            if (unit.column == hex.column && unit.row == hex.row)
                return unit;
        }
        return null;
    }

    public void SendMessageToPlayers(NetMessage message)
    {
        Sender.TCP_SendToClient(player1.id, message);
        if (player2 != null)
            Sender.TCP_SendToClient(player2.id, message);
    }

    public void EndTurn()
    {
        if (player2 != null)
            class_on_turn = player2.player_data.class_type;
        else // EDIT IF WE ADDING MORE THAN 2 CLASSES
            class_on_turn = class_on_turn == ClassType.Light ? ClassType.Dark : ClassType.Light;

        foreach (Unit unit in units)
        {
            if (unit.class_type == class_on_turn)
            {
                unit.special_ability.ReduceCooldown();
                foreach (Ability ability in unit.abilities)
                    ability.ReduceCooldown();
            }
        }

        foreach (Unit unit in units)
        {          
            if (unit.class_type == class_on_turn)
            {
                for (int i = unit.cc.Count - 1; i >= 0; i--)
                {
                    if (unit.cc[i].ReduceCooldown())
                        unit.cc.RemoveAt(i);
                }
            }
        }

        if (challenge_royal_activated)
        {
            challenge_royal_move--;
            Database.UpdateChallengeRoyalMove(this);
            TryToRemoveFields();
        }
        move++;
    }
    private void TryToRemoveFields()
    {
        int inner = 0;
        if (challenge_royal_move == 20)
            inner = 3;
        else if (challenge_royal_move == 10)
            inner = 2;
        else if (challenge_royal_move == 0)
            inner = 1;
        else return;

        Console.WriteLine("REMOVE OUTER RING");
        Hex center_hex = map.GetHex(0, 0);
        List<Hex> inner_hexes = map.HexesInRange(center_hex, inner);

        foreach (var hex in map.hexes)
        {
            if(!inner_hexes.Contains(hex))
            {
                Unit unit = GetUnit(hex);
                if (unit != null)
                {
                    unit.stats.CurrentHealth = 0;
                    Database.UpdateUnitHealth(unit);
                    unit.ChangeState(unit.death_state, this);
                }
                else
                {
                    hex.walkable = false;
                    Database.UpdateHex(this, hex);
                }
            }
        }
    }

    public Ability GetAbilityByID(int id)
    {
        foreach (var unit in units)
        {
            foreach (var ability in unit.abilities)
            {
                if (ability.id == id)
                    return ability;
            }

            if (unit.special_ability.id == id)
                return unit.special_ability;
        }
        return null;
    }
}


static class ELO
{

    // Function to calculate the Probability
    static float Probability(float rating1,
                                 float rating2)
    {
        return 1.0f * 1.0f / (1 + 1.0f *
               (float)(Math.Pow(10, 1.0f *
                 (rating1 - rating2) / 400)));
    }

    // Function to calculate Elo rating
    // K is a constant.
    // d determines whether Player A wins or
    // Player B.
    public static void EloRating(Player player1, Player player2,
                                int K, bool d)
    {
        float Ra = player1.player_data.rank;
        float Rb = player2.player_data.rank;

        // To calculate the Winning
        // Probability of Player B
        float Pb = Probability(Ra, Rb);

        // To calculate the Winning
        // Probability of Player A
        float Pa = Probability(Rb, Ra);

        // Case -1 When Player A wins
        // Updating the Elo Ratings
        if (d == true)
        {
            Ra = Ra + K * (1 - Pa);
            Rb = Rb + K * (0 - Pb);
        }

        // Case -2 When Player B wins
        // Updating the Elo Ratings
        else
        {
            Ra = Ra + K * (0 - Pa);
            Rb = Rb + K * (1 - Pb);
        }
        Console.WriteLine("Player 1 new MMR: " + Ra);
        Console.WriteLine("Player 2 new MMR: " + Rb);

        if (Ra < 500)
            Ra = 500;
        if (Rb < 500)
            Rb = 500;

        player1.player_data.rank = (int)Ra;
        player2.player_data.rank = (int)Rb;

        Database.UpdatePlayerMMR(player1);
        Database.UpdatePlayerMMR(player2);
    }

}