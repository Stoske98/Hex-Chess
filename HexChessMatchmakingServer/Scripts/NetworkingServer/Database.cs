using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Threading.Tasks;

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

        public async static void AuthenticatePlayer(int clientID, string deviceID)
        {
            Data.Player player_data = await AuthenticatePlayerAsync(clientID,deviceID);

              NetAuthentication responess = new NetAuthentication
              {
                  player = player_data
              };
              Sender.TCP_SendToClient(clientID, responess);  
        }

        public async static void CreateMatch(Matchmaking.Match match)
        {
            int matchID = await CreateMatchAsync(match.player1, match.player2);

            List<Data.Hex> hexes = CreateMap();

            Data.Game team1_structure = await Data.Deserialize<Data.Game>(match.xml_team1_structure);
            Data.Game team2_structure = await Data.Deserialize<Data.Game>(match.xml_team2_structure);

            foreach (Data.Unit unit in team1_structure.units_data)
            {
                foreach (Data.Hex hex in hexes)
                {
                    if (unit.hex_column == hex.column && unit.hex_row == hex.row)
                    {
                        hex.walkable = 0;
                        break;
                    }
                }
                await CreateUnitAsync(match.player1,unit,matchID);
            }
            foreach (Data.Unit unit in team2_structure.units_data)
            {
                foreach (Data.Hex hex in hexes)
                {
                    if (unit.hex_column == hex.column && unit.hex_row == hex.row)
                    {
                        hex.walkable = 0;
                        break;
                    }
                }
                await CreateUnitAsync(match.player2, unit, matchID);
            }
            foreach (Data.Hex hex in hexes)
                await CreateHexAsync(hex,matchID);

            await UpdatePlayerClassType(match.player1);
            await UpdatePlayerClassType(match.player2);

            NetMatchCreated responess = new NetMatchCreated();
            responess.match_id = matchID;
            responess.ip = Tools.GetIP(AddressFamily.InterNetwork);
            responess.port = 27001;

            Matchmaking.Matchmaking.matches[match.player1.match_id] = null;
            match.player1.match_id = -1;
            match.player2.match_id = -1;

            Sender.TCP_SendToClient(match.player1.id, responess);
            Sender.TCP_SendToClient(match.player2.id, responess);   

        }

        public async static void ChangePlayerNickname(string nickname, int clientID)
        {
            nickname = await UpdatePlayerNickname(nickname, clientID);
            Server.clients[clientID].player.data.nickname = nickname;

            NetChangeNickname responess = new NetChangeNickname();
            responess.nickname = nickname;
            Sender.TCP_SendToClient(clientID, responess);
        }

        public async static void IsGameExist(int clientID)
        {
            int game_id = await GetGameId(clientID);

            NetSynchronic responess = new NetSynchronic();
            responess.match_id = game_id;
            responess.ip = Tools.GetIP(AddressFamily.InterNetwork);
            responess.port = 27001;

            Sender.TCP_SendToClient(clientID,responess);

        }
        public async static Task<string> UpdatePlayerNickname(string nickname, int clientID)
        {
            Task<string> task = Task.Run(() =>
            {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE accounts SET nickname = '{0}' " +
                        "WHERE account_id = {1};"
                        , nickname, Server.clients[clientID].player.data.account_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return nickname;
            });
            return await task;
        }
        public async static Task<int> GetGameId(int clientID)
        {
            Task<int> task = Task.Run(() => {

                int game_id = 0;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT game_id FROM game WHERE (account_one = {0} OR account_two = {0}) AND win_account = 0", Server.clients[clientID].player.data.account_id);
                    
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    game_id = int.Parse(reader["game_id"].ToString());
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                
                    return game_id;
            });
            return await task;
        }

        private async static Task<int> CreateMatchAsync(Player player1, Player player2)
        {
            Task<int> task = Task.Run(() =>
            {
                long LastInsertedId = -1;
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("INSERT INTO game (account_one, account_two) VALUES({0}, {1})", player1.data.account_id, player2.data.account_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        LastInsertedId = command.LastInsertedId;
                    }
                    connection.Close();
                }
                return (int)LastInsertedId;
            });
            return await task;
        }
        private async static Task<Data.Player> AuthenticatePlayerAsync(int clientID, string deviceID)
        {
            Task<Data.Player> task = Task.Run(() => {

                Data.Player data = new Data.Player();

                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("SELECT account_id, nickname, rank, selected_class FROM accounts WHERE device_id = '{0}'", deviceID);
                    bool account_found = false;
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
                                    data.class_type = (Matchmaking.ClassType)int.Parse(reader["selected_class"].ToString());
                                    account_found = true;
                                }
                            }
                        }
                    }
                    if (!account_found)
                    {
                        query = String.Format("INSERT INTO accounts (device_id, nickname, selected_class) VALUES('{0}', '{1}', {2})", deviceID, deviceID.Substring(0, 20), (int)Matchmaking.ClassType.Light);
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            data.account_id = command.LastInsertedId;
                            data.nickname = deviceID.Substring(0, 20);
                            data.class_type = Matchmaking.ClassType.Light;
                            data.rank = 500;
                        }
                    }
                    connection.Close();
                }
                Server.clients[clientID].player = new Player(clientID, deviceID, data);
                return data;
            });
            return await task;
        }
        public async static Task<int> UpdatePlayerClassType(Player player)
        {
            Task<int> task = Task.Run(() =>
            {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("UPDATE accounts SET selected_class = {0} " +
                        "WHERE account_id = {1};"
                        , (int)player.data.class_type, player.data.account_id);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return 1;
            });
            return await task;
        }
        private async static Task<int> CreateHexAsync(Data.Hex hex, int matchID)
        {
            Task<int> task = Task.Run(() =>
            {
                using (MySqlConnection connection = GetMysqlConnection())
                {
                    string query = String.Format("INSERT INTO hex (game_id, hex_column, hex_row, walkable) VALUES({0}, {1}, {2}, {3})", matchID, hex.column, hex.row, hex.walkable);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                return 1;
            });
            return await task;
        }
        public async static Task<int> CreateUnitAsync(Player player, Data.Unit unit, int matchID)
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
                           matchID, player.data.account_id, server_unit.unit_id, unit.hex_column, unit.hex_row, server_unit.max_health, server_unit.max_health, server_unit.damage, server_unit.attack_range, server_unit.attack_speed, unit.rotation);
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
        private static List<Data.Hex> CreateMap()
        {
            List<Data.Hex> hexes = new List<Data.Hex>();
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
                    hexes.Add(hex);
                }
            }
            return hexes;
        }
        #endregion

    }
}
