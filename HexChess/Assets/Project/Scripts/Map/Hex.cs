using Networking.Client;
using System.Collections.Generic;
using UnityEngine;
public enum Modifier
{
    Trap = 1,
}
public class Modified_Hex
{
    public int ability_id;
    public Modifier type;
    public ClassType not_visible_for_the_class_type;
    public GameObject GameObject;

    public Modified_Hex(Data.Modified_Hex modified_hex)
    {
        ability_id = modified_hex.ability_id;
        type = (Modifier)modified_hex.type;
        not_visible_for_the_class_type = (ClassType)modified_hex.not_visible_for_the_class_type;
    }
    public void Spawn(Hex hex, ClassType class_type)
    {
        switch (type)
        {
            case Modifier.Trap:
                GameObject = Object.Instantiate(Resources.Load<GameObject>("Modified/" + type.ToString() + "/prefab"), hex.game_object.transform.position, Quaternion.identity);
                GameObject.transform.SetParent(hex.game_object.transform);

                if (GameManager.Instance.player.data.class_type == class_type)
                    GameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void OnDestroy(float time)
    {
        Object.Destroy(GameObject,time);
    }
    public Modified_Hex() { }
}
public class Hex 
{
    public int column { set; get; }
    public int row { set; get; }
    public GameObject game_object { set; get; }
    public List<Hex> neighbors { set; get; }
    public bool walkable { set; get; }
    public int weight { get; set; }
    public int cost { get; set; }
    public Hex prev_tile { get; set; }

    public MeshRenderer hex_mesh;

    public List<Modified_Hex> modified_hexes = new List<Modified_Hex>();

    public Color current_color;
    public Color start_color;
    public Hex(int _column, int _row, GameObject hex_go, bool _walkable = true)
    {
        column = _column;
        row = _row;
        game_object = hex_go;
        walkable = _walkable;
        neighbors = new List<Hex>();
        weight = 1;
        hex_mesh = game_object.GetComponent<MeshRenderer>();
    }

    public Data.Hex HexToData()
    {
        Data.Hex hex = new Data.Hex();
        hex.column = column;
        hex.row = row;
        hex.walkable = walkable == true ? 1 : 0;
        return hex;
    }

    public void SetColor(Color color)
    {
        hex_mesh.material.color = color;
        current_color = color;
    }
    public void Highlight(Color color)
    {
        hex_mesh.material.color = color;
    }
    public void ResetColor()
    {
        hex_mesh.material.color = current_color;
    }

    public void SetStartColor(Color color)
    {
        hex_mesh.material.color = color;
        start_color = color;
        current_color = start_color;
    }
    public void ResetStartColor()
    {
        hex_mesh.material.color = start_color;
        current_color = start_color;
    }
    public int HexDistance(Hex hex)
    {
        return (Mathf.Abs(column - hex.column) + Mathf.Abs(column + row - hex.column - hex.row) + Mathf.Abs(row - hex.row)) / 2;
    }

    public bool isHexContainTrap()
    {
        foreach (var modified_hex in modified_hexes)
            if (modified_hex.type == Modifier.Trap)
                return true;
        return false;
    }

    public ClassType ClassThatActivateTrap()
    {
        foreach (var modified_hex in modified_hexes)
            if (modified_hex.type == Modifier.Trap)
                return modified_hex.not_visible_for_the_class_type;
        return ClassType.None;
    }
    public Modified_Hex GetTrapModifier()
    {
        foreach (var modified_hex in modified_hexes)
            if (modified_hex.type == Modifier.Trap)
                return modified_hex;
        return null;
    }
}


