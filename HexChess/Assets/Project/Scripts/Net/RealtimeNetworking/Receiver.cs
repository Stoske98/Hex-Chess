namespace Networking.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using UnityEngine;

    public class Receiver : MonoBehaviour
    {
        #region Receiver Singleton
        private static Receiver _instance;

        public static Receiver Instance { get { return _instance; } }


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
        Data.Player enemy = null;
        private Coroutine pollTicketCoroutine;
        public void SubscibeMainMenu()
        {
            RealtimeNetworking.C_INIT_RESPONESS += OnInitializationResponess;
            RealtimeNetworking.C_CREATE_TICKET_RESPONESS += OnCreateTicketResponess;
            RealtimeNetworking.C_FIND_MATCH_RESPONESS += OnFindMatchResponess;
            RealtimeNetworking.C_STOP_MATCH_FINDING_RESPONESS += OnStopMatchFindingResponess;
            RealtimeNetworking.C_ACCEPT_MATCH_RESPONESS += OnAcceptMatchResponess;
            RealtimeNetworking.C_DECLINE_MATCH_RESPONESS += OnDeclineMatchResponess;
            RealtimeNetworking.C_MATCH_CREATED_RESPONESS += OnMatchCreatedResponess;
            RealtimeNetworking.C_WELCOME_MATCHMAKING_RESPONESS += OnWelcomeMatchmakingResponess;
            RealtimeNetworking.C_AUTHENTICATION_RESPONESS += OnAuthenticationMResponess;
            RealtimeNetworking.C_SYNCHRONIC_RESPONESS += OnSyncronicMResponess;
            RealtimeNetworking.C_CHANGE_NICKNAME_RESPONESS += OnChangeNicknameResponess;
            RealtimeNetworking.C_CHAT_M_RESPONESS += OnChatMResponess;
            RealtimeNetworking.C_ERROR_RESPONESS += OnErrortResponess;
        }
        public void SubscribeMatch()
        {
            RealtimeNetworking.C_INIT_RESPONESS += OnInitializationResponess;
            RealtimeNetworking.C_AUTH_RESPONESS += OnAuthenticationnResponess;
            RealtimeNetworking.C_SYNC_RESPONESS += OnSynchronicResponess;
            RealtimeNetworking.C_MOVE_RESPONESS += OnMoveResponess;
            RealtimeNetworking.C_ATTACK_RESPONESS += OnAttackResponess;
            RealtimeNetworking.C_ABILITY_RESPONESS += OnAbilityResponess;
            RealtimeNetworking.C_JESTER_SA_RESPONESS += OnJesterSpecialAbilityResponess;
            RealtimeNetworking.C_SWORDSMAN_SA_RESPONESS += OnSwordsmanSpecialAbilityResponess;
            RealtimeNetworking.C_END_TURN_RESPONESS += OnEndTurnResponess;
            RealtimeNetworking.C_END_GAME_RESPONESS += OnEndGameResponess;
            RealtimeNetworking.C_WELCOME_GAME_RESPONESS += OnWelcomeGameResponess;
            RealtimeNetworking.C_CHAT_RESPONESS += OnChatResponess;
            RealtimeNetworking.C_DISCONNECT_RESPONESS += OnDisconnectResponess;
            RealtimeNetworking.C_RECONNECT_RESPONESS += OnReconnectResponess;
            RealtimeNetworking.C_ERROR_RESPONESS += OnErrortResponess;

        }
        public void UnSubscribeOnLeaveMatch()
        {
            RealtimeNetworking.C_INIT_RESPONESS -= OnInitializationResponess;
            RealtimeNetworking.C_AUTH_RESPONESS -= OnAuthenticationnResponess;
            RealtimeNetworking.C_SYNC_RESPONESS -= OnSynchronicResponess;
            RealtimeNetworking.C_MOVE_RESPONESS -= OnMoveResponess;
            RealtimeNetworking.C_ATTACK_RESPONESS -= OnAttackResponess;
            RealtimeNetworking.C_ABILITY_RESPONESS -= OnAbilityResponess;
            RealtimeNetworking.C_JESTER_SA_RESPONESS -= OnJesterSpecialAbilityResponess;
            RealtimeNetworking.C_SWORDSMAN_SA_RESPONESS -= OnSwordsmanSpecialAbilityResponess;
            RealtimeNetworking.C_END_TURN_RESPONESS -= OnEndTurnResponess;
            RealtimeNetworking.C_END_GAME_RESPONESS -= OnEndGameResponess;
            RealtimeNetworking.C_WELCOME_GAME_RESPONESS -= OnWelcomeGameResponess;
            RealtimeNetworking.C_CHAT_RESPONESS -= OnChatResponess;
            RealtimeNetworking.C_DISCONNECT_RESPONESS -= OnDisconnectResponess;
            RealtimeNetworking.C_RECONNECT_RESPONESS -= OnReconnectResponess;
            RealtimeNetworking.C_ERROR_RESPONESS -= OnErrortResponess;
        }
        public void UnSubscibeOnLeaveMainMenu()
        {
            RealtimeNetworking.C_INIT_RESPONESS -= OnInitializationResponess;
            RealtimeNetworking.C_CREATE_TICKET_RESPONESS -= OnCreateTicketResponess;
            RealtimeNetworking.C_FIND_MATCH_RESPONESS -= OnFindMatchResponess;
            RealtimeNetworking.C_STOP_MATCH_FINDING_RESPONESS -= OnStopMatchFindingResponess;
            RealtimeNetworking.C_ACCEPT_MATCH_RESPONESS -= OnAcceptMatchResponess;
            RealtimeNetworking.C_DECLINE_MATCH_RESPONESS -= OnDeclineMatchResponess;
            RealtimeNetworking.C_MATCH_CREATED_RESPONESS -= OnMatchCreatedResponess;
            RealtimeNetworking.C_WELCOME_MATCHMAKING_RESPONESS -= OnWelcomeMatchmakingResponess;
            RealtimeNetworking.C_AUTHENTICATION_RESPONESS -= OnAuthenticationMResponess;
            RealtimeNetworking.C_SYNCHRONIC_RESPONESS -= OnSyncronicMResponess;
            RealtimeNetworking.C_CHANGE_NICKNAME_RESPONESS -= OnChangeNicknameResponess;
            RealtimeNetworking.C_CHAT_M_RESPONESS -= OnChatMResponess;
            RealtimeNetworking.C_ERROR_RESPONESS -= OnErrortResponess;
        }

        private void OnInitializationResponess(NetMessage message)
        {
            NetInitialization responess = message as NetInitialization;
            int id = responess.Id;
            string receiveToken = responess.Token;
            string sendToken = Tools.GenerateToken();
            Client.instance.ConnectionResponse(true, id, sendToken, receiveToken);

            NetInitialization request = new NetInitialization();
            request.Token = receiveToken;
            Sender.TCP_SendToServer(request);
            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }
        private void OnWelcomeGameResponess(NetMessage message)
        {
            NetWelcomeGame responess = message as NetWelcomeGame;

            NetAuthentication request = new NetAuthentication();
            request.deviceID = Player.Instance.DeviceID;
            request.matchID = GameManager.Instance.id;
            Sender.TCP_SendToServer(request);
        }
        private void OnWelcomeMatchmakingResponess(NetMessage message)
        {
            NetWelcomeMatchmaking responess = message as NetWelcomeMatchmaking;

            NetAuthentication_M request = new NetAuthentication_M();
            request.deviceID = Player.Instance.DeviceID;
            Sender.TCP_SendToServer(request);
        }
        private void OnAuthenticationnResponess(NetMessage message)
        {
            NetAuthentication responess = message as NetAuthentication;
            GameManager.Instance.player.data = responess.player;

            NetSynchronic sync = new NetSynchronic();
            Sender.TCP_SendToServer(sync);

        }

        private void OnAuthenticationMResponess(NetMessage message)
        {
            NetAuthentication_M responess = message as NetAuthentication_M; 
            Debug.Log("Client: " + responess.player.nickname + "\nRank: " + responess.player.rank + "\nClass : " + responess.player.class_type.ToString());
            GameManager.Instance.player.data = responess.player;
            MainMenuUIManager.Instance.SetPlayerInfo(GameManager.Instance.player);

            NetSynchronicM request = new NetSynchronicM();
            Sender.TCP_SendToServer(request);

        }
        private void OnSynchronicResponess(NetMessage message)
        {
            NetSynchronic responess = message as NetSynchronic;
            GameManager.Instance.SyncGameData(responess.game_data);

            NetReconnect requests = new NetReconnect();
            Sender.TCP_SendToServer(requests);
        }
        private void OnSyncronicMResponess(NetMessage message)
        {
            NetSynchronicM responess = message as NetSynchronicM;
            if (responess.match_id != 0)
            {
                GameManager.Instance.id = responess.match_id;
                UnSubscibeOnLeaveMainMenu();
                RealtimeNetworking.Disconnected();
                RealtimeNetworking.SetConnection("46.101.238.234", responess.port);
                Invoke("Connect", 1);
            }
            else
                MainMenuUIManager.Instance.OnConnected();
        }
        private void OnMoveResponess(NetMessage message)
        {
            NetMove responess = message as NetMove;
            
            Hex to_hex = MapGenerator.Instance.GetHex(responess.to_hex.column, responess.to_hex.row);
            foreach (Unit unit in GameManager.Instance.units)
            {
                if(unit.special_id == responess.unit_id)
                {
                    unit.SetPath(to_hex);
                    break;
                }
            }
        }
        private void OnAttackResponess(NetMessage message)
        {
            NetAttack responess = message as NetAttack;

            Unit aliance_unit = null;
            Unit enemy_unit = null;

            foreach (var unit in GameManager.Instance.units)
            {
                if (unit.special_id == responess.unit_id)
                    aliance_unit = unit;
                else if (unit.special_id == responess.enemy_id)
                    enemy_unit = unit;
            }

            if(aliance_unit != null && enemy_unit != null)
                aliance_unit.SetAttack(enemy_unit);
        }

        private void OnAbilityResponess(NetMessage message)
        {
            NetAbility responess = message as NetAbility;
            Hex to_hex = MapGenerator.Instance.GetHex(responess.to_hex.column, responess.to_hex.row);
            foreach (Unit unit in GameManager.Instance.units)
            {
                if (unit.special_id == responess.unit_id)
                {
                    unit.SetAbility(responess.input, to_hex);
                    break;
                }
            }
                
        }
        private void OnJesterSpecialAbilityResponess(NetMessage message)
        {   
            NetJesterSpecialAbility responess = message as NetJesterSpecialAbility;

            Spawner spawner = new Spawner();
            Unit illusion = spawner.SpawnUnit(responess.unit);
            GameManager.Instance.units.Add(illusion);

            Hex to_hex = MapGenerator.Instance.GetHex(responess.to_hex.column, responess.to_hex.row);
            illusion.SetPath(to_hex);
            illusion.column = to_hex.column;
            illusion.row = to_hex.row;
        }

        private void OnSwordsmanSpecialAbilityResponess(NetMessage message)
        {
            NetSwordsmanSpecialAbility responess = message as NetSwordsmanSpecialAbility;
            Vector2Int swordsman_sa = new Vector2Int(responess.unit_id,responess.enemy_id);
            Swordsman.swordsman_passive.Enqueue(swordsman_sa);

        }

        private void OnEndTurnResponess(NetMessage message)
        {
            NetEndTurn responess = message as NetEndTurn;
            GameManager.Instance.EndTurn(responess.class_on_turn);         
        }
        private void OnEndGameResponess(NetMessage message)
        {
            NetEndGame responess = message as NetEndGame;
            GameManager.Instance.id = 0;
            // GameManager.Instance.OnGameEnd();
            GameUIManager.Instance.PlayWinAnimation(responess.class_win);

            UnSubscribeOnLeaveMatch();
            RealtimeNetworking.Disconnected();
            RealtimeNetworking.SetConnection("46.101.238.234", responess.port);
            Invoke("ConnectToMatchmaking", 2);
        }
        private void OnCreateTicketResponess(NetMessage message)
        {
            pollTicketCoroutine = StartCoroutine(PollTicket());
            MainMenuUIManager.Instance.StartFindingMatchResponess();
        }

        private void OnFindMatchResponess(NetMessage message)
        {
            StopCoroutine(pollTicketCoroutine);
            MainMenuUIManager.Instance.StopFindingMatchResponess();

            NetFindMatch responess = message as NetFindMatch;
            enemy = responess.enemy_data;

            MainMenuUIManager.Instance.ShowMatchFound(enemy);
        }

        private void OnAcceptMatchResponess(NetMessage message)
        {
            NetAcceptMatch responess = message as NetAcceptMatch;
            MainMenuUIManager.Instance.OnAcceptMatchResponess(responess.class_type);
        }
        private void OnDeclineMatchResponess(NetMessage message)
        {
            NetDeclineMatch responess = message as NetDeclineMatch;
            MainMenuUIManager.Instance.OnDeclineMatchResponess();
            enemy = null;
        }
        private void OnStopMatchFindingResponess(NetMessage message)
        {
            StopCoroutine(pollTicketCoroutine);
            MainMenuUIManager.Instance.StopFindingMatchResponess();
        }
        private void OnMatchCreatedResponess(NetMessage message)
        {
            NetMatchCreated responess = message as NetMatchCreated;
            GameManager.Instance.id = responess.match_id;
            UnSubscibeOnLeaveMainMenu();

            RealtimeNetworking.Disconnected();
            RealtimeNetworking.SetConnection("46.101.238.234", responess.port);
            MainMenuUIManager.Instance.BothPlayerAcceptMatch();
            Invoke("Connect", 2);
        }
        private void Connect()
        {
            SubscribeMatch();
            GameManager.Instance.player.ConnectedToServer();
            GameManager.Instance.main_menu_canvas.enabled = false;
            GameManager.Instance.game_canvas.enabled = true;
        }
        private void ConnectToMatchmaking()
        {
            GameManager.Instance.OnGameEnd();
            SubscibeMainMenu();
            GameManager.Instance.player.ConnectedToServer();
            GameManager.Instance.main_menu_canvas.enabled = true;
            GameManager.Instance.game_canvas.enabled = false;
        }

        private void OnChangeNicknameResponess(NetMessage message)
        {
            NetChangeNickname responess = message as NetChangeNickname;
            MainMenuUIManager.Instance.ChangeNickname(responess.nickname);
        }

        private void OnChatMResponess(NetMessage message)
        {
            NetChat_M responess = message as NetChat_M;
            MainMenuUIManager.Instance.chat.AddMessage(responess.message,responess.nickname);
        }

        private void OnChatResponess(NetMessage message)
        {
            NetChat responess = message as NetChat;
            GameUIManager.Instance.chat.AddMessage(responess.message, responess.nickname);
        }

        private void OnDisconnectResponess(NetMessage message)
        {
            NetDisconnect responess = message as NetDisconnect;
            GameUIManager.Instance.OnDisconnect(responess.nickname);
        }

        private void OnReconnectResponess(NetMessage message)
        {
            NetReconnect responess = message as NetReconnect;
            GameUIManager.Instance.OnReconnect();
        }
        private void OnErrortResponess(NetMessage message)
        {
            NetError responess = message as NetError;

            switch (responess.Error)
            {

                default:
                    break;
            }
        }

        public static void ReceiveCustom(Packet packet)
        {
            if (packet != null)
            {
                RealtimeNetworking.ReceivePacket(packet);
            }
        }

        private IEnumerator PollTicket()
        {
            while (true)
            {
                NetFindMatch request = new NetFindMatch();
                Sender.TCP_SendToServer(request);

                yield return new WaitForSeconds(6);
            }

        }

    }
}