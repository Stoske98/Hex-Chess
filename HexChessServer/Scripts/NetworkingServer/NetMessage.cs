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

    public class NetAuthentication : NetMessage
    {
        public string deviceID { get; set; }
        public int matchID { get; set; }
        public Data.Player player { get; set; }
        public NetAuthentication()
        {
            Code = OpCode.AUTH;
        }

        public NetAuthentication(Packet reader)
        {
            Code = OpCode.AUTH;
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
            matchID = reader.ReadInt();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_AUTH_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetSynchronic : NetMessage
    {
        public string xml_player_data { set; get; }
        public NetSynchronic()
        {
            Code = OpCode.SYNC;
        }

        public NetSynchronic(Packet reader)
        {
            Code = OpCode.SYNC;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(xml_player_data);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_SYNC_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetChat : NetMessage
    {
        public string nickname { set; get; }
        public string message { set; get; }
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

    public class NetDisconnect : NetMessage
    {
        public string nickname { set; get; }
        public NetDisconnect()
        {
            Code = OpCode.DISCONNECT;
        }

        public NetDisconnect(Packet reader)
        {
            Code = OpCode.DISCONNECT;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(nickname);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_DISCONNECT_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetReconnect : NetMessage
    {
        public NetReconnect()
        {
            Code = OpCode.RECONNECT;
        }

        public NetReconnect(Packet reader)
        {
            Code = OpCode.RECONNECT;
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
            Terminal.S_RECONNECT_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetSurrend : NetMessage
    {
        public NetSurrend()
        {
            Code = OpCode.SURREND;
        }

        public NetSurrend(Packet reader)
        {
            Code = OpCode.SURREND;
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
            Terminal.S_SURREND_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetEndGame : NetMessage
    {
        public Units.ClassType class_win;
        public string ip;
        public int port;
        public NetEndGame()
        {
            Code = OpCode.END_GAME;
        }

        public NetEndGame(Packet reader)
        {
            Code = OpCode.END_GAME;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)class_win);
            writer.WriteString(ip);
            writer.WriteInt(port);
        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_END_GAME_REQUEST?.Invoke(this, clientID);
        }

    }
    public class NetMove : NetMessage
    {
        public int end_turn = 0;
        public int unit_id { get; set; }
        public Data.Hex to_hex { set; get; }
        public NetMove()
        {
            Code = OpCode.MOVE;
        }

        public NetMove(Packet reader)
        {
            Code = OpCode.MOVE;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(to_hex.column);
            writer.WriteInt(to_hex.row);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            to_hex = new Data.Hex
            {
                column = reader.ReadInt(),
                row = reader.ReadInt(),
            };
            end_turn = reader.ReadInt();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_MOVE_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetAttack : NetMessage
    {
        public int end_turn = 0;
        public int unit_id { set; get; }
        public int enemy_id { set; get; }
        public NetAttack()
        {
            Code = OpCode.ATTACK;
        }

        public NetAttack(Packet reader)
        {
            Code = OpCode.ATTACK;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(enemy_id);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            enemy_id = reader.ReadInt();
            end_turn = reader.ReadInt();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_ATTACK_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetAbility : NetMessage
    {
        public int end_turn = 0;
        public AbilityInput input { set; get; }
        public int unit_id { set; get; }
        public Data.Hex to_hex { set; get; }
        public NetAbility()
        {
            Code = OpCode.ABILITY;
        }

        public NetAbility(Packet reader)
        {
            Code = OpCode.ABILITY;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)input);
            writer.WriteInt(unit_id);
            writer.WriteInt(to_hex.column);
            writer.WriteInt(to_hex.row);

        }

        public override void Deserialize(Packet reader)
        {
            input = (AbilityInput)reader.ReadByte();
            unit_id = reader.ReadInt();
            to_hex = new Data.Hex
            {
                column = reader.ReadInt(),
                row = reader.ReadInt(),
            };
            end_turn = reader.ReadInt();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_ABILITY_REQUEST?.Invoke(this, clientID);
        }
    }
    public class NetJesterSpecialAbility : NetMessage
    {
        public string xml_unit_data { set; get; }
        public Data.Hex to_hex { set; get; }
        public NetJesterSpecialAbility()
        {
            Code = OpCode.JESTER_SA;
        }

        public NetJesterSpecialAbility(Packet reader)
        {
            Code = OpCode.JESTER_SA;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteString(xml_unit_data);
            writer.WriteInt(to_hex.column);
            writer.WriteInt(to_hex.row);
        }

        public override void Deserialize(Packet reader)
        {
            
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_JESTER_SA_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetSwordsmanSpecialAbility : NetMessage
    {
        public int unit_id { set; get; }
        public int enemy_id { set; get; }
        public NetSwordsmanSpecialAbility()
        {
            Code = OpCode.SWORDSMAN_SA;
        }

        public NetSwordsmanSpecialAbility(Packet reader)
        {
            Code = OpCode.SWORDSMAN_SA;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteInt(unit_id);
            writer.WriteInt(enemy_id);

        }

        public override void Deserialize(Packet reader)
        {
            unit_id = reader.ReadInt();
            enemy_id = reader.ReadInt();
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_SWORDSMAN_SA_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetEndTurn : NetMessage
    {
        public Units.ClassType class_on_turn;
        public NetEndTurn()
        {
            Code = OpCode.END_TURN;
        }

        public NetEndTurn(Packet reader)
        {
            Code = OpCode.END_TURN;
            Deserialize(reader);
        }

        public override void Serialize(ref Packet writer)
        {
            writer.WriteByte((byte)Code);
            writer.WriteByte((byte)class_on_turn);

        }

        public override void Deserialize(Packet reader)
        {
        }

        public override void ReceivedOnServer(int clientID)
        {
            Terminal.S_END_TURN_REQUEST?.Invoke(this, clientID);
        }
    }

    public class NetWelcomeGame : NetMessage
    {
        public NetWelcomeGame()
        {
            Code = OpCode.ON_WELCOME_GAME;
        }

        public NetWelcomeGame(Packet reader)
        {
            Code = OpCode.ON_WELCOME_GAME;
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
            Terminal.S_WELCOME_GAME_REQUEST?.Invoke(this, clientID);
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