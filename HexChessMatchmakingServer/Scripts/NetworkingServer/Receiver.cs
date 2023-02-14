using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Networking.Matchmaking;

namespace Networking.Server
{
    class Receiver
    {
        public static void OnStart()
        {
            Matchmaking.Matchmaking.Initialization();
            Terminal.S_INIT_REQUEST += OnInitializationRequest;
            Terminal.S_AUTHENTICATION_REQUEST += OnAuthenticationRequest;
            Terminal.S_SYNC_REQUEST += OnSynchronicRequest;
            Terminal.S_CREATE_TICKET_REQUEST += OnCreateTicketRequest;
            Terminal.S_FIND_MATCH_REQUEST += OnFindMatchRequest;
            Terminal.S_STOP_MATCH_FINDING_REQUEST += OnStopMatchFindingRequest;
            Terminal.S_ACCEPT_MATCH_REQUEST += OnAcceptMatchRequest;
            Terminal.S_DECLINE_MATCH_REQUEST += OnDeclineMatchhRequest;
            Terminal.S_CHANGE_NICKNAME_REQUEST += OnChangeNicknameRequest;
            Terminal.S_CHAT_REQUEST += OnChatRequest;
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
             Database.AuthenticatePlayer(clientID, request.deviceID);

        }

        private static void OnSynchronicRequest(NetMessage message, int clientID)
        {
            NetSynchronic request = message as NetSynchronic;
            Database.IsGameExist(clientID);
        }
        private static void OnCreateTicketRequest(NetMessage message, int clientID)
        {
            NetCreateTicket request = message as NetCreateTicket;
            Matchmaking.Matchmaking.CreateTicketRequest(clientID, request.class_type);
            
        }
        private static void OnFindMatchRequest(NetMessage message, int clientID)
        {
            Matchmaking.Matchmaking.FindMatchRequest(clientID);
        }

        private static void OnStopMatchFindingRequest(NetMessage message, int clientID)
        {
            Matchmaking.Matchmaking.StopMatchFindingRequest(clientID);
        }

        private static void OnAcceptMatchRequest(NetMessage message, int clientID)
        {
            NetAcceptMatch request = message as NetAcceptMatch;
            Matchmaking.Matchmaking.IsMatchAccepted(clientID, request.xml_team_structure);
        }

        private static void OnDeclineMatchhRequest(NetMessage message, int clientID)
        {
            Matchmaking.Matchmaking.DeclineMatchRequest(clientID);
        }

        private static void OnChangeNicknameRequest(NetMessage message, int clientID)
        {
            NetChangeNickname request = message as NetChangeNickname;
            Database.ChangePlayerNickname(request.nickname, clientID);
        }
        private static void OnChatRequest(NetMessage message, int clientID)
        {
            NetChat request = message as NetChat;
            request.nickname = Server.clients[clientID].player.data.nickname;
            Sender.TCP_SentToAll(request);
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