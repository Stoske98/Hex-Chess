using Networking.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region MapGenerator Singleton
    private static MapGenerator _instance;

    public static MapGenerator Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
    #endregion

    public float H;
    public int N;
    public float OFFSET;
    public Material HexMaterial;
    public Dictionary<Vector2Int, Hex> hexes = new Dictionary<Vector2Int, Hex>();
    [Header("Start Color")]
    public Color pattern_color_1;
    public Color pattern_color_2;
    public Color pattern_color_3;

    [Header("Game Color")]
    public Color selected_hero_color;
    public Color move_color;
    public Color attack_color;
    public Color ability_color;
    public Color special_color;

    public readonly List<Vector2Int> Vector2Neigbors = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(-1, 1), new Vector2Int(1, -1) };
   
    private Vector3 worldMousePosition;
    private Ray ray; 
    RaycastHit hit;
    Vector2Int roundedVector2;
    List<Vector2Int> neighborsV2;
    Hex closest_hex = null;
    Hex neighbor = null;
    private enum StartColumnColor
    { 
        WHITE = 0,
        BLACK = 1, 
        GRAY = 2
    }

    private void Start()
    {
    }
    public void CreateMap(List<Data.Hex> date_hexes)
    {
        Vector3 pos = Vector3.zero;
        for (int c = -N; c <= N; c++)
        {
            int r1 = Mathf.Max(-N, -c - N);
            int r2 = Mathf.Min(N, -c + N);

            int counter = 0;
            int index = 0;
            for (int i = 0; i < Mathf.Abs(c); i++)
            {
                counter++;
                if (counter == 3)
                    counter = 0;
            }

            StartColumnColor start_color = (StartColumnColor)counter;
            switch (start_color)
            {
                case StartColumnColor.WHITE:
                    index = 0;
                    break;
                case StartColumnColor.BLACK:
                    index = 2;
                    break;
                case StartColumnColor.GRAY:
                    index = 1;
                    break;
                default:
                    break;
            }

            for (int r = r1; r <= r2; r++)
            {
                pos.x = H * 3.0f / 2.0f * c * OFFSET;
                pos.z = H * Mathf.Sqrt(3.0f) * (r + c / 2.0f) * OFFSET;

                if (index == 3)
                    index = 0;

                foreach (var data_hex in date_hexes)
                {
                    if (data_hex.column == c && data_hex.row == r)
                    {
                        Hex hex = CreateHex(pos, c, r, data_hex.walkable == 1 ? true : false);
                        foreach (var modified_hex in data_hex.modified)
                        {
                            Modified_Hex mh = new Modified_Hex(modified_hex);
                            hex.modified_hexes.Add(mh);
                            mh.Spawn(hex, (ClassType)modified_hex.not_visible_for_the_class_type);
                        }
                        hexes.Add(new Vector2Int(c, r), hex);
                        if (index == 0)
                            hex.SetStartColor(pattern_color_1);
                        else if (index == 1)
                            hex.SetStartColor(pattern_color_2);
                        else if (index == 2)
                            hex.SetStartColor(pattern_color_3);
                    }
                }
                index++;
            }
        }
        foreach (Hex hex in hexes.Values)
            SetNeighbors(hex);
    }
    public void CreateMap()
    {
        Vector3 pos = Vector3.zero;
        for (int c = -N; c <= N; c++)
        {
            int r1 = Mathf.Max(-N, -c - N);
            int r2 = Mathf.Min(N, -c + N);

            for (int r = r1; r <= r2; r++)
            {
                pos.x = H * 3.0f / 2.0f * c * OFFSET;
                pos.z = H * Mathf.Sqrt(3.0f) * (r + c / 2.0f) * OFFSET;

                Hex hex = CreateHex(pos, c, r);
                hexes.Add(new Vector2Int(c, r), hex);  
            }
        }

        foreach (Hex hex in hexes.Values)
            SetNeighbors(hex);
    }

    private Hex CreateHex(Vector3 center, int column, int row, bool isWalkable = true)
    {
        GameObject hex_gameObject = new GameObject();
        hex_gameObject.transform.position = center;
        hex_gameObject.transform.SetParent(transform);
        hex_gameObject.name = "Tile[" + column + "][" + row + "]";
        hex_gameObject.AddComponent<MeshFilter>();
        hex_gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = hex_gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        float angle = 0;
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * H;
            angle += 60;

        }
        mesh.vertices = vertices;

        mesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };

        mesh.RecalculateNormals();

        hex_gameObject.GetComponent<MeshRenderer>().material = HexMaterial;       
        hex_gameObject.transform.localEulerAngles = new Vector3(-90, 0, 0);
        return new Hex(column, row, hex_gameObject, isWalkable);
    }
   
    public Hex GetHexFromMousePosition(Vector3 mousePosition, Camera camera)
    {
        ray = camera.ScreenPointToRay(mousePosition);
        worldMousePosition = Vector3.zero;

        if (Physics.Raycast(ray, out hit))
            worldMousePosition = hit.point;
        else return null;

        roundedVector2 = GetRoundedVector(worldMousePosition.x, worldMousePosition.z);

        neighborsV2 = new List<Vector2Int>()
        {
            roundedVector2,
            roundedVector2 + Vector2Neigbors[0],
            roundedVector2 + Vector2Neigbors[1],
            roundedVector2 + Vector2Neigbors[2],
            roundedVector2 + Vector2Neigbors[3],
            roundedVector2 + Vector2Neigbors[4],
            roundedVector2 + Vector2Neigbors[5],
        };

        closest_hex = null;
        neighbor = null;

        foreach (Vector2Int nv2 in neighborsV2)
        {            
            if(hexes.ContainsKey(nv2))
            {
                neighbor = hexes[nv2];
                if (closest_hex == null)
                    closest_hex = neighbor;
                else
                    if (Vector2.Distance(new Vector2(neighbor.game_object.transform.position.x, neighbor.game_object.transform.position.z), new Vector2(worldMousePosition.x, worldMousePosition.z)) <
                                    Vector2.Distance(new Vector2(closest_hex.game_object.transform.position.x, closest_hex.game_object.transform.position.z), new Vector2(worldMousePosition.x, worldMousePosition.z)))
                        closest_hex = neighbor;
            }
        }
        return closest_hex;
    }

    
    private Vector2Int GetRoundedVector(float x, float y)
    {
        float _x = (2.0f / 3 * x) / (H * OFFSET);
        float _y = (-1.0f / 3 * x + Mathf.Sqrt(3) / 3 * y) / (H * OFFSET);

        return Round(_x,_y);
    }
    private Vector2Int Round(float x, float y)
    {
        float s = -x - y;

        int _x = Mathf.RoundToInt(x);
        int _y = Mathf.RoundToInt(y);
        int _s = Mathf.RoundToInt(s);

        float _xDiff = Mathf.Abs(_x - x);
        float _yDiff = Mathf.Abs(_y - y);
        float _sDiff = Mathf.Abs(_s - s);

        if (_xDiff > _yDiff && _xDiff > _sDiff)
            _x = -_y - _s;
        else if (_yDiff > _sDiff)
            _y = -_x - _s;
        else
            _s = -_x - _y;
        return new Vector2Int(_x, _y);
    }
    private void SetNeighbors(Hex hex)
    {
        foreach (Vector2Int vector2 in Vector2Neigbors)
        {
            Hex neighbor = GetHex(hex.column + vector2.x, hex.row + vector2.y);
            if (neighbor != null)
                hex.neighbors.Add(neighbor);
        }
    }

    public Hex GetHex(int col, int row)
    {
        Vector2Int pos = new Vector2Int(col, row);
        Hex hex;
        if (hexes.ContainsKey(pos))
        {
            hexes.TryGetValue(pos, out hex);
            if (hex != null)
                return hex;
        }
        return null;
    }

    public Hex ReturnHex(GameObject gameObject)
    {
        foreach (Hex hex in hexes.Values)
        {
            if (hex.game_object == gameObject)
            {
                return hex;
            }
        }
        return null;
    }

    public Hex GetHexInDirection(Hex center, Direction direction)
    {
        Hex hex = null;

        switch (direction)
        {
            case Direction.Top:
                hex = GetHex(center.column,center.row + 1);
                break;
            case Direction.Bottom:
                hex = GetHex(center.column, center.row - 1);
                break;
            case Direction.Top_Left:
                hex = GetHex(center.column + 1, center.row);
                break;
            case Direction.Top_Right:
                hex = GetHex(center.column - 1, center.row + 1);
                break;
            case Direction.Bottom_Left:
                hex = GetHex(center.column + 1, center.row - 1);
                break;
            case Direction.Bottom_Right:
                hex = GetHex(center.column - 1, center.row);
                break;
            default:
                break;
        }
        return hex;
    }

    public List<Hex> HexesInRange(Hex center, int range)
    {
        List<Hex> hices = new List<Hex>();

        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                Hex hex = GetHex(q + center.column, r + center.row);
                if (hex != null)
                    hices.Add(hex);
            }
        }
        return hices;
    }
}

public enum Direction
{
    Top = 0,
    Bottom = 1,
    Top_Left = 2,
    Top_Right = 3,
    Bottom_Left = 4,
    Bottom_Right = 5
}

