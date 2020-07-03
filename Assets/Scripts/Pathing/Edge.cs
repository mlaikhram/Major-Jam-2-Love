﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : PathComponent
{
    public NodeType nodeType;
    public int value;
    private List<Node> nodes = new List<Node>();
    public List<Node> Nodes => nodes;

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    private void OnMouseEnter()
    {
        if (!isObstructed)
        {
            switch (Player.instance.MouseState)
            {
                case Obstruction.ROAD_BLOCK:
                    if (nodeType == NodeType.SINGLE_ROAD_H || nodeType == NodeType.SINGLE_ROAD_V)
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
                case Obstruction.ROAD_BLOCK:
                    if (nodeType == NodeType.SINGLE_ROAD_H || nodeType == NodeType.SINGLE_ROAD_V)
                    {
                        Debug.Log("rerouting people");
                        value = Distance.MAX_DISTANCE;
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
