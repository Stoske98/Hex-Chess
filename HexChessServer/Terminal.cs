using System;
using System.Numerics;

namespace Networking.Server
{
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
        public const int port = 27001;
        public static void OnClientConnected(int id, string ip)
        {
            NetWelcomeGame ressponess = new NetWelcomeGame();
            Sender.TCP_SendToClient(id,ressponess);
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

            switch(opCode)
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
                case OpCode.ON_WELCOME_GAME:
                    msg = new NetWelcomeGame(packet);
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
                    Console.WriteLine("Message received had no OpCode");
                    return;
                break;
            }

            msg.ReceivedOnServer(clientID);
        }
        #endregion

        #region NetMessages Events
        public static Action<NetMessage, int> S_INIT_REQUEST;
        public static Action<NetMessage, int> S_AUTH_REQUEST;
        public static Action<NetMessage, int> S_SYNC_REQUEST;
        public static Action<NetMessage, int> S_MOVE_REQUEST;
        public static Action<NetMessage, int> S_ATTACK_REQUEST;
        public static Action<NetMessage, int> S_ABILITY_REQUEST;
        public static Action<NetMessage, int> S_JESTER_SA_REQUEST;
        public static Action<NetMessage, int> S_SWORDSMAN_SA_REQUEST;
        public static Action<NetMessage, int> S_END_TURN_REQUEST;
        public static Action<NetMessage, int> S_SURREND_REQUEST;
        public static Action<NetMessage, int> S_END_GAME_REQUEST;
        public static Action<NetMessage, int> S_WELCOME_GAME_REQUEST;
        public static Action<NetMessage, int> S_CHAT_REQUEST;
        public static Action<NetMessage, int> S_DISCONNECT_REQUEST;
        public static Action<NetMessage, int> S_RECONNECT_REQUEST;

        public static Action<NetMessage, int> S_ERROR_REQUEST;
        #endregion
    }

}