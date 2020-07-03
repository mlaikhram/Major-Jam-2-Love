using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : PathComponent
{
    public List<Edge> edges;
    public NodeType nodeType;
    public float idleTime;
    public bool hideIdle;
    public Vector3 idlePosition;

    private bool isOpen = true;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        foreach (Edge edge in edges)
        {
            edge.AddNode(this);
        }
    }

    public HashSet<Node> Neighbors
    {
        get
        {
            HashSet<Node> nodes = new HashSet<Node>();
            foreach (Edge edge in edges)
            {
                foreach (Node node in edge.Nodes)
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
        int distance = 5 * Distance.MAX_DISTANCE;
        foreach (Edge edge in edges)
        {
            if (edge.Nodes.Contains(neighbor) && edge.value < distance)
            {
                ans = edge;
                distance = edge.value;
            }
        }
        return ans;
    }

    private void OnMouseEnter()
    {
        if (!isObstructed)
        {
            switch (Player.instance.MouseState)
            {
                case Obstruction.RUSH_HOUR:
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS)
                    {
                        Debug.Log("warning people");
                        foreach (Person person in listeners)
                        {
                            Debug.Log(person.name);
                            person.CheckPreview(this);
                        }
                    }
                    break;

                case Obstruction.CROSSING_GUARD:
                    if (nodeType == NodeType.INTERSECT_ROAD)
                    {
                        Debug.Log("warning people");
                        foreach (Person person in listeners)
                        {
                            Debug.Log(person.name);
                            person.CheckPreview(this);
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void OnMouseExit()
    {
        if (!isObstructed)
        {
            foreach (Person person in listeners)
            {
                person.ClearPreview();
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isObstructed)
        {
            switch (Player.instance.MouseState)
            {
                case Obstruction.RUSH_HOUR:
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS)
                    {
                        Debug.Log("rerouting people");
                        idleTime += Delay.RUSH_HOUR;
                        isObstructed = true;
                        Player.instance.SelectObstruction(Obstruction.NONE);
                    }
                    break;

                case Obstruction.CROSSING_GUARD:
                    if (nodeType == NodeType.INTERSECT_ROAD)
                    {
                        Debug.Log("rerouting people");
                        idleTime += Delay.CROSSING_GUARD;
                        isObstructed = true;
                        Player.instance.SelectObstruction(Obstruction.NONE);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
