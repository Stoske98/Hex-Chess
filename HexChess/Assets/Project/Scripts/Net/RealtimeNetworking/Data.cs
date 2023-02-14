using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Networking.Client
{
    public static class Data
    {

        public class Player
        {
            public long account_id;
            public string nickname;
            public int rank;
            public ClassType class_type;
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
            public int id;
            public int column;
            public int row;
            public int walkable;
            public List<Modified_Hex> modified = new List<Modified_Hex>();
        }
        public class Modified_Hex
        {
            public int id;
            public int hex_id;
            public int ability_id;
            public int type;
            public int not_visible_for_the_class_type;
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

        public class CC
        {
            public int id;
            public int cast_ability_id;
            public int cc_type;
            public int max_cooldown;
            public int current_cooldown;
        }

        public static string Serialize<T>(this T target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringWriter writer = new StringWriter();
            xml.Serialize(writer, target);
            return writer.ToString();
        }

        public static T Deserialize<T>(this string target)
        {
            XmlSerializer xml = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(target);
            return (T)xml.Deserialize(reader);
        }


    }

}