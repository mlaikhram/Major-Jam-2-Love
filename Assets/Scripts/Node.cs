﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : PathComponent
{
    public List<Edge> edges;

    public HashSet<Node> Neighbors
    {
        get
        {
            HashSet<Node> nodes = new HashSet<Node>();
            foreach (Edge edge in edges)
            {
                foreach (Node node in edge.nodes)
                {
                    if (node != this)
                    {
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }
    }

    public Edge NeighborEdge(Node neighbor)
    {
        Edge ans = null;
        int distance = Distance.MAX_DISTANCE;
        foreach (Edge edge in edges)
        {
            if (edge.nodes.Contains(neighbor) && edge.value < distance)
            {
                ans = edge;
                distance = edge.value;
            }
        }
        return ans;
    }
}