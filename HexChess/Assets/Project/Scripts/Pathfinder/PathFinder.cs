using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public static class PathFinder
    {
        public static List<Hex> FindPath_BFS(Hex start, Hex end)
        {

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.neighbors)
                {
                    if (neighbor.walkable)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);

                            neighbor.prev_tile = current;
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }

        public static List<Hex> FindPath_GreedyBestFirstSearch(Hex start, Hex end)
        {

            Comparison<Hex> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = GetEuclideanHeuristicCost(rhs, end);

                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
            frontier.Add(start);

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.neighbors)
                {
                    if (neighbor.walkable)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            frontier.Add(neighbor);
                            visited.Add(neighbor);

                            neighbor.prev_tile = current;
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }
        public static List<Hex> FindPath_Dijkstra(Hex start, Hex end)
        {

            foreach (Hex hex in MapGenerator.Instance.hexes.Values)
            {
                hex.cost = int.MaxValue;
            }

            start.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            MinHeap<Hex> frontier = new MinHeap<Hex>((lhs, rhs) => lhs.cost.CompareTo(rhs.cost));
            frontier.Add(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.neighbors)
                {
                    if(neighbor.walkable)
                    {
                        int newNeighborCost = current.cost + neighbor.weight;
                        if (newNeighborCost < neighbor.cost)
                        {
                            neighbor.cost = newNeighborCost;
                            neighbor.prev_tile = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Add(neighbor);
                        }
                    }
                  
                }
            }

            List<Hex> path = BacktrackToPath(end);

            return path;
        }
        
        public static List<Hex> FindPath_AStar(Hex start, Hex end)
        {
            foreach (Hex hex in MapGenerator.Instance.hexes.Values)
                hex.cost = int.MaxValue;

            start.cost = 0;

            Comparison<Hex> heuristicComparison = (lhs, rhs) =>
            {
                float lhsCost = lhs.cost + GetEuclideanHeuristicCost(lhs, end);
                float rhsCost = rhs.cost + GetEuclideanHeuristicCost(rhs, end);
               
                return lhsCost.CompareTo(rhsCost);
            };

            MinHeap<Hex> frontier = new MinHeap<Hex>(heuristicComparison);
            frontier.Add(start);

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Remove();

                if (current == end)
                {
                    break;
                }

                foreach (var neighbor in current.neighbors)
                {
                    if (neighbor.walkable)
                    {
                        int newNeighborCost = current.cost + neighbor.weight;
                        if (newNeighborCost < neighbor.cost)
                        {
                            neighbor.cost = newNeighborCost;
                            neighbor.prev_tile = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Add(neighbor);
                        }
                    }
                }
            }

            List<Hex> path = BacktrackToPath(end);
                        
            return path;
        }

        public static List<Hex> BFS_HexesMoveRange(Hex start, float MoveRange)
        {
            List<Hex> tilesInRange = new List<Hex>();

            foreach (Hex hex in MapGenerator.Instance.hexes.Values)
            {
                hex.cost = int.MaxValue;
            }

            start.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbor in current.neighbors)
                {
                    if (neighbor.walkable)
                    {
                        int newNeighborCost = current.cost + neighbor.weight;

                        if (newNeighborCost < neighbor.cost)
                        {
                            neighbor.cost = newNeighborCost;
                            neighbor.prev_tile = current;
                        }


                        if (!visited.Contains(neighbor))
                        {
                            if (MoveRange - newNeighborCost >= 0)
                            {
                                tilesInRange.Add(neighbor);
                            }
                            else
                            {
                                return tilesInRange;
                            }
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);
                        }
                    }
                }
            }
            return tilesInRange;
        }

        public static List<Hex> BFS_LongestPath(Hex start, Unit cast_unit)
        {
            List<Hex> longestPath = new List<Hex>();

            foreach (Hex tile in MapGenerator.Instance.hexes.Values)
            {
                tile.cost = int.MaxValue;
            }

            start.cost = 0;

            HashSet<Hex> visited = new HashSet<Hex>();
            visited.Add(start);

            Queue<Hex> frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            start.prev_tile = null;

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbor in current.neighbors)
                {
                    Unit unit = GameManager.Instance.GetUnit(neighbor);
                    if (unit != null && unit.class_type != cast_unit.class_type)
                    {
                        int newNeighborCost = current.cost + neighbor.weight;
                        if (newNeighborCost < neighbor.cost)
                        {
                            neighbor.cost = newNeighborCost;
                            neighbor.prev_tile = current;
                        }

                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            frontier.Enqueue(neighbor);
                        }

                    }

                }
            }
            Dictionary<Hex, int> pathdicitonary = new Dictionary<Hex, int>();

            foreach (Hex h in visited)
            {
                List<Hex> p = BacktrackToPath(h);
                pathdicitonary.Add(h, p.Count);
            }

            Hex hex = null;
            int max = 0;
            List<Hex> hexes = new List<Hex>();
            foreach (var p in pathdicitonary)
            {
                if (p.Value > max)
                {
                    hex = p.Key;
                    max = p.Value;
                    hexes.Add(hex);
                }
            }
            foreach (var p in pathdicitonary)
            {
                if (p.Value == max && p.Key != hex)
                {
                    hexes.Add(p.Key);
                }
            }


            if (hex != null)
            {
                longestPath = BacktrackToPath(hex);
            }

            return longestPath;
        }

        private static float GetEuclideanHeuristicCost(Hex current, Hex end)
        {
            float heuristicCost = (current.game_object.transform.position - end.game_object.transform.position).magnitude;
            return heuristicCost;
        }

        private static List<Hex> BacktrackToPath(Hex end)
        {
            Hex current = end;
            List<Hex> path = new List<Hex>();

            while (current != null)
            {
                path.Add(current);
                current = current.prev_tile;
            }

            path.Reverse();

            return path;
        }
    }
}
