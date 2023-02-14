namespace Networking.Server
{
    public class NetMessage
    {
        public OpCode Code { set; get; }

        public virtual void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }
        public virtual void Deserialize(Packet reader)
        {
        }
        public virtual void ReceivedOnServer(int clientID)
        {

        }
    }

    public class NetInitialization : NetMessage
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public NetInitialization()
        {
            Code = OpCode.INIT;
        }

        public NetInitialization(Packet reader)
        {
            Code = OpCode.INIT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(Id);
            writer.WriteString(Token);
        }

        public override void Deserialize(Packet reader)
        {
            Token = reader.ReadString();
        }
        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_INIT_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetWelcomeMatchmaking : NetMessage
    {
        public NetWelcomeMatchmaking()
        {
            Code = OpCode.ON_WELCOME_MATCHMAKING;
        }

        public NetWelcomeMatchmaking(Packet reader)
        {
            Code = OpCode.ON_WELCOME_MATCHMAKING;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);

        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_WELCOME_MATCHMAKING_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetAuthentication : NetMessage
    {
        public string deviceID { get; set; }
        public Data.Player player { get; set; }
        public NetAuthentication()
        {
            Code = OpCode.AUTHENTICATION;
        }

        public NetAuthentication(Packet reader)
        {
            Code = OpCode.AUTHENTICATION;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(player.nickname);
            writer.WriteInt(player.rank);
            writer.WriteByte((byte)player.class_type);
        }

        public override void Deserialize(Packet reader)
        {
            deviceID = reader.ReadString();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_AUTHENTICATION_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetSynchronic : NetMessage
    {
        public int match_id { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public NetSynchronic()
        {
            Code = OpCode.SYNC_M;
        }

        public NetSynchronic(Packet reader)
        {
            Code = OpCode.SYNC_M;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(match_id);
            writer.WriteString(ip);
            writer.WriteInt(port);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_SYNC_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetChangeNickname : NetMessage
    {
        public string nickname { get; set; }
        public NetChangeNickname()
        {
            Code = OpCode.NICKNAME;
        }

        public NetChangeNickname(Packet reader)
        {
            Code = OpCode.NICKNAME;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(nickname);
        }

        public override void Deserialize(Packet reader)
        {
            nickname = reader.ReadString();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_CHANGE_NICKNAME_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetChat : NetMessage
    {
        public string nickname { get; set; }
        public string message { get; set; }
        public NetChat()
        {
            Code = OpCode.CHAT;
        }

        public NetChat(Packet reader)
        {
            Code = OpCode.CHAT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(nickname);
            writer.WriteString(message);
        }

        public override void Deserialize(Packet reader)
        {
            message = reader.ReadString();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_CHAT_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetCreateTicket : NetMessage
    {
        public Matchmaking.ClassType class_type;
        public NetCreateTicket()
        {
            Code = OpCode.CREATE_TICKET;
        }

        public NetCreateTicket(Packet reader)
        {
            Code = OpCode.CREATE_TICKET;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
            class_type = (Matchmaking.ClassType)reader.ReadByte();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_CREATE_TICKET_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetFindMatch : NetMessage
    {
        public Data.Player enemy;
        public NetFindMatch()
        {
            Code = OpCode.FIND_MATCH;
        }

        public NetFindMatch(Packet reader)
        {
            Code = OpCode.FIND_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(enemy.nickname);
            writer.WriteInt(enemy.rank);
            writer.WriteByte((byte)enemy.class_type);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_FIND_MATCH_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetStopMatchFinding : NetMessage
    {
        public NetStopMatchFinding()
        {
            Code = OpCode.STOP_MATCH_FINDING;
        }

        public NetStopMatchFinding(Packet reader)
        {
            Code = OpCode.STOP_MATCH_FINDING;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_STOP_MATCH_FINDING_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetAcceptMatch : NetMessage
    {
        public Matchmaking.ClassType class_type;
        public string xml_team_structure;
        public NetAcceptMatch()
        {
            Code = OpCode.ACCEPT_MATCH;
        }

        public NetAcceptMatch(Packet reader)
        {
            Code = OpCode.ACCEPT_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)class_type);
        }

        public override void Deserialize(Packet reader)
        {
            xml_team_structure = reader.ReadString();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_ACCEPT_MATCH_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetDeclineMatch : NetMessage
    {
        public Matchmaking.ClassType class_type;
        public NetDeclineMatch()
        {
            Code = OpCode.DECLINE_MATCH;
        }

        public NetDeclineMatch(Packet reader)
        {
            Code = OpCode.DECLINE_MATCH;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code); 
            writer.WriteByte((byte)class_type);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_DECLINE_MATCH_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetMatchCreated : NetMessage
    {
        public int match_id;
        public string ip;
        public int port;
        public NetMatchCreated()
        {
            Code = OpCode.MATCH_CREATED;
        }

        public NetMatchCreated(Packet reader)
        {
            Code = OpCode.MATCH_CREATED;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(match_id);
            writer.WriteString(ip);
            writer.WriteInt(port);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_MATCH_CREATED_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetError : NetMessage
    {
        public Error Error { set; get; }
        public NetError()
        {
            Code = OpCode.ERROR;
        }

        public NetError(Packet reader)
        {
            Code = OpCode.ERROR;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)Error);
        }

        public override void Deserialize(Packet reader)
        {
            Error = (Error)reader.ReadByte();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_ERROR_REQUEST?.Invoke(this, clientID);
        }
    }

    public enum AbilityInput
    {
        Q = 0,
        W = 1,
        E = 2,
        S = 3,
    }
    public enum Error
    {
    }

}