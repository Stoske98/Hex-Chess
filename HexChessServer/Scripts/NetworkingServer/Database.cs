using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;
using Units;

namespace Networking.Server
{
    class Database
    {

        #region MySQL
        //private static MySqlConnection _mysqlConnection;
        private const string _mysqlServer = "localhost";
        private const string _mysqlUsername = "root";
        private const string _mysqlPassword = "Kassker98";
        private const string _mysqlDatabase = "hexchess";
        private const UInt16 _mysqlPort = 3306;

        public static MySqlConnection GetMysqlConnection()
        {
            MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder();
            connString.Server = _mysqlServer;
            connString.UserID = _mysqlUsername;
            connString.Password = _mysqlPassword;
            connString.Port = _mysqlPort;
            connString.Database = _mysqlDatabase;
            connString.CharacterSet = "utf8";
            MySqlConnection _mysqlConnection = new MySqlConnection(connString.ToString());
            try
            {
                _mysqlConnection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("MESSAGE: " + e.Message);
                Console.WriteLine("STACK TRACE: " + e.StackTrace);
                Console.WriteLine("SOURCE: " + e.Source);
                Console.WriteLine("DICTIONARY DATA: " + e.Data);
                Console.WriteLine("HELP LINK: " + e.HelpLink);
            }
            return _mysqlConnection;
        }

        public async static void AuthenticatePlayer(int clientID, string deviceID, int matchID)
        {
            //await for database proces authentification and return account id, meanwhile work on another request
            Data.Player player_data = await AuthenticatePlayerAsync(clientID, deviceID, matchID);

            NetAuthentication responess = new NetAuthentication
            {
                player = player_data
            };
            Sender.TCP_SendToClient(clientID, responess);


        }
        public async static void SyncGameData(int clientID)
        {
            List<Data.Unit> units = await GetAllUnitsFromMatchAsync(clientID);
            List<Data.Hex> hexes_data = await GetHexesAsync(clientID);
            Game game = await GetMatchDataAsync(clientID);

            Data.Game game_data = new Data.Game
            {
                class_on_turn = (int)game.class_on_turn,
                move = game.move,
                challenge_royal_move = game.challenge_royal_move,
                challenge_royal_activated = Convert.ToInt32(game.challenge_royal_activated),
                units_data = units,
                hexes_data = hexes_data
            };

            NetSynchronic responess = new NetSynchronic
            {
                xml_player_data = await Data.Serialize<Data.Game>(game_data),
            };
            Sender.TCP_SendToClient(clientID, responess);

        }
        public async static void SendMessage(int clientID, NetChat request)
        {
            Game game_data = await GetMatchDataAsync(clientID);
            request.nickname = Server.clients[clientID].player.player_data.nickname;

            game_data.SendMessageToPlayers(request);
        }
        public async static void UseUnitAbility(int clientID, NetAbility request)
        {
            List<Data.Unit> units_data = await GetAllUnitsFromMatchAsync(clientID);
            List<Data.Hex> hexes_data = await GetHexesAsync(clientID);
            Game game_data = await GetMatchDataAsync(clientID);

            game_data.Initialize(hexes_data, units_data);
            game_data.UseAbilityUnit(Server.clients[clientID].player,request.unit_id, request.to_hex, request.input);
            UpdateTheUnitsDuringTheTurn(game_data);

            // Console.WriteLine("Unit with ID: "+request.unit_id+" cast ability on hex [" + request.to_hex.column + "," + request.to_hex.row + "]");

            if (request.end_turn == 1 ? true : false)
            {
                game_data.EndTurn();
                UpdateOnEndOfTheTurn(game_data); 
                
                await Task.Delay(3000);

                NetEndTurn resspones = new NetEndTurn();
                resspones.class_on_turn = game_data.class_on_turn;
                game_data.SendMessageToPlayers(resspones);
            }
        }
        public async static void MoveUnit(int clientID, NetMove request)
        {
            List<Data.Unit> units_data = await GetAllUnitsFromMatchAsync(clientID);
            List<Data.Hex> hexes_data = await GetHexesAsync(clientID);
            Game game_data = await GetMatchDataAsync(clientID);

            game_data.Initialize(hexes_data, units_data);
            game_data.MoveUnit(Server.clients[clientID].player, request.unit_id, request.to_hex);
            UpdateTheUnitsDuringTheTurn(game_data);

            //   Console.WriteLine("Unit with ID:" + request.unit_id + " move on hex [" + request.to_hex.column +","+ request.to_hex.row + "]");

            if (request.end_turn == 1 ? true : false)
            {
                Console.WriteLine("END TURN");
                game_data.EndTurn();
                UpdateOnEndOfTheTurn(game_data);

                await Task.Delay(3000);

                NetEndTurn resspones = new NetEndTurn();
                resspones.class_on_turn = game_data.class_on_turn;
                game_data.SendMessageToPlayers(resspones);
            }

        }
        public async static void AttackUnit(int clientID, NetAttack request)
        {
            List<Data.Unit> units_data = await GetAllUnitsFromMatchAsync(clientID);
            List<Data.Hex> hexes_data = await GetHexesAsync(clientID);
            Game game_data = await GetMatchDataAsync(clientID);

            game_data.Initialize(hexes_data, units_data);
            game_data.AttackUnit(Server.clients[clientID].player, request.unit_id, request.enemy_id);
            UpdateTheUnitsDuringTheTurn(game_data);

            //  Console.WriteLine("Unit with ID " + request.unit_id + " attack enemy unit with ID: " + request.enemy_id);

            if (request.end_turn == 1 ? true : false)
            {
                game_data.EndTurn();
                UpdateOnEndOfTheTurn(game_data);

                await Task.Delay(3000);

                NetEndTurn resspones = new NetEndTurn();
                resspones.class_on_turn = game_data.class_on_turn;
                game_data.SendMessageToPlayers(resspones);
            }
        }
        private async static Task<Data.Player> AuthenticatePlayerAsync(int clientID, string deviceID, int matchID)
        {
            Task<Data.Player> task = Task.Run(() => {

                Data.Player data = new Data.Player();

                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT account_id, nickname, rank, selected_class FROM accounts WHERE device_id = '{0}'", deviceID);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    data.account_id = int.Parse(reader["account_id"].ToString());
                                    data.nickname = reader["nickname"].ToString();
                                    data.rank = int.Parse(reader["rank"].ToString());
                                    data.class_type = (ClassType)int.Parse(reader["selected_class"].ToString());

                                }
                            }
                        }
                    }

                    connection.Close();
                }

                Server.clients[clientID].player = new Player(clientID, deviceID, matchID, data);
                return data;
            });
            return await task;
        }
        public static void UpdateTheUnitsDuringTheTurn(Game game)
        {
            {
                bool endTurn = false;
                Console.WriteLine("Update start Move", game.move);
                int counter = 0;
                while (!endTurn)
                {
                    endTurn = game.Update(Program.updatePeriod);
                    Console.WriteLine(counter + ": update turn");
                    counter++;
                    if (counter > 1000)
                        break;
                }

                foreach (var unit in game.units)
                {
                    unit.UnSubscribe(game.events);
                }
                Console.WriteLine("Update end Move", game.move);
            }
        }
       
        public async static void MoveTriksterIllusion(Game game_data, Data.Unit unit_data, Map.Hex startHex, Map.Hex first_hex, Map.Hex second_hex)
        {
            int illusion1_id = await CreateUnitWithData(game_data.player1.id, unit_data);
            Data.Unit illusion1_data = new Data.Unit
            {
                id = illusion1_id,
                class_type = unit_data.class_type,
                unit_type = unit_data.unit_type,
                hex_column = unit_data.hex_column,
                hex_row = unit_data.hex_row,
                max_health = unit_data.max_health,
                current_health = unit_data.current_health,
                damage = unit_data.damage,
                attack_range = unit_data.attack_range,
                attack_speed = unit_data.attack_speed,
                rotation = unit_data.rotation,
                special_ability = unit_data.special_ability,
                abilities = unit_data.abilities

            };
            if (second_hex != null)
            {
                int illusion2_id = await CreateUnitWithData(game_data.player1.id, unit_data);
                Data.Unit illusion2_data = new Data.Unit
                {
                    id = illusion2_id,
                    class_type = unit_data.class_type,
                    unit_type = unit_data.unit_type,
                    hex_column = unit_data.hex_column,
                    hex_row = unit_data.hex_row,
                    max_health = unit_data.max_health,
                    current_health = unit_data.current_health,
                    damage = unit_data.damage,
                    attack_range = unit_data.attack_range,
                    attack_speed = unit_data.attack_speed,
                    rotation = unit_data.rotation,
                    special_ability = unit_data.special_ability,
                    abilities = unit_data.abilities
                };

                NetJesterSpecialAbility responess1 = new NetJesterSpecialAbility();
                responess1.xml_unit_data = await Data.Serialize<Data.Unit>(illusion2_data);
                responess1.to_hex = second_hex.HexToData();

                game_data.SendMessageToPlayers(responess1);
            }

            NetJesterSpecialAbility responess = new NetJesterSpecialAbility();
            responess.xml_unit_data = await Data.Serialize<Data.Unit>(illusion1_data);
            responess.to_hex = first_hex.HexToData();

            game_data.SendMessageToPlayers(responess);

        }
        public static int CreateJester(Player player, Data.Unit unit)
        {
                long LastInsertedId = -1;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    if (unit != null && unit.special_ability != null && unit.abilities != null)
                    {

                        string query = String.Format("INSERT INTO unit (match_id, account_id, server_unit_id, hex_column, hex_row, max_health, current_health, damage, attack_range, attack_speed, rotation_y) " +
                            "VALUES ({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10})",
                            player.match_id, player.player_data.account_id, unit.id, unit.hex_column, unit.hex_row, unit.max_health, unit.current_health, unit.damage, unit.attack_range, unit.attack_speed, unit.rotation);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            LastInsertedId = command.LastInsertedId;
                        }

                        if (LastInsertedId != -1)
                        {
                            foreach (var ability in unit.abilities)
                            {
                                query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name) " +
                            "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}')",
                            LastInsertedId, ability.globalID, ability.ability_type, ability.max_cooldown, ability.current_cooldown, ability.range, ability.quantity, ability.cc_time, ability.name);
                                using (MySqlCommand command = new MySqlCommand(query, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name, special) " +
                            "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}',{9})",
                            LastInsertedId, unit.special_ability.globalID, unit.special_ability.ability_type, unit.special_ability.max_cooldown, unit.special_ability.current_cooldown, unit.special_ability.range, unit.special_ability.quantity, unit.special_ability.cc_time, unit.special_ability.name, 1);
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        connection.Close();
                    }
                }
                return (int)LastInsertedId;
        }
        public async static Task<int> CreateUnitWithData(int clientID, Data.Unit unit)
        {
            Task<int> task = Task.Run(() =>
            {
                long LastInsertedId = -1;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    if (unit != null && unit.special_ability != null && unit.abilities != null)
                    {

                        string query = String.Format("INSERT INTO unit (match_id, account_id, server_unit_id, hex_column, hex_row, max_health, current_health, damage, attack_range, attack_speed, rotation_y) " +
                            "VALUES ({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10})",
                            Server.clients[clientID].player.match_id, Server.clients[clientID].player.player_data.account_id, unit.id, unit.hex_column, unit.hex_row, unit.max_health, unit.current_health, unit.damage, unit.attack_range, unit.attack_speed, unit.rotation);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            LastInsertedId = command.LastInsertedId;
                        }

                        if (LastInsertedId != -1)
                        {
                            foreach (var ability in unit.abilities)
                            {
                                query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name) " +
                            "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}')",
                            LastInsertedId, ability.globalID, ability.ability_type, ability.max_cooldown, ability.current_cooldown, ability.range, ability.quantity, ability.cc_time, ability.name);
                                using (MySqlCommand command = new MySqlCommand(query, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name, special) " +
                            "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}',{9})",
                            LastInsertedId, unit.special_ability.globalID, unit.special_ability.ability_type, unit.special_ability.max_cooldown, unit.special_ability.current_cooldown, unit.special_ability.range, unit.special_ability.quantity, unit.special_ability.cc_time, unit.special_ability.name, 1);
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        connection.Close();
                    }
                }
                return (int)LastInsertedId;
            });
            return await task;
        }
        public async static Task<int> CreateUnit(int clientID, Data.Unit unit)
        {
            Task<int> task = Task.Run(() =>
            {
                long LastInsertedId = -1;
                Data.ServerUnit server_unit = null;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT server_unit.unit_id, server_unit.max_health, server_unit.damage, server_unit.attack_range, " +
                        "server_unit.attack_speed, server_ability.server_ability_name, server_ability.global_id, server_ability.ability_type, server_ability.max_cooldown, " +
                        "server_ability.ability_range, server_ability.quantity, server_ability.cc_time " +
                        " FROM server_unit LEFT JOIN server_ability ON server_unit.unit_id = server_ability.server_unit_id " +
                        "WHERE unit_type = {0} AND unit_class = {1}; ", unit.unit_type, unit.class_type);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (server_unit == null)
                                        server_unit = new Data.ServerUnit();
                                    server_unit.unit_id = int.Parse(reader["unit_id"].ToString());
                                    server_unit.max_health = int.Parse(reader["max_health"].ToString());
                                    server_unit.damage = int.Parse(reader["damage"].ToString());
                                    server_unit.attack_range = int.Parse(reader["attack_range"].ToString());
                                    server_unit.attack_speed = float.Parse(reader["attack_speed"].ToString());

                                    Data.ServerAbility ability_data = new Data.ServerAbility();
                                    if (int.TryParse(reader["global_id"].ToString(), out ability_data.globalID))
                                    {
                                        ability_data.name = reader["server_ability_name"].ToString();
                                        ability_data.globalID = int.Parse(reader["global_id"].ToString());
                                        ability_data.ability_type = int.Parse(reader["ability_type"].ToString());
                                        ability_data.max_cooldown = int.Parse(reader["max_cooldown"].ToString());
                                        ability_data.range = int.Parse(reader["ability_range"].ToString());
                                        ability_data.quantity = int.Parse(reader["quantity"].ToString());
                                        ability_data.cc_time = int.Parse(reader["cc_time"].ToString());

                                        server_unit.server_abilities.Add(ability_data);
                                    }

                                }
                            }
                        }
                    }

                    if (server_unit != null)
                    {

                        query = String.Format("SELECT ability_type, max_cooldown, ability_range, quantity, cc_time, server_ability_name" +
                        " FROM server_special_ability WHERE server_unit_type = {0}; ", unit.unit_type);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Data.ServerAbility ability_data = new Data.ServerAbility();
                                        ability_data.name = reader["server_ability_name"].ToString();
                                        ability_data.globalID = unit.unit_type;
                                        ability_data.ability_type = int.Parse(reader["ability_type"].ToString());
                                        ability_data.max_cooldown = int.Parse(reader["max_cooldown"].ToString());
                                        ability_data.range = int.Parse(reader["ability_range"].ToString());
                                        ability_data.quantity = int.Parse(reader["quantity"].ToString());
                                        ability_data.cc_time = int.Parse(reader["cc_time"].ToString());
                                        server_unit.special_ability = ability_data;
                                    }
                                }
                            }
                        }

                        query = String.Format("INSERT INTO unit (match_id, account_id, server_unit_id, hex_column, hex_row, max_health, current_health, damage, attack_range, attack_speed, rotation_y) " +
                           "VALUES ({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10})",
                           Server.clients[clientID].player.match_id, Server.clients[clientID].player.player_data.account_id, server_unit.unit_id, unit.hex_column, unit.hex_row, server_unit.max_health, server_unit.max_health, server_unit.damage, server_unit.attack_range, server_unit.attack_speed, unit.rotation);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            LastInsertedId = command.LastInsertedId;
                        }

                        if (LastInsertedId != -1)
                        {
                            foreach (var server_ability in server_unit.server_abilities)
                            {
                                query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name) " +
                           "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}')",
                           LastInsertedId, server_ability.globalID, server_ability.ability_type, server_ability.max_cooldown, 0, server_ability.range, server_ability.quantity, server_ability.cc_time, server_ability.name);
                                using (MySqlCommand command = new MySqlCommand(query, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name, special) " +
                           "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}',{9})",
                           LastInsertedId, server_unit.special_ability.globalID, server_unit.special_ability.ability_type, server_unit.special_ability.max_cooldown, 0, server_unit.special_ability.range, server_unit.special_ability.quantity, server_unit.special_ability.cc_time, server_unit.special_ability.name, 1);
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        connection.Close();
                    }

                }
                return 1;
            });
            return await task;
        }
        public static void DestroyUnit(Unit unit)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("DELETE FROM ability WHERE unit_id = {0};", unit.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    query = String.Format("DELETE FROM unit WHERE unit_id = {0};", unit.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateUnitPosition(Unit unit)
        {
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("UPDATE unit SET hex_column = {0}, hex_row = {1}, rotation_y = {2} " +
                    "WHERE unit_id = {3};"
                    , unit.column, unit.row, unit.rotationAngle, unit.id);
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public static void UpdateUnitStats(Unit unit)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE unit SET max_health = {0}, current_health = {1}, damage = {2}, attack_range = {3}, attack_speed = {4} " +
                        "WHERE unit_id = {5};"
                        , unit.stats.MaxHealth, unit.stats.CurrentHealth, unit.stats.Damage, unit.stats.AttackRange, unit.stats.AttackSpeed, unit.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateUnitHealth(Unit unit)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE unit SET current_health = {0} " +
                        "WHERE unit_id = {1};"
                        , unit.stats.CurrentHealth, unit.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateUnitRotation(Unit unit)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE unit SET rotation_y = {0} " +
                        "WHERE unit_id = {1};"
                        , unit.rotationAngle, unit.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateUnitAbility(Ability ability, bool reduce = false)
        {
            int ability_cooldown = reduce == false ? ability.max_cooldown : ability.current_cooldown;
           
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE ability SET current_cooldown = {0} " +
                        "WHERE ability_id = {1};"
                        , ability_cooldown, ability.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void CreateModifiedHex(Map.Hex hex ,Map.Modified_Hex modified_hex)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("INSERT INTO modified_hex (hex_id, type, not_visible_for_class, ability_id)" +
                        " VALUES({0}, {1}, {2}, {3})", 
                        hex.id, (int)modified_hex.type, (int)modified_hex.not_visible_for_the_class_type, modified_hex.ability_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void DestroyModifiedHex(Map.Modified_Hex modified_hex)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("DELETE FROM modified_hex WHERE id_modified_hex  = {0};", modified_hex.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void CreateCC(Game game, CC cc)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("INSERT INTO cc (match_id, from_ability_id, on_unit_id, cc_type, cc_max_cooldown, cc_current_cooldown) VALUES({0}, {1}, {2}, {3}, {4}, {5})", game.game_id, cc.cast_from_ability_id, cc.unit.id, (int)cc.type, cc.max_cooldown, cc.current_cooldown);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateCC(CC cc)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE cc SET cc_current_cooldown = {0} " +
                        "WHERE cc_id = {1};"
                        , cc.current_cooldown, cc.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void DestroyCC(CC cc)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("DELETE FROM cc WHERE cc_id = {0};", cc.id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateHex(Game game, Map.Hex hex)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE hex SET walkable = {0} WHERE game_id = {1} AND hex_column = {2} AND hex_row = {3};",Convert.ToInt16(hex.walkable), game.game_id, hex.column, hex.row);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateChallengeRoyalMove(Game game)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE game SET challenge_royal_move = {0} " +
                        "WHERE game_id = {1};", game.challenge_royal_move, game.game_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateOnEndOfTheTurn(Game game)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE game SET class_on_turn = {0}, move = {1} " +
                        "WHERE game_id = {2};", (int)game.class_on_turn, game.move, game.game_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        public static void UpdateWinner(Game game)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE game SET win_account = {0} " +
                        "WHERE game_id = {1};", game.win, game.game_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }

        public static void UpdatePlayerMMR(Player player)
        {
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("UPDATE accounts SET rank = {0} " +
                    "WHERE account_id = {1};", player.player_data.rank, player.player_data.account_id);
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public async static void SendWinner(int clientID)
        {
            NetEndGame responess = new NetEndGame();
            Game game = await GetMatchDataAsync(clientID);
            if (game.player1 != null && game.player1.player_data.account_id == game.win)
                responess.class_win = game.player1.player_data.class_type;
            else
                if(game.player2 != null && game.player2.player_data.account_id == game.win)
                responess.class_win = game.player2.player_data.class_type;


            responess.ip = Tools.GetIP(AddressFamily.InterNetwork);
            responess.port = 27000;
            Sender.TCP_SendToClient(clientID, responess);
        }

        public async static void UpdateWinnerAndSendToClients(int clientID)
        {
            Game game_data = await GetMatchDataAsync(clientID);
            game_data.OnKingDeath(Server.clients[clientID].player.player_data.class_type);
            SendWinner(clientID);
            if (game_data.player2 != null)
                SendWinner(game_data.player2.id);
        }
        private async static Task<int> GetWinner(int matchID)
        {
            Task<int> task = Task.Run(() =>
            {
                int win_account = 0;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT win_account FROM game WHERE game_id = {0};", matchID);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int.TryParse(reader["win_account"].ToString(), out win_account);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return win_account;
            });
            return await task;
        }
        public static void ActivateChallengeRoyal(Game game)
        {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE game SET challenge_royal_activated = {0} " +
                        "WHERE game_id = {1};", 1, game.game_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
        }
        private async static Task<List<Data.Hex>> GetHexesAsync(int clientID)
        {
            Task<List<Data.Hex>> task = Task.Run(() =>
            {
                List<Data.Hex> data = new List<Data.Hex>();
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT hex.hex_id, hex.hex_column, hex.hex_row, hex.walkable, modified_hex.id_modified_hex, modified_hex.ability_id, modified_hex.type, modified_hex.not_visible_for_class" +
                        " FROM hex LEFT JOIN modified_hex ON hex.hex_id = modified_hex.hex_id " +
                        " WHERE game_id = {0};", Server.clients[clientID].player.match_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Data.Hex hex = null;
                                    int id = int.Parse(reader["hex_id"].ToString());
                                    foreach (var h in data)
                                    {
                                        if (h.id == id)
                                        {
                                            hex = h;
                                            break;
                                        }
                                    }
                                    if(hex == null)
                                    {
                                        hex = new Data.Hex();
                                        hex.id = id;
                                        hex.column = int.Parse(reader["hex_column"].ToString());
                                        hex.row = int.Parse(reader["hex_row"].ToString());
                                        hex.walkable = int.Parse(reader["walkable"].ToString());
                                        data.Add(hex);
                                    }
                                    Data.Modified_Hex modified_hex = new Data.Modified_Hex();
                                    if (int.TryParse(reader["id_modified_hex"].ToString(), out modified_hex.id))
                                    {
                                        modified_hex.id = int.Parse(reader["id_modified_hex"].ToString());
                                        modified_hex.ability_id = int.Parse(reader["ability_id"].ToString());
                                        modified_hex.type = int.Parse(reader["type"].ToString());
                                        modified_hex.not_visible_for_the_class_type = int.Parse(reader["not_visible_for_class"].ToString());
                                        hex.modified.Add(modified_hex);
                                    }
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return data;
            });
            return await task;
        }
        private async static Task<List<Data.Unit>> GetAllUnitsFromMatchAsync(int clientID)
        {
            Task<List<Data.Unit>> task = Task.Run(() =>
            {
                List<Data.Unit> data = new List<Data.Unit>();
                using (MySqlConnection connection = GetMysqlConnection())
                {

                    string query = String.Format("SELECT server_unit.unit_type, server_unit.unit_class, unit.unit_id, unit.hex_column, unit.hex_row, unit.max_health, unit.current_health, unit.damage, unit.attack_range, unit.attack_speed, unit.rotation_y," +
                        "ability.ability_id ,ability.ability_name, ability.global_id, ability.ability_type, ability.special, ability.max_cooldown, ability.current_cooldown, ability.ability_range, ability.quantity, ability.cc_time, " +
                        "cc.cc_id, cc.from_ability_id, cc.cc_type, cc.cc_max_cooldown, cc.cc_current_cooldown "
                + "FROM unit " +
                "LEFT JOIN server_unit ON unit.server_unit_id = server_unit.unit_id " +
                "LEFT JOIN ability ON ability.unit_id = unit.unit_id " +
                "LEFT JOIN cc ON cc.on_unit_id = unit.unit_id " +
                "WHERE unit.match_id = {0};", Server.clients[clientID].player.match_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Data.Unit unit = null;
                                    int id = int.Parse(reader["unit_id"].ToString());
                                    foreach (var u in data)
                                    {
                                        if (u.id == id)
                                        {
                                            unit = u;
                                            break;
                                        }
                                    }
                                    if (unit == null)
                                    {
                                        unit = new Data.Unit();
                                        unit.id = int.Parse(reader["unit_id"].ToString());
                                        unit.unit_type = int.Parse(reader["unit_type"].ToString());
                                        unit.class_type = int.Parse(reader["unit_class"].ToString());
                                        unit.hex_column = int.Parse(reader["hex_column"].ToString());
                                        unit.hex_row = int.Parse(reader["hex_row"].ToString());
                                        unit.max_health = int.Parse(reader["max_health"].ToString());
                                        unit.current_health = int.Parse(reader["current_health"].ToString());
                                        unit.damage = int.Parse(reader["damage"].ToString());
                                        unit.attack_range = int.Parse(reader["attack_range"].ToString());
                                        unit.attack_speed = float.Parse(reader["attack_speed"].ToString());
                                        unit.rotation = float.Parse(reader["rotation_y"].ToString());
                                        data.Add(unit);
                                    }

                                    Data.Ability ability_data = new Data.Ability();
                                    if (int.TryParse(reader["global_id"].ToString(), out ability_data.globalID))
                                    {
                                        int special = int.Parse(reader["special"].ToString());
                                        ability_data.id = int.Parse(reader["ability_id"].ToString());
                                        ability_data.name = reader["ability_name"].ToString();
                                        ability_data.globalID = int.Parse(reader["global_id"].ToString());
                                        ability_data.ability_type = int.Parse(reader["ability_type"].ToString());
                                        ability_data.max_cooldown = int.Parse(reader["max_cooldown"].ToString());
                                        ability_data.current_cooldown = int.Parse(reader["current_cooldown"].ToString());
                                        ability_data.range = int.Parse(reader["ability_range"].ToString());
                                        ability_data.quantity = int.Parse(reader["quantity"].ToString());
                                        ability_data.cc_time = int.Parse(reader["cc_time"].ToString());

                                        if (special == 0)
                                            unit.abilities.Add(ability_data);
                                        else
                                            unit.special_ability = ability_data;
                                    }
                                    Data.CC cc_data = new Data.CC();
                                    if (int.TryParse(reader["cc_id"].ToString(), out cc_data.id))
                                    {
                                        cc_data.id = int.Parse(reader["cc_id"].ToString());
                                        cc_data.cast_ability_id = int.Parse(reader["from_ability_id"].ToString());
                                        cc_data.cc_type = int.Parse(reader["cc_type"].ToString());
                                        cc_data.max_cooldown = int.Parse(reader["cc_max_cooldown"].ToString());
                                        cc_data.current_cooldown = int.Parse(reader["cc_current_cooldown"].ToString());
                                        unit.cc.Add(cc_data);
                                    }
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return data;
            });
            return await task;
        }
        public async static void OnPlayerReconnect(int clinedID)
        {
            Player player = Server.clients[clinedID].player;
            int enemy_account = await GetOpponentAccount((int)player.player_data.account_id, player.match_id);
            Player enemy = null;

            foreach (var client in Server.clients)
            {
                if (client.Value.player != null && client.Value.player.player_data.account_id == enemy_account)
                {
                    enemy = client.Value.player;
                    break;
                }
            }

            if (enemy != null)
            {
                NetReconnect responess = new NetReconnect();
                Sender.TCP_SendToClient(player.id, responess);
                Sender.TCP_SendToClient(enemy.id, responess);
            }
        }

        public async static void OnPlayerDisconnect(string nickname, long accountID, int matchID)
        {
            int win_account = await GetWinner(matchID);
            if(win_account == 0)
            {
                int enemy_account = await GetOpponentAccount((int)accountID, matchID);
                Player enemy = null;
                foreach (var client in Server.clients)
                {
                    if (client.Value.player != null && client.Value.player.player_data.account_id == enemy_account)
                    {
                        enemy = client.Value.player;
                        break;
                    }
                }

                if (enemy != null)
                {
                    NetDisconnect responess = new NetDisconnect();
                    responess.nickname = nickname;
                    Sender.TCP_SendToClient(enemy.id, responess);
                }

            }
        }
        private async static Task<int> GetOpponentAccount(int accountID, int matchID)
        {
            Task<int> task = Task.Run(() =>
            {
                int opponent_account = 0;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT account_one, account_two from game WHERE game_id = {0};", matchID);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int account_one = int.Parse(reader["account_one"].ToString());
                                    int account_two = int.Parse(reader["account_two"].ToString());

                                    if (accountID == account_one)
                                        opponent_account = account_two;
                                    else
                                        opponent_account = account_one;
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return opponent_account;
            });
            return await task;
        }
        private async static Task<Game> GetMatchDataAsync(int clientID)
        {
            Task<Game> task = Task.Run(() =>
            {
                Game game = new Game();
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT account_one, account_two, class_on_turn, move, challenge_royal_move, challenge_royal_activated, win_account FROM game WHERE game_id = {0};", Server.clients[clientID].player.match_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    game.game_id = Server.clients[clientID].player.match_id;
                                    game.player1 = Server.clients[clientID].player;
                                    game.class_on_turn = (ClassType)int.Parse(reader["class_on_turn"].ToString());
                                    game.move = int.Parse(reader["move"].ToString());

                                    game.challenge_royal_activated = Convert.ToBoolean(int.Parse(reader["challenge_royal_activated"].ToString()));
                                    game.challenge_royal_move = int.Parse(reader["challenge_royal_move"].ToString());
                                    int.TryParse(reader["win_account"].ToString(), out game.win);

                                    int account1 = int.Parse(reader["account_one"].ToString());
                                    int account2 = int.Parse(reader["account_two"].ToString());


                                    if (game.player1.player_data.account_id == account1)
                                    {
                                        foreach (var client in Server.clients)
                                        {
                                            if (client.Value.player != null && client.Value.player.player_data.account_id == account2)
                                            {
                                                game.player2 = client.Value.player;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var client in Server.clients)
                                        {
                                            if (client.Value.player != null && client.Value.player.player_data.account_id == account1)
                                            {
                                                game.player2 = client.Value.player;
                                                break;
                                            }
                                        }
                                    }


                                }
                            }
                        }
                    }
                    connection.Close();
                }
                return game;
            });
            return await task;
        }
        #endregion

        #region Test Functions
        //TEST FUNCTIONS
        private static void CreateMap()
        {
            int N = 4;
            for (int c = -N; c <= N; c++)
            {
                int r1 = Math.Max(-N, -c - N);
                int r2 = Math.Min(N, -c + N);
                for (int r = r1; r <= r2; r++)
                {
                    Data.Hex hex = new Data.Hex
                    {
                        column = c,
                        row = r,
                        walkable = 1
                    };
                    CreateHexAsync(hex, 0);
                }
            }
        }
        private async static void CreateHexAsync(Data.Hex hex, int matchID)
        {
            Task task = Task.Run(() =>
            {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("INSERT INTO hex (game_id, hex_column, hex_row) VALUES({0}, {1}, {2})", matchID, hex.column, hex.row, hex.walkable);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            });
            await task;
        }
        private static void CreateTeam1(Data.Game game_data, ClassType classType)
        {
            //king
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.King,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = -4,
                rotation = 0
            });
            //queen
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Queen,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = -3,
                rotation = 0
            });
            //jester
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Jester,
                class_type = (int)classType,
                hex_column = -2,
                hex_row = -2,
                rotation = 0
            });
            //wizzard
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Wizzard,
                class_type = (int)classType,
                hex_column = 2,
                hex_row = -4,
                rotation = 0
            });
            //tank
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Tank,
                class_type = (int)classType,
                hex_column = -1,
                hex_row = -3,
                rotation = 0
            });
            //tank
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Tank,
                class_type = (int)classType,
                hex_column = 1,
                hex_row = -4,
                rotation = 0
            });
            //archer
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Archer,
                class_type = (int)classType,
                hex_column = 3,
                hex_row = -4,
                rotation = 0
            });
            //archer
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Archer,
                class_type = (int)classType,
                hex_column = -3,
                hex_row = -1,
                rotation = 0
            });
            //knight
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Knight,
                class_type = (int)classType,
                hex_column = 1,
                hex_row = -3,
                rotation = 0
            });
            //knight
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Knight,
                class_type = (int)classType,
                hex_column = -1,
                hex_row = -2,
                rotation = 0
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = -2,
                hex_row = -1,
                rotation = 0
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = 2,
                hex_row = -3,
                rotation = 0
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = -2,
                rotation = 0
            });
        }
        private static void CreateTeam2(Data.Game game_data, ClassType classType)
        {
            //king
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.King,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = 4,
                rotation = 180
            });
            //queen
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Queen,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = 3,
                rotation = 180
            });
            //jester
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Jester,
                class_type = (int)classType,
                hex_column = 2,
                hex_row = 2,
                rotation = 180
            });
            //wizzard
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Wizzard,
                class_type = (int)classType,
                hex_column = -2,
                hex_row = 4,
                rotation = 180
            });
            //tank
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Tank,
                class_type = (int)classType,
                hex_column = 1,
                hex_row = 3,
                rotation = 180
            });
            //tank
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Tank,
                class_type = (int)classType,
                hex_column = -1,
                hex_row = 4,
                rotation = 180
            });
            //archer
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Archer,
                class_type = (int)classType,
                hex_column = -3,
                hex_row = 4,
                rotation = 180
            });
            //archer
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Archer,
                class_type = (int)classType,
                hex_column = 3,
                hex_row = 1,
                rotation = 180
            });
            //knight
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Knight,
                class_type = (int)classType,
                hex_column = -1,
                hex_row = 3,
                rotation = 180
            });
            //knight
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Knight,
                class_type = (int)classType,
                hex_column = 1,
                hex_row = 2,
                rotation = 180
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = 2,
                hex_row = 1,
                rotation = 180
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = -2,
                hex_row = 3,
                rotation = 180
            });
            //swordsman
            game_data.units_data.Add(new Data.Unit
            {
                unit_type = (int)UnitType.Swordsman,
                class_type = (int)classType,
                hex_column = 0,
                hex_row = 2,
                rotation = 180
            });
        }

        public static void TestFunction()
        {
            Data.Game player1 = new Data.Game();
            Data.Game player2 = new Data.Game();

            CreateMap();
            CreateTeam1(player1, ClassType.Light);
            CreateTeam2(player2, ClassType.Dark);

            foreach (var unit in player1.units_data)
            {
                CreateUnitTest(2, unit);
                Data.Hex hex = new Data.Hex
                {
                    column = unit.hex_column,
                    row = unit.hex_row,
                    walkable = 0
                };
                UpdateHexTest(2, hex);
            }

            foreach (var unit in player2.units_data)
            {
                CreateUnitTest(3, unit);
                Data.Hex hex = new Data.Hex
                {
                    column = unit.hex_column,
                    row = unit.hex_row,
                    walkable = 0
                };
                UpdateHexTest(3, hex);
            }
        }
        public async static void CreateUnitTest(int clientID, Data.Unit unit)
        {
            Task task = Task.Run(() =>
            {
                Data.ServerUnit server_unit = null;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT server_unit.unit_id, server_unit.max_health, server_unit.damage, server_unit.attack_range, " +
                        "server_unit.attack_speed, server_ability.server_ability_name, server_ability.global_id, server_ability.ability_type, server_ability.max_cooldown, " +
                        "server_ability.ability_range, server_ability.quantity, server_ability.cc_time " +
                        "FROM server_unit LEFT JOIN server_ability ON server_unit.unit_id = server_ability.server_unit_id " +
                        "WHERE unit_type = {0} AND unit_class = {1}; ", unit.unit_type, unit.class_type);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (server_unit == null)
                                        server_unit = new Data.ServerUnit();
                                    server_unit.unit_id = int.Parse(reader["unit_id"].ToString());
                                    server_unit.max_health = int.Parse(reader["max_health"].ToString());
                                    server_unit.damage = int.Parse(reader["damage"].ToString());
                                    server_unit.attack_range = int.Parse(reader["attack_range"].ToString());
                                    server_unit.attack_speed = float.Parse(reader["attack_speed"].ToString());

                                    Data.ServerAbility ability_data = new Data.ServerAbility();
                                    if (int.TryParse(reader["global_id"].ToString(), out ability_data.globalID))
                                    {
                                        ability_data.name = reader["server_ability_name"].ToString();
                                        ability_data.globalID = int.Parse(reader["global_id"].ToString());
                                        ability_data.ability_type = int.Parse(reader["ability_type"].ToString());
                                        ability_data.max_cooldown = int.Parse(reader["max_cooldown"].ToString());
                                        ability_data.range = int.Parse(reader["ability_range"].ToString());
                                        ability_data.quantity = int.Parse(reader["quantity"].ToString());
                                        ability_data.cc_time = int.Parse(reader["cc_time"].ToString());

                                        server_unit.server_abilities.Add(ability_data);
                                    }

                                }
                            }
                        }
                    }

                    if (server_unit != null)
                    {

                        query = String.Format("SELECT ability_type, max_cooldown, ability_range, quantity, cc_time, server_ability_name" +
                        " FROM server_special_ability WHERE server_unit_type = {0}; ", unit.unit_type);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Data.ServerAbility ability_data = new Data.ServerAbility();
                                        ability_data.name = reader["server_ability_name"].ToString();
                                        ability_data.globalID = unit.unit_type;
                                        ability_data.ability_type = int.Parse(reader["ability_type"].ToString());
                                        ability_data.max_cooldown = int.Parse(reader["max_cooldown"].ToString());
                                        ability_data.range = int.Parse(reader["ability_range"].ToString());
                                        ability_data.quantity = int.Parse(reader["quantity"].ToString());
                                        ability_data.cc_time = int.Parse(reader["cc_time"].ToString());
                                        server_unit.special_ability = ability_data;
                                    }
                                }
                            }
                        }

                        long LastInsertedId = -1;
                        query = String.Format("INSERT INTO unit (match_id, account_id, server_unit_id, hex_column, hex_row, max_health, current_health, damage, attack_range, attack_speed, rotation_y) " +
                           "VALUES ({0},{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10})",
                           0, clientID, server_unit.unit_id, unit.hex_column, unit.hex_row, server_unit.max_health, server_unit.max_health, server_unit.damage, server_unit.attack_range, server_unit.attack_speed, unit.rotation);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            LastInsertedId = command.LastInsertedId;
                        }

                        if (LastInsertedId != -1)
                        {
                            foreach (var server_ability in server_unit.server_abilities)
                            {
                                query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name) " +
                           "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}')",
                           LastInsertedId, server_ability.globalID, server_ability.ability_type, server_ability.max_cooldown, 0, server_ability.range, server_ability.quantity, server_ability.cc_time, server_ability.name);
                                using (MySqlCommand command = new MySqlCommand(query, connection))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            query = String.Format("INSERT INTO ability (unit_id, global_id, ability_type, max_cooldown, current_cooldown, ability_range, quantity, cc_time, ability_name, special) " +
                           "VALUES ({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}',{9})",
                           LastInsertedId, server_unit.special_ability.globalID, server_unit.special_ability.ability_type, server_unit.special_ability.max_cooldown, 0, server_unit.special_ability.range, server_unit.special_ability.quantity, server_unit.special_ability.cc_time, server_unit.special_ability.name, 1);
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.ExecuteNonQuery();
                            }

                        }
                        connection.Close();
                    }

                }
            });
            await task;
        }
        public async static void UpdateHexTest(int clientID, Data.Hex hex)
        {// kada klijent zahteva update proveriti sa serverom da li su sync podaci
            Task task = Task.Run(() =>
            {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE hex SET walkable = {0} WHERE game_id = {1} AND hex_column = {2} AND hex_row = {3};", hex.walkable, 0, hex.column, hex.row);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return true;
            });
            await task;
        }
        #endregion
    }
}
