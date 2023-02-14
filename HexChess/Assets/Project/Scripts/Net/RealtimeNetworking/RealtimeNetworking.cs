namespace Networking.Client
{
    using System;
    using UnityEngine;
    public enum OpCode
    {
        INIT = 1,
        AUTH = 2,
        SYNC = 3,
        MOVE = 4,
        ATTACK = 5,
        ABILITY = 6,
        JESTER_SA = 7,
        END_TURN = 8,
        SWORDSMAN_SA = 9,
        CHAT = 10,
        DISCONNECT = 11,
        RECONNECT = 12,
        END_GAME = 13,
        SURREND = 14,

        ON_WELCOME_GAME = 99,
        ON_WELCOME_MATCHMAKING = 100,

        AUTHENTICATION_M = 101,
        SYNC_M = 102,
        CREATE_TICKET = 103,
        FIND_MATCH = 104,
        ACCEPT_MATCH = 105,
        MATCH_CREATED = 106,
        STOP_MATCH_FINDING = 107,
        DECLINE_MATCH = 108,
        NICKNAME = 109,
        CHAT_M = 110,


        ERROR = 404,
    }

    public class RealtimeNetworking : MonoBehaviour
    {
        
      

        private bool _initialized = false;

        private static RealtimeNetworking _instance = null; public static RealtimeNetworking instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RealtimeNetworking>();
                    if (_instance == null)
                    {
                        _instance = Client.instance.gameObject.AddComponent<RealtimeNetworking>();
                    }
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
        }
        public static void Disconnected()
        {
            Client.instance.Disconnect();
        }
        public static void SetConnection(string ip_address, int port)
        {
            Client.instance.ip = ip_address;
            Client.instance.port = port;
        }
        public static void Connect()
        {
            Client.instance.ConnectToServer();
        }

        public void _Connection(bool result)
        {
            if (OnConnectingToServerResult != null)
            {
                OnConnectingToServerResult.Invoke(result);
            }
        }

        public void _Disconnected()
        {
            if (OnDisconnectedFromServer != null)
            {
                OnDisconnectedFromServer.Invoke();
            }
        }

        public static void ReceivePacket(Packet packet)
        {
            NetMessage msg = null;

            OpCode opCode = (OpCode)packet.ReadByte();
            switch (opCode)
            {
                case OpCode.INIT:
                    msg = new NetInitialization(packet);
                    break;
                case OpCode.AUTH:
                    msg = new NetAuthentication(packet);
                    break;
                case OpCode.SYNC:
                    msg = new NetSynchronic(packet);
                    break;
                case OpCode.MOVE:
                    msg = new NetMove(packet);
                    break;
                case OpCode.ATTACK:
                    msg = new NetAttack(packet);
                    break;
                case OpCode.ABILITY:
                    msg = new NetAbility(packet);
                    break;
                case OpCode.JESTER_SA:
                    msg = new NetJesterSpecialAbility(packet);
                    break;
                case OpCode.SWORDSMAN_SA:
                    msg = new NetSwordsmanSpecialAbility(packet);
                    break;
                case OpCode.END_TURN:
                    msg = new NetEndTurn(packet);
                    break;
                case OpCode.SURREND:
                    msg = new NetSurrend(packet);
                    break;
                case OpCode.END_GAME:
                    msg = new NetEndGame(packet);
                    break;
                case OpCode.AUTHENTICATION_M:
                    msg = new NetAuthentication_M(packet);
                    break;
                case OpCode.SYNC_M:
                    msg = new NetSynchronicM(packet);
                    break;
                case OpCode.CREATE_TICKET:
                    msg = new NetCreateTicket(packet);
                    break;
                case OpCode.FIND_MATCH:
                    msg = new NetFindMatch(packet);
                    break;
                case OpCode.STOP_MATCH_FINDING:
                    msg = new NetStopMatchFinding(packet);
                    break;
                case OpCode.ACCEPT_MATCH:
                    msg = new NetAcceptMatch(packet);
                    break;
                case OpCode.DECLINE_MATCH:
                    msg = new NetDeclineMatch(packet);
                    break;
                case OpCode.MATCH_CREATED:
                    msg = new NetMatchCreated(packet);
                    break;
                case OpCode.ON_WELCOME_GAME:
                    msg = new NetWelcomeGame(packet);
                    break;
                case OpCode.ON_WELCOME_MATCHMAKING:
                    msg = new NetWelcomeMatchmaking(packet);
                    break;
                case OpCode.NICKNAME:
                    msg = new NetChangeNickname(packet);
                    break;
                case OpCode.CHAT_M:
                    msg = new NetChat_M(packet);
                    break;
                case OpCode.CHAT:
                    msg = new NetChat(packet);
                    break;
                case OpCode.DISCONNECT:
                    msg = new NetDisconnect(packet);
                    break;
                case OpCode.RECONNECT:
                    msg = new NetReconnect(packet);
                    break;
                case OpCode.ERROR:
                    msg = new NetError(packet);
                    break;
                default:
                    Debug.Log("Message received had no OpCode");
                    break;
            }
            msg.ReceivedOnClient();

        }
        #region Events
        public static event NoCallback OnDisconnectedFromServer;
        public static event ActionCallback OnConnectingToServerResult;
        public delegate void ActionCallback(bool successful);
        public delegate void NoCallback();
        #endregion
        #region NetMessages Events
        public static Action<NetMessage> C_INIT_RESPONESS;
        public static Action<NetMessage> C_AUTH_RESPONESS;
        public static Action<NetMessage> C_SYNC_RESPONESS;
        public static Action<NetMessage> C_MOVE_RESPONESS;
        public static Action<NetMessage> C_ATTACK_RESPONESS;
        public static Action<NetMessage> C_ABILITY_RESPONESS;
        public static Action<NetMessage> C_JESTER_SA_RESPONESS;
        public static Action<NetMessage> C_SWORDSMAN_SA_RESPONESS;
        public static Action<NetMessage> C_END_TURN_RESPONESS;
        public static Action<NetMessage> C_SURREND_RESPONESS;
        public static Action<NetMessage> C_END_GAME_RESPONESS;
        public static Action<NetMessage> C_WELCOME_GAME_RESPONESS;
        public static Action<NetMessage> C_CHAT_RESPONESS;
        public static Action<NetMessage> C_DISCONNECT_RESPONESS;
        public static Action<NetMessage> C_RECONNECT_RESPONESS;

        public static Action<NetMessage> C_AUTHENTICATION_RESPONESS;
        public static Action<NetMessage> C_SYNCHRONIC_RESPONESS;
        public static Action<NetMessage> C_CREATE_TICKET_RESPONESS;
        public static Action<NetMessage> C_FIND_MATCH_RESPONESS;
        public static Action<NetMessage> C_STOP_MATCH_FINDING_RESPONESS;
        public static Action<NetMessage> C_ACCEPT_MATCH_RESPONESS;
        public static Action<NetMessage> C_DECLINE_MATCH_RESPONESS;
        public static Action<NetMessage> C_MATCH_CREATED_RESPONESS;
        public static Action<NetMessage> C_WELCOME_MATCHMAKING_RESPONESS;
        public static Action<NetMessage> C_CHANGE_NICKNAME_RESPONESS;
        public static Action<NetMessage> C_CHAT_M_RESPONESS;

        public static Action<NetMessage> C_ERROR_RESPONESS;
        #endregion
    }
}