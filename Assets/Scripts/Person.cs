using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person : MonoBehaviour
{
    public int seed; // could possibly add "salt" that changes after every committed obstruction (only obstructions that affect me); salt determines an initial amount of times to cycle the seed
    public float walkSpeed;
    public List<Node> requiredNodes;


    private List<PathComponent> path = new List<PathComponent>();
    public List<PathComponent> Path => path;
    private LineRenderer line;
    public LineRenderer Line => line;

    public PathMarker marker;
    public float markerCooldown;
    private float markerTimer = 0;

    // Start is called before the first frame update
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        // initialize aesthetics
        // Calculate initial best path through all required nodes
        for (int i = 1; i < requiredNodes.Count; ++i)
        {
            path.AddRange(Map.instance.CalculateShortestPaths(requiredNodes[i - 1], seed)[requiredNodes[i]]);
        }
        Debug.Log("calculating optimal route");
        foreach (PathComponent comp in path)
        {
            Debug.Log(comp.name);
            comp.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
        }
        // tp to initial node
        Debug.Log("moving to ");
        Debug.Log(requiredNodes[0].transform.position);
        transform.position = requiredNodes[0].transform.position;
        requiredNodes.RemoveAt(0);
    }

    private void FixedUpdate()
    {
        // walk to next node in list
        //      if at a node with an idle time, wait for idle time before continuing
        //      if at destination node, delete self
        PathComponent targetNode = null;
        foreach (PathComponent comp in path)
        {
            if (comp is Node)
            {
                targetNode = comp;
                break;
            }
        }
        Vector3 heading = targetNode.transform.position - transform.position;
        float distance = heading.magnitude;

        // check if you've reached the center of the current node
        if (distance <= 0.03f)
        {
            // do idle if necessary


            // otherwise delete node
            path.Remove(targetNode);
            if (requiredNodes.Contains(targetNode as Node))
            {
                requiredNodes.Remove(targetNode as Node);
                if (!requiredNodes.Any())
                {
                    Destroy(gameObject);
                }
            }
            // remove the next edge
            path.RemoveAt(0);
        }
        // otherwise keep walking
        else
        {
            Vector3 direction = heading / distance;
            transform.position += walkSpeed * Time.deltaTime * direction;
        }

        // update line render
        line.positionCount = 1;
        line.SetPosition(0, transform.position);
        int i = 1;
        foreach (PathComponent comp in path)
        {
            if (comp is Node)
            {

                ++line.positionCount;
                line.SetPosition(i++, comp.transform.position);
            }
        }

        // spawn marker if necessary
        if (markerTimer <= 0f)
        {
            Debug.Log("spawning marker");
            PathMarker tempMarker = Instantiate<PathMarker>(marker, transform.position, Quaternion.identity);
            tempMarker.Owner = this;
            markerTimer += markerCooldown;
        }
        markerTimer -= Time.deltaTime;
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

    public void CheckPreview(PathComponent comp)
    {
        // check if road block is placeable by seeing if the preview route is above the Distance.LIMIT
    }
}
