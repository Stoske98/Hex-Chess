using System;
using System.Numerics;

namespace Networking.Server
{
    public enum OpCode
    {
        INIT = 1,
        ON_WELCOME_MATCHMAKING = 100,
        AUTHENTICATION = 101,
        SYNC_M = 102,
        CREATE_TICKET = 103,
        FIND_MATCH = 104,
        ACCEPT_MATCH = 105,
        MATCH_CREATED = 106,
        STOP_MATCH_FINDING = 107,
        DECLINE_MATCH = 108,
        NICKNAME = 109,
        CHAT = 110,

        ERROR = 404,
    }
    partial class Terminal
    {

        #region Update
        public const int updatesPerSecond = 30;
        public static void Update()
        {

        }
        #endregion

        #region Connection
        public const int maxPlayers = 100000;
        public const int port = 27000;
        public static void OnClientConnected(int id, string ip)
        {
            NetWelcomeMatchmaking ressponess = new NetWelcomeMatchmaking();
            Sender.TCP_SendToClient(id, ressponess);
        }

        public static void OnClientDisconnected(int id, string ip)
        {

        }
        #endregion

        #region Data


        public static void ReceivedPacket(int clientID, Packet packet)
        {
            NetMessage msg = null;
            OpCode opCode = (OpCode)packet.ReadByte();

            switch (opCode)
            {
                case OpCode.INIT:
                    msg = new NetInitialization(packet);
                    break;
                case OpCode.ON_WELCOME_MATCHMAKING:
                    msg = new NetWelcomeMatchmaking(packet);
                    break;
                case OpCode.AUTHENTICATION:
                    msg = new NetAuthentication(packet);
                    break;
                case OpCode.SYNC_M:
                    msg = new NetSynchronic(packet);
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
                case OpCode.NICKNAME:
                    msg = new NetChangeNickname(packet);
                    break;
                case OpCode.CHAT:
                    msg = new NetChat(packet);
                    break;
                case OpCode.ERROR:
                    msg = new NetError(packet);
                    break;

                default:
                    Console.WriteLine("Message received had no OpCode");
                    break;
            }

            msg.ReceivedOnServer(clientID);
        }
        #endregion

        #region NetMessages Events
        public static Action<NetMessage, int> S_INIT_REQUEST;
        public static Action<NetMessage, int> S_WELCOME_MATCHMAKING_REQUEST;
        public static Action<NetMessage, int> S_AUTHENTICATION_REQUEST;
        public static Action<NetMessage, int> S_SYNC_REQUEST;
        public static Action<NetMessage, int> S_CREATE_TICKET_REQUEST;
        public static Action<NetMessage, int> S_FIND_MATCH_REQUEST;
        public static Action<NetMessage, int> S_STOP_MATCH_FINDING_REQUEST;
        public static Action<NetMessage, int> S_ACCEPT_MATCH_REQUEST;
        public static Action<NetMessage, int> S_DECLINE_MATCH_REQUEST;
        public static Action<NetMessage, int> S_MATCH_CREATED_REQUEST;
        public static Action<NetMessage, int> S_CHANGE_NICKNAME_REQUEST;
        public static Action<NetMessage, int> S_CHAT_REQUEST;

        public static Action<NetMessage, int> S_ERROR_REQUEST;
        #endregion
    }

}