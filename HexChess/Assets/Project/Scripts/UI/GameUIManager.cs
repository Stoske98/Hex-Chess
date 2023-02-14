using Networking.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region GameUIManager Singleton
    private static GameUIManager _instance;

    public static GameUIManager Instance { get { return _instance; } }


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

    private Animator animator;
    public GameObject health_bar_prefab;
    public GameObject connecting_panel;
    public Chat chat;
    public TextMeshProUGUI move;

    [Header("Unit")]
    public Image heroImage;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI health;
    public GameObject ability_gameObject;
    private AbilityController[] abilityControllers;

    [Header("Challenge Royale")]
    public TextMeshProUGUI challenge_royale_move;
    public TextMeshProUGUI challenge_royale_TXT;
    public TextMeshProUGUI dropdown_txt;
    public Image challenge_image;
    public Sprite light_challenge;
    public Sprite dark_challenge;

    [Header("Win")]
    public Image win_image;
    public Sprite light_win;
    public Sprite dark_win;

    [Header("On Turn")]
    public Image on_turn_image;
    public Sprite light_on_turn;
    public Sprite dark_on_turn;

    [Header("On Enemy Disconnect")]
    public GameObject disconnect_gameobject;
    public TextMeshProUGUI disconnect_text;


    [Header("Surrend")]
    public GameObject surrend_gameobject;

    [Header("Heroe Interactive Bar")]
    public RectTransform hero_interactive_bar;
    private AbilityController[] bar_controller;

    public DeathController death_controller_ui;


    private void Start()
    {
        abilityControllers = ability_gameObject.GetComponentsInChildren<AbilityController>();
        bar_controller = hero_interactive_bar.GetComponentsInChildren<AbilityController>();
        animator = gameObject.GetComponent<Animator>();
    }
    public void UpdateUnitUI(Unit unit)
    {
        unit.health_bar.Update(unit);
        if (GameManager.Instance.selected_unit == unit)
            OnSelectUnit(unit);
    }
    public void OnHoverUnitEnter(Unit unit)
    {
        unit.health_bar.Update(unit);
        unit.health_bar.Activate();       
    }

    public void OnHoverUnitExit(Unit unit)
    {
        unit.health_bar.Deactivate();
    }

    public void OnSelectUnit(Unit unit)
    {
        heroImage.sprite = unit.sprite;
        health.text = unit.stats.current_health.ToString();
        damage.text = unit.stats.damage.ToString();
        ShowAbilities(unit);
    }
    public void OnDeselectUnit(Unit unit)
    {
        unit.health_bar.Deactivate();
    }

    private void ShowAbilities(Unit unit)
    {
        for (int i = 0; i < unit.abilities.Length; i++)
        {
            abilityControllers[i].gameObject.SetActive(true);
            abilityControllers[i].ShowAbility(unit.abilities[i]);
        }

        for (int i = unit.abilities.Length; i < abilityControllers.Length; i++)
        {
            abilityControllers[i].gameObject.SetActive(false);
        }
    }

    public void ShowBar(Unit unit, Camera camera)
    {
        hero_interactive_bar.gameObject.SetActive(true);

        Hex unit_hex = MapGenerator.Instance.GetHex(unit.column, unit.row);
        Vector3 screenPos = camera.WorldToScreenPoint(unit_hex.game_object.transform.position);
        hero_interactive_bar.position = screenPos;

        List<Hex> hexes = new List<Hex>();
        if(!unit.IsDisarmed() && !unit.IsStunned())
        {
            for (int i = 0; i < unit.abilities.Length; i++)
            {
                hexes.Clear();
                unit.abilities[i].GetAbilityHexes(unit_hex, ref hexes);

                if (hexes.Count != 0)
                    bar_controller[i].ShowBarAbility(unit.abilities[i], true);
                else
                    bar_controller[i].ShowBarAbility(unit.abilities[i], false);
                bar_controller[i].gameObject.SetActive(true);

            }
            for (int i = unit.abilities.Length; i < bar_controller.Length; i++)
            {
                if (i != 3)//if i == 3 that means attack 
                    bar_controller[i].gameObject.SetActive(false);
                else
                {
                    hexes.Clear();
                    unit.GetAttackMoves(unit_hex, ref hexes);
                    if (hexes.Count != 0)
                        bar_controller[i].cooldownImage.fillAmount = 0;
                    else
                        bar_controller[i].cooldownImage.fillAmount = 1;
                    bar_controller[i].gameObject.SetActive(true);

                }
            }
        }
        else
        {
            for (int i = 0; i < unit.abilities.Length; i++)
            {
                bar_controller[i].ShowBarAbility(unit.abilities[i], false);
                bar_controller[i].gameObject.SetActive(true);
            }
            for (int i = unit.abilities.Length; i < bar_controller.Length; i++)
            {
                if (i != 3)
                    bar_controller[i].gameObject.SetActive(false);
                else
                {
                    bar_controller[i].cooldownImage.fillAmount = 1;
                    bar_controller[i].gameObject.SetActive(true);
                }
            }
        }
       
    }
    public void PlayWinAnimation(ClassType class_type)
    {
        if (class_type == ClassType.Light)
            win_image.sprite = light_win;
        else
            win_image.sprite = dark_win;

        animator.Play("Win");
    }

    public void PlayChallengeRoyaleAnimation(ClassType class_type)
    {
        if (class_type == ClassType.Light)
            challenge_image.sprite = light_challenge;
        else
            challenge_image.sprite = dark_challenge;

        animator.Play("ChallengeRoyale");
    }
    public void ActivateChallengeRoyale()
    {
        GameManager.Instance.challenge_royal_activated = true;
        UpdateChallengeRoyaleMove(GameManager.Instance.challenge_royal_move);
        challenge_royale_move.transform.GetChild(0).gameObject.SetActive(true);
        challenge_royale_TXT.text = "Challenge Royale";
        dropdown_txt.text = "Dropdown";
    }
    public void UpdateChallengeRoyaleMove(int move)
    {
        if (move >= 0)
        {
            if (move > 20)
                challenge_royale_move.text = (move - 20).ToString();
            else if (move > 10)
                challenge_royale_move.text = (move - 10).ToString();
            else
                challenge_royale_move.text = (move).ToString();
        }
    }

    public void UpdateWhoIsOnTurn(ClassType class_type)
    {
        if (class_type == ClassType.Light)
            on_turn_image.sprite = light_on_turn;
        else
            on_turn_image.sprite = dark_on_turn;
    }

    public void OnConnect()
    {
        connecting_panel.SetActive(false);
    }
    public void OnSurrendButtonClick()
    {
        surrend_gameobject.SetActive(true);
    }
    public void ConfirmSurrend()
    {
        NetSurrend request = new NetSurrend();
        Sender.TCP_SendToServer(request);
        GameManager.Instance.pause = true;
        surrend_gameobject.SetActive(false);
    }

    public void DeclineSurrend()
    {
        surrend_gameobject.SetActive(false);
    }
    public void OnGameEnd()
    {
        connecting_panel.SetActive(true);
        animator.Play("Idle");
        chat.Idle();

        challenge_royale_move.text = "";
        challenge_royale_move.transform.GetChild(0).gameObject.SetActive(false);
        challenge_royale_TXT.text = "";
        dropdown_txt.text = "";

        death_controller_ui.OnReset();

        disconnect_text.text = "Waiting for enemy opponent to connect.";
        disconnect_gameobject.SetActive(true);

        surrend_gameobject.SetActive(false);
    }

    public void OnDisconnect(string nickname)
    {
        GameManager.Instance.pause = true;
        disconnect_text.text = nickname + " has disconnected from the game. Please wait for them to reconnect.";
        disconnect_gameobject.SetActive(true);
    }

    public void OnReconnect()
    {
        GameManager.Instance.pause = false;
        disconnect_gameobject.SetActive(false);
    }
    public void OnHeroBarHover(int index)
    {
        Unit selected_unit = GameManager.Instance.selected_unit;
        HeroBar bar = (HeroBar)index;

        if (selected_unit != null)
        {
            GameManager.Instance.RemoveMarkedFields();
            Hex unit_hex = MapGenerator.Instance.GetHex(selected_unit.column, selected_unit.row);
            if (bar != HeroBar.ATTACK)
            {
                Ability ability = selected_unit.abilities[index];
                ability.GetAbilityHexes(unit_hex, ref GameManager.Instance.abilityMoves);
                GameManager.Instance.MarkAbilityFields();
            }
            else
            {
                selected_unit.GetAttackMoves(unit_hex, ref GameManager.Instance.availableAttackMoves);
                GameManager.Instance.MarkAttackFields();
            }
        }
    }
    public void OnHeroBarUnHover()
    {
        GameManager.Instance.RemoveMarkedFields();
    }
    public void OnHeroBarClick(int index)
    {
        Unit selected_unit = GameManager.Instance.selected_unit;
        HeroBar bar = (HeroBar)index;

        if(selected_unit != null && !selected_unit.IsStunned() && !selected_unit.IsDisarmed())
        {
            if(bar != HeroBar.ATTACK)
            {
                Ability ability = selected_unit.abilities[index];
                switch (bar)
                {
                    case HeroBar.ABILITY_1:
                        GameManager.Instance.OnPressAbilityKey(ability, AbilityInput.Q);
                        break;
                    case HeroBar.ABILITY_2:
                        GameManager.Instance.OnPressAbilityKey(ability, AbilityInput.W);
                        break;
                    case HeroBar.ABILITY_3:
                        GameManager.Instance.OnPressAbilityKey(ability, AbilityInput.E);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                GameManager.Instance.RemoveMarkedFields();
                selected_unit.GetAttackMoves(MapGenerator.Instance.GetHex(selected_unit.column, selected_unit.row), ref GameManager.Instance.availableAttackMoves);
                GameManager.Instance.MarkAttackFields();
            }
        }
        hero_interactive_bar.gameObject.SetActive(false);

    }

    private enum HeroBar
    {
        ABILITY_1 = 0, ABILITY_2 = 1, ABILITY_3 = 2, ATTACK = 3
    }
}


