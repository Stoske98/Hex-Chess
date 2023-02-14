using Networking.Client;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    #region MainMenuUIManager Singleton
    private static MainMenuUIManager _instance;

    public static MainMenuUIManager Instance { get { return _instance; } }


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
    public GameObject connecting_panel;
    public Chat chat;

    public Button find_match_light_btn;
    public Button find_match_dark_btn;

    public GameObject finding_match_light;
    public GameObject finding_match_dark;

    public Button deselect_dark_btn;
    public Button deselect_light_btn;

    [Header("Player Info")]
    public TextMeshProUGUI nickname;
    public TextMeshProUGUI rank;
    public TMP_InputField input_nickname;

    [Header("MatchFound")]
    public GameObject match_found_gameobject;
    public TextMeshProUGUI enemy_nickname;
    public TextMeshProUGUI enemy_rank;
    public TextMeshProUGUI status;
    public GameObject buttons;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void StartFindingMatchRequest(int class_type)
    {
        GameManager.Instance.player.data.class_type = (ClassType)class_type;

        NetCreateTicket request = new NetCreateTicket();
        request.class_type = (ClassType)class_type;
        Sender.TCP_SendToServer(request);
    }
    public void StopFindingMatchRequest()
    {
        NetStopMatchFinding request = new NetStopMatchFinding();
        Sender.TCP_SendToServer(request);
    }

    public void AcceptMatchRequest()
    {
        NetAcceptMatch request = new NetAcceptMatch();

        Data.Game data = new Data.Game();
        if (GameManager.Instance.player.data.class_type == ClassType.Light)
            GameManager.Instance.CreateTeam1(data, ClassType.Light);
        else
            GameManager.Instance.CreateTeam2(data, ClassType.Dark);

        request.xml_team_structure = Data.Serialize<Data.Game>(data);
        Sender.TCP_SendToServer(request);
    }

    public void DeclineMatchRequest()
    {
        NetDeclineMatch request = new NetDeclineMatch();
        Sender.TCP_SendToServer(request);
    }
    public void OnDeclineMatchResponess()
    {
            buttons.SetActive(false);
            status.color = Color.red;
            status.text = "Match has been DECLINED";
            Invoke("ResetMatchBox",2);

    }
    public void ShowMatchFound(Data.Player enemy)
    {
        enemy_nickname.text = enemy.nickname;
        enemy_rank.text = enemy.rank.ToString();
        buttons.SetActive(true);
        match_found_gameobject.SetActive(true);
    }

    public void BothPlayerAcceptMatch()
    {
        status.color = Color.green;
        status.text = "Both player ACCEPTED the match"; 
        Invoke("ResetMatchBox", 2);
        animator.Play("Idle");
    }
    private void ResetMatchBox()
    {
        match_found_gameobject.SetActive(false);
        buttons.SetActive(true);
        status.text = "";
    }
    public void OnAcceptMatchResponess(ClassType class_type)
    {
        status.color = Color.green;
        status.text = "Match has been ACCEPTED";
        if (class_type == GameManager.Instance.player.data.class_type)
            buttons.SetActive(false);
    }
    public void StartFindingMatchResponess()
    {
        if (GameManager.Instance.player.data.class_type == ClassType.Light)
        {
            deselect_light_btn.interactable = false;
            finding_match_light.SetActive(true);
            find_match_light_btn.interactable = false;

        }
        else
        {
            deselect_dark_btn.interactable = false;
            finding_match_dark.SetActive(true);
            find_match_dark_btn.interactable = false;
        }
    }
    public void StopFindingMatchResponess()
    {
        if (GameManager.Instance.player.data.class_type == ClassType.Light)
        {
            deselect_light_btn.interactable = true;
            finding_match_light.SetActive(false);
            find_match_light_btn.interactable = true;

        }
        else
        {
            deselect_dark_btn.interactable = true;
            finding_match_dark.SetActive(false);
            find_match_dark_btn.interactable = true;
        }
    }

    public void ChangeNickname()
    {
        NetChangeNickname request = new NetChangeNickname();
        request.nickname = input_nickname.text;
        Sender.TCP_SendToServer(request);
    }

    public void ChangeNickname(string name)
    {
        nickname.text = name;
        input_nickname.gameObject.SetActive(false);
    }

    public void ActivateInputNickname()
    {
        input_nickname.text = nickname.text;
        input_nickname.gameObject.SetActive(true);
    }
    public void SelectLight()
    {
        animator.Play("SelectLight");
    }

    public void DeselectLight()
    {
        animator.Play("DeselectLight");
    }
    public void SelectDark()
    {
        animator.Play("SelectDark");
    }

    public void DeselectDark()
    {
        animator.Play("DeselectDark");
    }
    public void SetPlayerInfo(Player player)
    {
        nickname.text = player.data.nickname;
        rank.text = player.data.rank.ToString();
    }
    public void OnConnected()
    {
        connecting_panel.SetActive(false);
    }
}
