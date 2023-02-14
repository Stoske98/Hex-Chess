using Networking.Client;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    #region GameManager Singleton
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }


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

    [HideInInspector] public int id;
    public MapGenerator map;
    public Transform units_container;
    public Player player;

    public ClassType class_on_turn = ClassType.None;
    public int move;
    public bool challenge_royal_activated = false;
    public int challenge_royal_move;

    public List<Unit> units = new List<Unit>();
    private List<Hex> availableMoves = new List<Hex>();
    private List<Hex> specialMoves = new List<Hex>();
    public List<Hex> availableAttackMoves = new List<Hex>();
    public List<Hex> abilityMoves = new List<Hex>();
    private AbilityInput ability_input = AbilityInput.None;
    private Camera camera;
    [HideInInspector] public CameraController camera_controller;

    [Header("Main and Game Canvas")]
    public Canvas main_menu_canvas;
    public Canvas game_canvas;

    Hex hitHex;
    Hex currentHex;
    public Hex selectedHex;

    Unit hit_unit;
    Unit hovered_unit;
    public Unit selected_unit;

    [HideInInspector] public bool pause = true;
    private void Start()
    {
        camera_controller = gameObject.GetComponent<CameraController>();
        camera = camera_controller.cam;
        pause = true;
       // MapGenerator.Instance.CreateMap();
    }
    void Update()
    {
        if(!pause)
        {
            Raycast();
            UpdateUnits();

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Hex hex in map.hexes.Values)
                if (hex.walkable == false)
                    hex.SetColor(Color.black);
            if (selected_unit != null)
            {
                if(selected_unit.IsStunned())
                {
                    foreach (var cc in selected_unit.cc)
                    {
                        Debug.Log(cc.type.ToString() + " cooldown " + cc.current_cooldown);
                    }
                }
               
            }
        }
    }

    private void Raycast()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            hitHex = map.GetHexFromMousePosition(Input.mousePosition, camera);
            Hover();
            if (selected_unit != null && selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
                if (IsUnitControlled())
                    return;

            if (Input.GetMouseButtonDown(0) && hitHex != null)
                SelectUnit();

            if (Input.GetMouseButtonDown(1))
                Deselect();
        }
    }

    public void Hover()
    {
        if (hitHex != null && currentHex != hitHex)
        {
            if (currentHex != null)
                currentHex.ResetColor();
                        
            currentHex = hitHex;
            currentHex.Highlight(Color.green);
            hit_unit = GetUnit(currentHex);

            if(hit_unit != null)
            {
                if(hovered_unit != hit_unit)
                {
                    if (hovered_unit == null)
                        hovered_unit = hit_unit;

                    GameUIManager.Instance.OnHoverUnitExit(hovered_unit);
                    hovered_unit = hit_unit;
                    GameUIManager.Instance.OnHoverUnitEnter(hovered_unit);

                }
            }
            else
            {
                if(hovered_unit != null)
                {
                    GameUIManager.Instance.OnHoverUnitExit(hovered_unit);
                    hovered_unit = null;
                    hit_unit = null;
                }
            }
        }
    }
    public bool IsUnitControlled()
    {
        if (!selected_unit.IsStunned())
        {
            if(!selected_unit.IsDisarmed())
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    return OnPressAbilityKey(selected_unit.abilities[0], AbilityInput.Q);
                if (Input.GetKeyDown(KeyCode.W))
                    return OnPressAbilityKey(selected_unit.abilities[1], AbilityInput.W);
                if (Input.GetKeyDown(KeyCode.E))
                    return OnPressAbilityKey(selected_unit.abilities[2], AbilityInput.E);
                if (Input.GetMouseButtonDown(1) && availableAttackMoves.Contains(hitHex))
                {
                    Unit enemy = GetUnit(hitHex);

                    NetAttack request = new NetAttack
                    {
                        unit_id = selected_unit.special_id,
                        enemy_id = enemy.special_id,
                        end_turn = Convert.ToInt32(true)
                    };
                    Sender.TCP_SendToServer(request);

                    class_on_turn = ClassType.None;
                    RemoveMarkedFields();
                    return true;
                }
                if(ability_input != AbilityInput.None)
                {
                    if (Input.GetMouseButtonDown(0) && abilityMoves.Contains(hitHex))
                    {
                        NetAbility request = new NetAbility
                        {
                            input = ability_input,
                            unit_id = selected_unit.special_id,
                            to_hex = new Data.Hex { column = hitHex.column, row = hitHex.row },
                            end_turn = Convert.ToInt32(true)
                        };
                        Sender.TCP_SendToServer(request);

                        class_on_turn = ClassType.None;
                        RemoveAbilityFields();
                        return true;
                    }
                }
            }
            if(Input.GetMouseButtonDown(0))
            {
                if(specialMoves.Contains(hitHex))
                {
                    NetAbility request = new NetAbility
                    {
                        input = AbilityInput.S,
                        unit_id = selected_unit.special_id,
                        to_hex = new Data.Hex { column = hitHex.column, row = hitHex.row },
                        end_turn = Convert.ToInt32(true)
                    };
                    Sender.TCP_SendToServer(request);

                    class_on_turn = ClassType.None;
                    RemoveMarkedFields();
                    return true;
                }
                else if(availableMoves.Contains(hitHex))
                {
                    NetMove request = new NetMove
                    {
                        unit_id = selected_unit.special_id,
                        to_hex = new Data.Hex { column = hitHex.column, row = hitHex.row },
                        end_turn = Convert.ToInt32(true)
                    };
                    Sender.TCP_SendToServer(request);

                    class_on_turn = ClassType.None;
                    RemoveMarkedFields();
                    return true;
                }
                else if (availableAttackMoves.Contains(hitHex))
                {
                    Unit enemy = GetUnit(hitHex);

                    NetAttack request = new NetAttack
                    {
                        unit_id = selected_unit.special_id,
                        enemy_id = enemy.special_id,
                        end_turn = Convert.ToInt32(true)
                    };
                    Sender.TCP_SendToServer(request);

                    class_on_turn = ClassType.None;
                    RemoveMarkedFields();
                    return true;
                }
            }
        }
        return false;
    }
    public void SelectUnit()
    {
        if (hovered_unit != null)
        {
            if(hovered_unit == selected_unit)
            {
                if (selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
                    SetUpHeroBar();
            }else
            {
                GameUIManager.Instance.hero_interactive_bar.gameObject.SetActive(false);
                selected_unit = hovered_unit;
                RemoveMarkedFields();
                selectedHex = currentHex;
                selectedHex.SetColor(map.selected_hero_color);
                GameUIManager.Instance.OnSelectUnit(selected_unit); 
                if (selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
                    UnitPossibilities();
            }
           
        }else
          Deselect();
          
    }
    public bool OnPressAbilityKey(Ability ability, AbilityInput input)
    {
        if(ability != null && ability.current_cooldown == 0 && ability.ability_type != AbilityType.Passive)
        {
            switch (ability.ability_type)
            {
                case AbilityType.Passive:
                    break;
                case AbilityType.Targetable:
                    RemoveMarkedFields();
                    ability.GetAbilityHexes(map.GetHex(selected_unit.column,selected_unit.row), ref abilityMoves);
                    MarkAbilityFields();
                    ability_input = input;
                    return true;
                case AbilityType.Instant:
                    RemoveMarkedFields();
                    NetAbility request = new NetAbility
                    {
                        input = input,
                        unit_id = selected_unit.special_id,
                        to_hex = new Data.Hex { column = -100, row = -100 },
                        end_turn = Convert.ToInt32(true)
                    };
                    Sender.TCP_SendToServer(request);
                    class_on_turn = ClassType.None;
                    return true;
                default:
                    break;
            }
        }
        return false;
    }
    public void UnitPossibilities()
    {
        selectedHex = map.GetHex(selected_unit.column,selected_unit.row);
        if(selectedHex != null)
        {
            selectedHex.SetColor(map.selected_hero_color);
            if (!selected_unit.IsStunned())
            {
                selected_unit.GetAvailableMoves(selectedHex, ref availableMoves);
                MarkFields(ref availableMoves, map.move_color);

                if (!selected_unit.IsDisarmed())
                {
                    selected_unit.GetAttackMoves(selectedHex, ref availableAttackMoves);
                    MarkFields(ref availableAttackMoves, map.attack_color);

                    selected_unit.GetSpecialMoves(selectedHex, ref specialMoves);
                    MarkFields(ref specialMoves, map.special_color);
                }
            }           
        }
    }
    private void Deselect()
    {
        RemoveMarkedFields();
        GameUIManager.Instance.hero_interactive_bar.gameObject.SetActive(false);
        if (selected_unit != null && selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
            UnitPossibilities();
    }
    public void OnGameEnd()
    {
        class_on_turn = ClassType.None;
        move = 0;
        challenge_royal_activated = false;
        challenge_royal_move = 30;
        foreach (var unit in units)
            Destroy(unit.game_object);
        units.Clear();
        availableMoves.Clear();
        specialMoves.Clear();
        availableAttackMoves.Clear();
        abilityMoves.Clear();
        ability_input = AbilityInput.None;
        foreach (var item in map.hexes)
            Destroy(item.Value.game_object);
        map.hexes.Clear();

        hitHex = null;
        currentHex = null;
        selectedHex = null;

        hit_unit = null;
        hovered_unit = null;
        selected_unit = null;

        GameUIManager.Instance.OnGameEnd();
    }

    public void SetUpHeroBar()
    {
        if(!GameUIManager.Instance.hero_interactive_bar.gameObject.activeSelf)
        {
            RemoveMarkedFields();
            selectedHex.SetColor(map.selected_hero_color);
            GameUIManager.Instance.ShowBar(selected_unit, camera);
        }else
        {
            GameUIManager.Instance.hero_interactive_bar.gameObject.SetActive(false);
            RemoveMarkedFields();
            UnitPossibilities();
        }
    }
   
    public void ToString()
    {
        Debug.Log("Class on turn: " + class_on_turn.ToString() + "\nMove: " + move + "\nChallenge Royal Activated: " + challenge_royal_activated + "\nChallenge Royal Move: " + challenge_royal_move);
    }
    public void EndTurn(ClassType on_turn)
    {
        class_on_turn = on_turn;
        GameUIManager.Instance.UpdateWhoIsOnTurn(class_on_turn);

        foreach (Unit unit in Instance.units)
        {
            if (unit.class_type == class_on_turn)
            {
                unit.special_ability.ReduceCooldown();
                foreach (Ability ability in unit.abilities)
                    ability.ReduceCooldown();

                GameUIManager.Instance.UpdateUnitUI(unit);
            }
        }

        foreach (Unit unit in Instance.units)
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

        if(challenge_royal_activated)
        {
            challenge_royal_move--;
            GameUIManager.Instance.UpdateChallengeRoyaleMove(challenge_royal_move);
            TryToRemoveFields();
        }
        move++;
        GameUIManager.Instance.move.text = move.ToString();
        ToString();

        if (selected_unit != null && selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
            UnitPossibilities();
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

        Hex center_hex = map.GetHex(0, 0);
        List<Hex> inner_hexes = map.HexesInRange(center_hex, inner);

        foreach (var hex in map.hexes.Values)
        {
            if (!inner_hexes.Contains(hex))
            {
                Unit unit = GetUnit(hex);
                if (unit != null)
                    unit.ReceiveDamage(unit.stats.current_health);
                else
                    hex.walkable = false;
                hex.game_object.SetActive(false);
            }
        }
    }

    public void RemoveFields()
    { 
        int inner = 0;
        if (challenge_royal_move <= 0)
            inner = 1;
        else if (challenge_royal_move <= 10)
            inner = 2;
        else if (challenge_royal_move <= 20)
            inner = 3;
        else return;

        Hex center_hex = map.GetHex(0, 0);
        List<Hex> inner_hexes = map.HexesInRange(center_hex, inner);

        foreach (var hex in map.hexes.Values)
        {
            if (!inner_hexes.Contains(hex))
                hex.game_object.SetActive(false);
        }
    }

    private void MarkFields(ref List<Hex> hices, Color color)
    {
        foreach (Hex hex in hices)
            hex.SetColor(color);
    }
    public void MarkAbilityFields()
    {
        foreach (Hex abilityMove in abilityMoves)
            abilityMove.SetColor(map.ability_color);
    }
    private void RemoveAbilityFields()
    {
        foreach (Hex abilityMove in abilityMoves)
            abilityMove.ResetStartColor();
        ability_input = AbilityInput.None;
        abilityMoves.Clear();
    }
    public void MarkAttackFields()
    {
        foreach (Hex attack_moves in availableAttackMoves)
            attack_moves.SetColor(map.attack_color);
    }
    public void RemoveMarkedFields()
    {
        foreach (Hex hex in map.hexes.Values)
            hex.ResetStartColor();

        availableAttackMoves.Clear();
        abilityMoves.Clear();
        availableMoves.Clear();
        specialMoves.Clear();
    }

    private void UpdateUnits()
    {
        foreach (Unit unit in units)
        {
            unit.Update(Time.deltaTime);
        }
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
    public void SyncGameData(Data.Game game_data)
    {
        map.CreateMap(game_data.hexes_data);
        units = Spawner.SpawnUnits(game_data.units_data);

        class_on_turn = (ClassType)game_data.class_on_turn;
        GameUIManager.Instance.UpdateWhoIsOnTurn(class_on_turn);

        move = game_data.move;
        GameUIManager.Instance.move.text = move.ToString();
        challenge_royal_move = game_data.challenge_royal_move;
        challenge_royal_activated = Convert.ToBoolean(game_data.challenge_royal_activated);

        foreach (var unit in units)
            if(unit.current_state == unit.death_state)
                GameUIManager.Instance.death_controller_ui.OnUnitDeath(unit);
        if (challenge_royal_activated)
        {
            GameUIManager.Instance.ActivateChallengeRoyale();
            RemoveFields();
        }
        
        camera_controller.SetCamera(player.data.class_type);
        SelectKing(player.data.class_type);
        GameUIManager.Instance.OnConnect();
    }

    public void SelectKing(ClassType classType)
    {
        foreach (var unit in units)
        {
            if (unit.class_type == classType && unit.unit_type == UnitType.King)
            {
                selected_unit = unit;
                selectedHex = currentHex = hitHex = map.GetHex(selected_unit.column, selected_unit.row);
                selectedHex.SetColor(map.selected_hero_color);
                GameUIManager.Instance.OnSelectUnit(selected_unit);

                RemoveMarkedFields();
                if (selected_unit.class_type == player.data.class_type && class_on_turn == player.data.class_type)
                    UnitPossibilities();
                return;
                
            }
        }
    }
    public void CreateTeam1(Data.Game game_data, ClassType classType)
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
    public void CreateTeam2(Data.Game game_data, ClassType classType)
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
}
