using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Server
{
    class Receiver
    {
        public static void OnStart()
        {
            Terminal.S_INIT_REQUEST += OnInitializationRequest;
            Terminal.S_AUTH_REQUEST += OnAuthenticationRequest;
            Terminal.S_SYNC_REQUEST += OnSynchronicRequest;
            Terminal.S_MOVE_REQUEST += OnMoveRequest;
            Terminal.S_ATTACK_REQUEST += OntAttackRequest;
            Terminal.S_ABILITY_REQUEST += OnAbilityRequest;
            Terminal.S_CHAT_REQUEST += OnChatRequest;
            Terminal.S_RECONNECT_REQUEST += OnReconnectRequest;
            Terminal.S_SURREND_REQUEST += OnSurrendRequest;
            Terminal.S_END_GAME_REQUEST += OnEndGameRequest;

            //Database.TestFunction();
        }

        private static void OnInitializationRequest(NetMessage message, int clientID)
        {
            NetInitialization request = message as NetInitialization;
            string token = request.Token;
            Server.clients[clientID].receiveToken = token;
        }
        private static void OnAuthenticationRequest(NetMessage message, int clientID)
        {
            NetAuthentication request = message as NetAuthentication;
            Database.AuthenticatePlayer(clientID, request.deviceID, request.matchID);

        }

        private static void OnSynchronicRequest(NetMessage message, int clientID)
        {
            NetSynchronic request = message as NetSynchronic;
            Database.SyncGameData(clientID);
        }

        private static void OnMoveRequest(NetMessage message, int clientID)
        {
            NetMove request = message as NetMove;
            Database.MoveUnit(clientID, request);    
        }
        private static void OntAttackRequest(NetMessage message, int clientID)
        {
            NetAttack request = message as NetAttack;
            Database.AttackUnit(clientID, request);
        }
        private static void OnAbilityRequest(NetMessage message, int clientID)
        {
            NetAbility request = message as NetAbility;
            Database.UseUnitAbility(clientID, request);
        }

        private static void OnChatRequest(NetMessage message, int clientID)
        {
            NetChat request = message as NetChat;
            Database.SendMessage(clientID, request);
        }
        private static void OnReconnectRequest(NetMessage message, int clientID)
        {
            NetReconnect request = message as NetReconnect;
            Database.OnPlayerReconnect(clientID);
        }

        private static void OnEndGameRequest(NetMessage message, int clientID)
        {
            Database.SendWinner(clientID);
        }
        private static void OnSurrendRequest(NetMessage message, int clientID)
        {
            Database.UpdateWinnerAndSendToClients(clientID);
        }
        
        public static void ReceiveCustom(int clientID, Packet packet)
        {
            if (packet != null)
            {
                Terminal.ReceivedPacket(clientID, packet);
            }
        }

    }
}