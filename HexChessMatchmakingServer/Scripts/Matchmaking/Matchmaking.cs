using System;
using System.Collections.Generic;
using System.Text;
using Networking.Server;

namespace Networking.Matchmaking
{
    public enum ClassType
    {
        None = 0,
        Light = 1,
        Dark = 2
    }

    class Matchmaking
    {
        public static List<Rank> list_of_ranks = new List<Rank>
        { 
            new Rank(0, 249,"Rank 1"),
            new Rank(250, 499, "Rank 2"),
            new Rank(500, 749, "Rank 3"),
            new Rank(750, 999, "Rank 4"),
            new Rank(1000, 1249, "Rank 5"),
            new Rank(1250, 1449, "Rank 6"),
            new Rank(1500, 1749, "Rank 7"),
            new Rank(1750, 1999, "Rank 8")
        };

        public static Queue<Ticket>[] queue_tickets;
        public static Match[] matches = new Match[Terminal.maxPlayers / 2];
        public static void Initialization()
        {
            queue_tickets = new Queue<Ticket>[list_of_ranks.Count];
            for (int i = 0; i < list_of_ranks.Count; i++)
                queue_tickets[i] = new Queue<Ticket>();

        }

        public static void CreateTicketRequest(int clientID, ClassType class_type)
        {
            Player player = Server.Server.clients[clientID].player;
            if(string.IsNullOrEmpty(player.ticket_id))
            {
                string _ticket_id = Guid.NewGuid().ToString();

                player.data.class_type = class_type;

                Ticket ticket = new Ticket
                {
                    ticket_id = _ticket_id,
                    player = player,
                };

                int tickets_position = GetTicketsPosition(ticket.player.data.rank);
                queue_tickets[tickets_position].Enqueue(ticket);

                player.ticket_id = _ticket_id;

                NetCreateTicket responess = new NetCreateTicket();
                Sender.TCP_SendToClient(clientID, responess);
            }     
        }

        public static void FindMatchRequest(int clientID)
        {
            Player player = Server.Server.clients[clientID].player;
            int tickets_position = GetTicketsPosition(player.data.rank);

            Ticket player_ticket = null;
            Ticket enemy_ticket = null;

            foreach (Ticket ticket in queue_tickets[tickets_position])
            {
                if (ticket.ticket_id == player.ticket_id)
                    player_ticket = ticket;
            }
            if (player_ticket == null)
            {
                Console.WriteLine(player.data.nickname + " ticket ne postoji !!!");
                return;
            }
                        
            if (queue_tickets[tickets_position].Count != 0)
            {
                enemy_ticket = queue_tickets[tickets_position].Peek();
                if(enemy_ticket != null && enemy_ticket != player_ticket)
                {
                    if (player_ticket.player.data.class_type != enemy_ticket.player.data.class_type)
                    {
                        CreateMatch(player_ticket.player, enemy_ticket.player);

                        queue_tickets[tickets_position].Dequeue();
                        enemy_ticket.player.ticket_id = "";

                        DeleteTicket(player);
                        return;
                    }
                }                   
            }
        }
        private static void CreateMatch(Player _player1, Player _player2)
        {
            
            Match match = new Match
            {
                player1 = _player1,
                player2 = _player2
            };
            int match_position = GetFreeMatchPosition();
            if (match_position != -1)
            {
                _player1.match_id = match_position;
                _player2.match_id = match_position;
                matches[match_position] = match;
            }
            NetFindMatch responess = new NetFindMatch();

            responess.enemy = _player2.data;
            Sender.TCP_SendToClient(_player1.id, responess);

            responess.enemy = _player1.data;
            Sender.TCP_SendToClient(_player2.id, responess);
        }

        public static void IsMatchAccepted(int clientID, string xml_team_structure)
        {
            Player player = Server.Server.clients[clientID].player;
            if(player.match_id != -1)
            {
                Match match = matches[player.match_id];
                if (match != null)
                {
                    match.Accept(player, xml_team_structure);
                    if (match.IsReady())
                        Database.CreateMatch(match);
                }
            }
        }
        public static void StopMatchFindingRequest(int clientID)
        {
            Player player = Server.Server.clients[clientID].player;
            if(!string.IsNullOrEmpty(player.ticket_id))
            {
                DeleteTicket(player);

                NetStopMatchFinding responess = new NetStopMatchFinding();
                Sender.TCP_SendToClient(clientID, responess);
            }
        }
        public static void DeleteTicket(Player player) 
        {
            int tickets_position = GetTicketsPosition(player.data.rank);

            Queue<Ticket> currentTicketQueue = new Queue<Ticket>();
            while (queue_tickets[tickets_position].Count != 0)
            {
                if (queue_tickets[tickets_position].Peek().ticket_id != player.ticket_id)
                    currentTicketQueue.Enqueue(queue_tickets[tickets_position].Dequeue());
                else
                {
                    queue_tickets[tickets_position].Dequeue();
                    player.ticket_id = "";
                }
            }

            queue_tickets[tickets_position] = currentTicketQueue;
        }

        public static void DeclineMatchRequest(int clientID)
        {
            Player player = Server.Server.clients[clientID].player;
            if(player.match_id != -1)
            {
                DeleteMatch(player);

                NetDeclineMatch responess = new NetDeclineMatch();
                responess.class_type = player.data.class_type;
                Sender.TCP_SendToClient(clientID, responess);
            }
        }

        public static void DeleteMatch(Player player)
        {
            int match_id = player.match_id;
            Match match = matches[match_id];
            if(match != null)
            {
                Player enemy = match.GetOpponent(player);

                enemy.match_id = -1;
                player.match_id = -1;
                matches[match_id] = null;

                NetDeclineMatch responess = new NetDeclineMatch();
                responess.class_type = player.data.class_type;
                Sender.TCP_SendToClient(enemy.id, responess);
            }
        }
        /*private Rank GetRank(int mmr)
        {
            Rank my_rank = null;
            foreach (Rank rank in list_of_ranks)
            {
                if (rank.min <= mmr && mmr <= rank.max)
                {
                    my_rank = rank;
                    break;
                }
            }
            return my_rank;
        }*/
        public static int GetTicketsPosition(int mmr)
        {
            int position = -1;
            foreach (Rank rank in list_of_ranks)
            {
                ++position;
                if (rank.min <= mmr && mmr <= rank.max)
                {
                    break;
                }
            }
            return position;
        }
        public static int GetFreeMatchPosition()
        {
            int freeIndex = -1;
            for (int i = 0; i < matches.Length; i++)
            {
                if (matches[i] == null)
                {
                    freeIndex = i;
                    break;
                }
            }
            return freeIndex;
        }
    }

    class Match
    {
        public Player player1 { get; set; }
        public bool player1_accepted { get; set; }
        public string xml_team1_structure { get; set; }
        public Player player2 { get; set; }
        public bool player2_accepted { get; set; }
        public string xml_team2_structure { get; set; }

        public void Accept(Player player, string xml_team_structure)
        {
            if (player1 == player)
            {
                player1_accepted = true;
                xml_team1_structure = xml_team_structure;
            }
            else
            {
                player2_accepted = true;
                xml_team2_structure = xml_team_structure;
            }

            NetAcceptMatch responess = new NetAcceptMatch();
            responess.class_type = player.data.class_type;

            Sender.TCP_SendToClient(player1.id,responess);
            Sender.TCP_SendToClient(player2.id, responess);
        }
        public bool IsReady()
        {
            if (player1_accepted && player2_accepted)
                return true;
            return false;
        }

        public Player GetOpponent(Player player)
        {
            if (player == player1)
                return player2;
            else
                return player1;
        }
    }


    class Ticket
    {
        public string ticket_id { get; set; }
        public Player player { get; set; }
        public int give_up { get; set; }

    }

    public class Rank
    {
        public int min { get; set; }
        public int max { get; set; }
        public string name { get; set; }

        public Rank(int _min, int _max, string _name)
        {
            min = _min;
            max = _max;
            name = _name;
        }
    }
}
