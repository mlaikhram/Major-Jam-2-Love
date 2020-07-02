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

    private void OnMouseOver()
    {
        // TODO some preview depending on the mouseState of Player
    }

    private void OnMouseDown()
    {
        Debug.Log("clicked on me " + this.name);
        // TODO do something depending on the mouseState of Player
    }
}
