using System;
using System.Net;
using System.Net.Sockets;

namespace Networking.Server
{
    public class Player
    {
        public int id;
        public string device_id = "";
        public Data.Player data;


        public int match_id = 0;
        public string ticket_id = "";

        public Player(int _id, string _device_id, Data.Player _player_data)
        {
            id = _id;
            device_id = _device_id;
            data = _player_data;
            match_id = -1;
        }

        public void OnDestroy()
        {
            if (!string.IsNullOrEmpty(ticket_id))
                Matchmaking.Matchmaking.DeleteTicket(this);
            if (match_id != -1)
                Matchmaking.Matchmaking.DeleteMatch(this);
        }
    }
    class Client
    {

        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public UDP udp;
        public string sendToken = "xxxxx";
        public string receiveToken = "xxxxx";

        public Player player;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Initialize(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                stream = socket.GetStream();
                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, IncomingData, null);
                NetInitialization msg = new NetInitialization
                {
                    Id = id,
                    Token = Server.clients[id].sendToken
                };
                Sender.TCP_SendToClient(id,msg);
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending data to client id {0} via TCP: {1}", id, ex.Message);
                }
            }

            private void IncomingData(IAsyncResult result)
            {
                try
                {
                    int length = stream.EndRead(result);
                    if (length <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }
                    byte[] data = new byte[length];
                    Array.Copy(receiveBuffer, data, length);
                    receivedData.Reset(CheckData(data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, IncomingData, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error receiving TCP data: {0}", ex.Message);
                    Server.clients[id].Disconnect();
                }
            }

            private bool CheckData(byte[] _data)
            {
                int length = 0;
                receivedData.SetBytes(_data);
                if (receivedData.UnreadLength() >= 4)
                {
                    length = receivedData.ReadInt();
                    if (length <= 0)
                    {
                        return true;
                    }
                }
                while (length > 0 && length <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(length);
                    Threading.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            Server.packetHandlers(id, _packet);
                        }
                    });
                    length = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        length = receivedData.ReadInt();
                        if (length <= 0)
                        {
                            return true;
                        }
                    }
                }
                if (length <= 1)
                {
                    return true;
                }
                return false;
            }

            public void Disconnect()
            {
                
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;
            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            public void SendData(Packet _packet)
            {
                Server.SendDataUDP(endPoint, _packet);
            }

            public void CheckData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);
                Threading.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        Server.packetHandlers(id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }

        private void Disconnect()
        {
            Console.WriteLine("{0} has been disconnected.", tcp.socket.Client.RemoteEndPoint);
            IPEndPoint ip = tcp.socket.Client.RemoteEndPoint as IPEndPoint;
            Terminal.OnClientDisconnected(id, ip.Address.ToString());
            tcp.Disconnect();
            udp.Disconnect();

            if(player != null)
                player.OnDestroy();
            player = null;
        }

    }
}