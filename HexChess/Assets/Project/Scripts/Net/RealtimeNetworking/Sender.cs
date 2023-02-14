namespace Networking.Client
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Sender : MonoBehaviour
    {

        #region Core
        private static void SendTCPData(NetMessage msg)
        {
            Packet _packet = new Packet();
            msg.Serialize(ref _packet);
            _packet.WriteLength();
            Client.instance.tcp.SendData(_packet);
        }

        private static void SendUDPData(NetMessage msg)
        {
            Packet _packet = new Packet();
            msg.Serialize(ref _packet);
            _packet.WriteLength();
            Client.instance.udp.SendData(_packet);
        }
        #endregion

        #region TCP
        
        public static void TCP_SendToServer(NetMessage msg)
        {
            SendTCPData(msg);
        }
        #endregion
        
        #region UDP
     

        public static void UDP_SendToServer(NetMessage msg)
        {
                SendUDPData(msg);
        }
        #endregion

    }
}