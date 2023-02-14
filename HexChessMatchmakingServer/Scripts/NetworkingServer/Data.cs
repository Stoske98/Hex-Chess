using System.Xml.Serialization;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Networking.Server
{
    public static class Data
    {
        public class Player
        {
            public long account_id;
            public string nickname;
            public int rank;
            public Matchmaking.ClassType class_type;
        }

        public class Game
        {
            public int class_on_turn;
            public int move;
            public int challenge_royal_activated;
            public int challenge_royal_move;
            public List<Unit> units_data = new List<Unit>();
            public List<Hex> hexes_data = new List<Hex>();
        }

        public class Hex
        {
            public int column;
            public int row;
            public int walkable;
        }

        public class Unit
        {
            public int id;
            public int unit_type;
            public int class_type;
            public int hex_column;
            public int hex_row;
            public int max_health;
            public int current_health;
            public int damage;
            public int attack_range;
            public float attack_speed;
            public float rotation;
            public Ability special_ability;
            public List<Ability> abilities = new List<Ability>();
            public List<CC> cc = new List<CC>();
        }

        public class ServerUnit
        {
            public int unit_id;
            public int unit_type;
            public int class_type;
            public int max_health;
            public int damage;
            public int attack_range;
            public float attack_speed;
            public ServerAbility special_ability;
            public List<ServerAbility> server_abilities = new List<ServerAbility>();

        }

        public class Ability
        {
            public int id;
            public int globalID;
            public int ability_type;
            public string name;
            public int max_cooldown;
            public int current_cooldown;
            public int quantity;
            public int cc_time;
            public int range;
        }

        public class ServerAbility
        {
            public int globalID;
            public int ability_type;
            public string name;
            public int max_cooldown;
            public int quantity;
            public int cc_time;
            public int range;
        }

        public class CC
        {
            public int id;
            public int cast_ability_id;
            public int cc_type;
            public int max_cooldown;
            public int current_cooldown;
        }
        public async static Task<string> Serialize<T>(this T target)
        {
            Task<string> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringWriter writer = new StringWriter();
                xml.Serialize(writer, target);
                return writer.ToString();
            });
            return await task;
        }

        public async static Task<T> Deserialize<T>(this string target)
        {
            Task<T> task = Task.Run(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(target);
                return (T)xml.Deserialize(reader);
            });
            return await task;
        }

        public static string Serialize_Test<T>(this T target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }

        public static T Deserialize_Test<T>(this string target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(target);
            return (T)xml.Deserialize(reader);
        }
    }
}

