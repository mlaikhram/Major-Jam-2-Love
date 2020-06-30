using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    public List<Node> graph;

    private void Start()
    {
        instance = this;
        Debug.Log("Calculating shortest paths from " + graph[4].name);
        Dictionary<Node, List<PathComponent>> shortestPaths = CalculateShortestPaths(graph[4]);
        foreach (Node node in shortestPaths.Keys)
        {
            Debug.Log("-------------DESTINATION: " + node.name);
            foreach (PathComponent comp in shortestPaths[node])
            {
                Debug.Log(comp.name);
                comp.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
            }
        }
    }

    public Dictionary<Node, List<PathComponent>> CalculateShortestPaths(Node start)
    {
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
            Node shortestNode = ShortestValue(distances, nodeQ);
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

    private Node ShortestValue(Dictionary<Node, int> currentDistances, List<string> possibleNodeNames)
    {
        Node ans = null;
        int shortestDistance = Distance.MAX_DISTANCE + 1;
        foreach (Node key in currentDistances.Keys) 
        { 
            if (possibleNodeNames.Contains(key.name) && currentDistances[key] < shortestDistance)
            {
                ans = key;
                shortestDistance = currentDistances[key];
            }
        }
        return ans;
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
