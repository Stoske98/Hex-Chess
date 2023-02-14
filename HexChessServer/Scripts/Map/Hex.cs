using Networking.Server;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Map { 
        public enum Direction
        {
        Top = 0,
        Bottom = 1,
        Top_Left = 2,
        Top_Right = 3,
        Bottom_Left = 4,
        Bottom_Right = 5
        }

        public enum Modifier
        {
            Trap = 1,
        }

        public class Modified_Hex
        {
            public int id;
            public int ability_id;
            public Modifier type;
            public Units.ClassType not_visible_for_the_class_type;

            public Modified_Hex(Data.Modified_Hex modified_hex)
            {
                id = modified_hex.id;
                ability_id = modified_hex.ability_id;
                type = (Modifier)modified_hex.type;
                not_visible_for_the_class_type = (Units.ClassType)modified_hex.not_visible_for_the_class_type;
            }

            public Modified_Hex() { }
        }
    public class Hex 
    {
        public int id { set; get; }
        public int column { set; get; }
        public int row { set; get; }
        public Vector2 position { set; get; }
        public List<Hex> neighbors { set; get; }
        public bool walkable { set; get; }
        public int weight { get; set; }
        public int cost { get; set; }
        public Hex prevTile { get; set; }

        public List<Modified_Hex> modified_hexes = new List<Modified_Hex>();

        public Hex(Data.Hex hex)
        {
            id = hex.id;
            column = hex.column;
            row = hex.row;
            position = Position(column, row);
            neighbors = new List<Hex>();
            walkable = hex.walkable == 1 ? true : false;
            weight = 1;
            foreach (Data.Modified_Hex modified_hex_data in hex.modified)
            {
                modified_hexes.Add(new Modified_Hex(modified_hex_data));
            }
        }

        public Data.Hex HexToData()
        {
            Data.Hex hex = new Data.Hex();
            hex.id = id;
            hex.column = column;
            hex.row = row;
            hex.walkable = walkable == true ? 1 : 0;
            return hex;
        }

        private Vector2 Position(int c, int r)
        {
            float x = 1.1f * 3.0f / 2.0f * c * 1.06f;
            float z = (float)(1.1f * Math.Sqrt(3.0f) * (r + c / 2.0f) * 1.06f);
            return new Vector2(x, z);
        }
        public bool isHexContainTrap()
        {
            foreach (var modified_hex in modified_hexes)
                if (modified_hex.type == Modifier.Trap)
                    return true;
            return false;
        }
        public Units.ClassType ClassThatActivateTrap()
        {
            foreach (var modified_hex in modified_hexes)
                if (modified_hex.type == Modifier.Trap)
                    return modified_hex.not_visible_for_the_class_type;
            return Units.ClassType.None;
        }

        public Modified_Hex GetTrapModifier()
        {
            foreach (var modified_hex in modified_hexes)
                if (modified_hex.type == Modifier.Trap)
                    return modified_hex;
            return null;
        }
    }

}