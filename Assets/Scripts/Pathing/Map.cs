using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    public List<Node> graph;

    private void Start()
    {
        instance = this;
    }

    public Dictionary<Node, List<PathComponent>> CalculateShortestPaths(Node start, int seed)
    {
        Rng rng = new Rng(seed);
        Dictionary<Node, int> distances = new Dictionary<Node, int>();
        Dictionary<Node, Node> predecessors = new Dictionary<Node, Node>();
        List<string> nodeQ = new List<string>();

        // Use Dijkstra's Algorithm to get shortest distances to each node
        foreach (Node node in graph)
        {
            distances[node] = Distance.MAX_DISTANCE;
            predecessors[node] = null;
            nodeQ.Add(node.name);
        }
        distances[start] = 0;
        while (nodeQ.Any())
        {
            // get node with shortest distance
            Node shortestNode = ShortestValue(distances, nodeQ, rng);
            nodeQ.Remove(shortestNode.name);
            foreach (Node neighbor in shortestNode.Neighbors)
            {
                int altDistance = distances[shortestNode] + shortestNode.NeighborEdge(neighbor).value;
                if (altDistance < distances[neighbor])
                {
                    distances[neighbor] = altDistance;
                    predecessors[neighbor] = shortestNode;
                }
            }
        }
        // convert to Dictionary<destination, List<PathComponent>>
        Dictionary<Node, List<PathComponent>> paths = new Dictionary<Node, List<PathComponent>>();

        foreach (Node node in predecessors.Keys)
        {
            List<PathComponent> shortestPath = new List<PathComponent>();
            PopulateListFromPredecessors(shortestPath, node, predecessors);
            paths[node] = shortestPath;
        }
        return paths;
    }

    private Node ShortestValue(Dictionary<Node, int> currentDistances, List<string> possibleNodeNames, Rng rng)
    {
        List<Node> possibleAns = new List<Node>();
        int shortestDistance = Distance.MAX_DISTANCE + 1;
        foreach (Node key in currentDistances.Keys) 
        { 
            if (possibleNodeNames.Contains(key.name) && currentDistances[key] < shortestDistance)
            {
                possibleAns.Clear();
                possibleAns.Add(key);
                shortestDistance = currentDistances[key];
            }
            else if (possibleNodeNames.Contains(key.name) && currentDistances[key] == shortestDistance)
            {
                possibleAns.Add(key);
            }
        }

        return possibleAns[rng.GetNumber() % possibleAns.Count];
    }

    private void PopulateListFromPredecessors(List<PathComponent> path, Node destination, Dictionary<Node, Node> predecessorMap)
    {
        if (predecessorMap[destination] != null)
        {
            PopulateListFromPredecessors(path, predecessorMap[destination], predecessorMap);
            path.Add(destination.NeighborEdge(predecessorMap[destination]));
            path.Add(destination);
        }
    }
}
