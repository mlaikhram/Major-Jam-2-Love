using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public List<Node> requiredNodes;
    private List<PathComponent> path = new List<PathComponent>();

    // Start is called before the first frame update
    private void Start()
    {
        // initialize aesthetics
        // Calculate initial best path through all required nodes
        for (int i = 1; i < requiredNodes.Count; ++i)
        {
            path.AddRange(Map.instance.CalculateShortestPaths(requiredNodes[i - 1])[requiredNodes[i]]);
        }
        // tp to initial node
    }

    private void FixedUpdate()
    {
        // walk to next node in list
        //      if at a node with an idle time, wait for idle time before continuing
        //      if at destination node, delete self
    }

    public void AcknowledgePathChange(PathComponent comp)
    {
        // if edge, re-roll from previous node, checking paths to all subsequent important nodes
        //      if important node is not the finish, and important node path length > 100, 
        //      then find a similar nearby important node and insert it after this node, shifting idle time from this node to new node

        // if node, check the type of node and act accordingly
        // if node is locked, then find a similar nearby important node and insert it after this node, shifting idle time from this node to new node
        // if node wait time is increased, do nothing
    }
}
