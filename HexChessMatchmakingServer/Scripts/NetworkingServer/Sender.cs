using System;
using System.Numerics;

namespace Networking.Server
{
    class Sender
    {

        #region Core
        /// <summary>Sends a packet to a client via TCP.</summary>
        /// <param name="clientID">The client to send the packet the packet to.</param>
        /// <param name="packet">The packet to send to the client.</param>
        private static void SendTCPData(int clientID, NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            Server.clients[clientID].tcp.SendData(packet);
        }

        /// <summary>Sends a packet to a client via UDP.</summary>
        /// <param name="clientID">The client to send the packet the packet to.</param>
        /// <param name="packet">The packet to send to the client.</param>
        private static void SendUDPData(int clientID, NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            Server.clients[clientID].udp.SendData(packet);
        }

        /// <summary>Sends a packet to all clients via TCP.</summary>
        /// <param name="packet">The packet to send.</param>
        private static void SendTCPDataToAll(NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        /// <summary>Sends a packet to all clients except one via TCP.</summary>
        /// <param name="exceptClientID">The client to NOT send the data to.</param>
        /// <param name="packet">The packet to send.</param>
        private static void SendTCPDataToAll(int exceptClientID, NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClientID)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        /// <summary>Sends a packet to all clients via UDP.</summary>
        /// <param name="packet">The packet to send.</param>
        private static void SendUDPDataToAll(NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }

        /// <summary>Sends a packet to all clients except one via UDP.</summary>
        /// <param name="exceptClientID">The client to NOT send the data to.</param>
        /// <param name="packet">The packet to send.</param>
        private static void SendUDPDataToAll(int exceptClientID, NetMessage msg)
        {
            Packet packet = new Packet();
            msg.Serialize(ref packet);
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != exceptClientID)
                {
                    Server.clients[i].udp.SendData(packet);
                }
            }
        }
        #endregion

        #region TCP
        

        public static void TCP_SendToClient(int clientID, NetMessage msg)
        {
            SendTCPData(clientID, msg);
        }

        public static void TCP_SentToAll(NetMessage msg)
        {
            SendTCPDataToAll(msg);
        }

        public static void TCP_SentToAllExeptOneClient(int excludedClientID,NetMessage msg)
        {
            SendTCPDataToAll(excludedClientID, msg);
        }

       
        #endregion

        #region UDP
      
        public static void UDP_SendToClient(int clientID, NetMessage msg)
        {
            SendUDPData(clientID, msg);
        }

        public static void UDP_SentToAll(NetMessage msg)
        {
            SendUDPDataToAll(msg);
        }

        public static void UDP_SentToAllExeptOneClient(int excludedClientID, NetMessage msg)
        {
            SendUDPDataToAll(excludedClientID, msg);
        }

       
        #endregion

    }
}