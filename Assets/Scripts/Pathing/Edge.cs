using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : PathComponent
{
    public int value;
    private List<Node> nodes = new List<Node>();
    public List<Node> Nodes => nodes;

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    private void OnMouseEnter()
    {
        switch (Player.instance.MouseState)
        {
            case Obstruction.ROAD_BLOCK:
                Debug.Log("warning people");
                foreach (Person person in listeners)
                {
                    Debug.Log(person.name);
                    person.CheckPreview(this);
                }
                break;
            default:
                break;
        }
    }

    private void OnMouseExit()
    {
        foreach (Person person in listeners)
        {
            person.ClearPreview();
        }
    }

    private void OnMouseOver()
    {
        // TODO some preview depending on the mouseState of Player
    }

    private void OnMouseDown()
    {
        switch (Player.instance.MouseState)
        {
            case Obstruction.ROAD_BLOCK:
                Debug.Log("rerouting people");
                foreach (Person person in listeners)
                {
                    Debug.Log(person.name);
                    person.AcknowledgePathChange();
                }
                break;
            default:
                break;
        }
        Player.instance.SelectObstruction(Obstruction.NONE);
    }
}
