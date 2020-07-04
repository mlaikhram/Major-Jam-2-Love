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

    public List<SpriteRenderer> windows;

    private bool isOpen = true;
    public bool IsOpen => isOpen;

    private GameObject obstructIcon = null;

    private void Awake()
    {
        foreach (Edge edge in edges)
        {
            edge.AddNode(this);
        }
    }

    private void Update()
    {
        if (!isObstructed && Input.GetMouseButtonDown(1))
        {
            DestroyObstructIcon();
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
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS || nodeType == NodeType.BANK)
                    {
                        CreateHourglass();
                        HashSet<Person> tempListeners = new HashSet<Person>(listeners);
                        foreach (Person person in tempListeners)
                        {
                            Debug.Log(person.name);
                            person.CheckPreview(this);
                        }
                    }
                    break;

                case Obstruction.SHUTDOWN:
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS)
                    {
                        TurnOffWindows();
                        HashSet<Person> tempListeners = new HashSet<Person>(listeners);
                        foreach (Person person in tempListeners)
                        {
                            Debug.Log(person.name);
                            person.CheckPreview(this);
                        }
                    }
                    break;

                case Obstruction.CROSSING_GUARD:
                    if (nodeType == NodeType.INTERSECT_ROAD)
                    {
                        CreateCrossingGuard();
                        HashSet<Person> tempListeners = new HashSet<Person>(listeners);
                        foreach (Person person in tempListeners)
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
            DestroyObstructIcon();
            if (Player.instance.MouseState == Obstruction.RUSH_HOUR || Player.instance.MouseState == Obstruction.CROSSING_GUARD || Player.instance.MouseState == Obstruction.SHUTDOWN)
            {
                HashSet<Person> tempListeners = new HashSet<Person>(listeners);
                foreach (Person person in tempListeners)
                {
                    person.ClearPreview();
                }
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
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS || nodeType == NodeType.BANK)
                    {
                        idleTime += Delay.RUSH_HOUR;
                        isObstructed = true;
                        Player.instance.SelectObstruction(Obstruction.NONE);
                    }
                    break;

                case Obstruction.SHUTDOWN:
                    if (nodeType == NodeType.COFFEE || nodeType == NodeType.BREAKFAST || nodeType == NodeType.NEWS)
                    {
                        isOpen = false;
                        HashSet<Person> tempListeners = new HashSet<Person>(listeners);
                        foreach (Person person in tempListeners)
                        {
                            person.AcknowledgePathChange();
                        }
                        isObstructed = true;
                        Player.instance.SelectObstruction(Obstruction.NONE);
                    }
                    break;

                case Obstruction.CROSSING_GUARD:
                    if (nodeType == NodeType.INTERSECT_ROAD)
                    {
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

    private void CreateCrossingGuard()
    {
        obstructIcon = Instantiate(Player.instance.crossingGuard, new Vector3(transform.position.x + 0.2f, transform.position.y + 0.4f, 0f), Quaternion.identity);
    }

    private void CreateHourglass()
    {
        obstructIcon = Instantiate(Player.instance.rushHourglass, new Vector3(idlePosition.x, idlePosition.y - 1f, 0f), Quaternion.identity);
    }

    private void TurnOffWindows()
    {
        foreach (SpriteRenderer window in windows)
        {
            window.color = new Color(0.35f, 0.35f, 0.35f, 1);
        }
    }

    private void DestroyObstructIcon()
    {
        if (obstructIcon != null)
        {
            Destroy(obstructIcon);
            obstructIcon = null;
        }
        foreach (SpriteRenderer window in windows)
        {
            window.color = new Color(1, 1, 1, 1);
        }
    }
}
