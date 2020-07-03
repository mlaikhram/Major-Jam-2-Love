using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEdge : MonoBehaviour
{
    public Edge[] edges;
    public NodeType nodeType;

    private bool isObstructed = false;

    private void OnMouseEnter()
    {
        if (!isObstructed)
        {
            switch (Player.instance.MouseState)
            {
                case Obstruction.ROAD_BLOCK:
                    if (nodeType == NodeType.SINGLE_ROAD_H || nodeType == NodeType.SINGLE_ROAD_V)
                    {
                        HashSet<Person> listeners = new HashSet<Person>();
                        foreach (Edge edge in edges)
                        {
                            listeners.UnionWith(edge.Listeners);
                        }
                        Debug.Log("warning people");
                        foreach (Person person in listeners)
                        {
                            Debug.Log(person.name);
                            person.CheckPreview(edges);
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
            foreach (Edge edge in edges)
            {
                foreach (Person person in edge.Listeners)
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
                case Obstruction.ROAD_BLOCK:
                    if (nodeType == NodeType.SINGLE_ROAD_H || nodeType == NodeType.SINGLE_ROAD_V)
                    {
                        HashSet<Person> listeners = new HashSet<Person>();
                        foreach (Edge edge in edges)
                        {
                            listeners.UnionWith(edge.Listeners);
                            edge.value = Distance.MAX_DISTANCE;
                        }
                        Debug.Log("rerouting people");
                        foreach (Person person in listeners)
                        {
                            Debug.Log(person.name);
                            person.AcknowledgePathChange();
                        }
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
