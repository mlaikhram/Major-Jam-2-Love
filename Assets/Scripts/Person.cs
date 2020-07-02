using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Person : MonoBehaviour
{
    public SpriteRenderer hair;
    public SpriteRenderer skin;
    public SpriteRenderer clothes;
    public int seed; // could possibly add "salt" that changes after every committed obstruction (only obstructions that affect me); salt determines an initial amount of times to cycle the seed
    public float walkSpeed;
    public List<Node> requiredNodes;

    [Serializable]
    public class IdleTimes
    {
        public float coffee;
        public float breakfast;
        public float news;
        public float bank;
    }

    public IdleTimes idleTimes;
    private Dictionary<NodeType, float> idleMap = new Dictionary<NodeType, float>();
    private Node idleNode = null;
    private float idleTimer = 0;

    private List<PathComponent> path = new List<PathComponent>();
    public List<PathComponent> Path => path;
    private LineRenderer line;
    public LineRenderer Line => line;

    public PathMarker marker;
    public float markerCooldown;
    private float markerTimer = 0;

    private PersonStatus status = PersonStatus.WALKING; // TODO: start in idle

    // Start is called before the first frame update
    private void Start()
    {
        line = GetComponent<LineRenderer>();

        // setup idle times
        idleMap[NodeType.SINGLE_ROAD] = 0;
        idleMap[NodeType.INTERSECT_ROAD] = 0;
        idleMap[NodeType.COFFEE] = idleTimes.coffee;
        idleMap[NodeType.BREAKFAST] = idleTimes.breakfast;
        idleMap[NodeType.NEWS] = idleTimes.news;
        idleMap[NodeType.BANK] = idleTimes.bank;
        idleMap[NodeType.WORK] = 0;

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
        switch (status)
        {
            case PersonStatus.PAIRED:
                return;

            case PersonStatus.IDLE:
                Debug.Log(this.name + " is idling " + idleTimer);
                if (idleTimer <= 0)
                {
                    status = PersonStatus.WALKING;
                    transform.position = idleNode.transform.position;
                    EnableSprites(true);
                    path.Remove(idleNode);
                    // remove the next edge
                    path.RemoveAt(0);
                    idleNode = null;
                }
                else
                {
                    idleTimer -= Time.deltaTime;
                }
                break;

            case PersonStatus.WALKING:
                // walk to next node in list
                //      if at a node with an idle time, wait for idle time before continuing
                //      if at destination node, delete self
                Node targetNode = null;
                foreach (PathComponent comp in path)
                {
                    if (comp is Node)
                    {
                        targetNode = comp as Node;
                        break;
                    }
                }
                Vector3 heading = targetNode.transform.position - transform.position;
                float distance = heading.magnitude;

                // check if you've reached the center of the current node
                if (distance <= 0.03f)
                {
                    // do idle if necessary
                    if (MustIdle(targetNode))
                    {
                        status = PersonStatus.IDLE;
                        idleNode = targetNode;
                        idleTimer = idleMap[targetNode.nodeType] + targetNode.idleTime;
                        transform.position = targetNode.idlePosition;
                        if (targetNode.hideIdle)
                        {
                            EnableSprites(false);
                        }
                    }

                    // otherwise delete node
                    path.Remove(targetNode);
                    if (requiredNodes[0] == (targetNode as Node))
                    {
                        requiredNodes.Remove(targetNode as Node);
                        if (!requiredNodes.Any())
                        {
                            Destroy(gameObject);
                            return;
                        }
                    }
                    // remove the next edge
                    path.RemoveAt(0);
                }
                // otherwise keep walking
                else
                {
                    if (heading.x < 0)
                    {
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                    else if (heading.x > 0)
                    {
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                    Vector3 direction = heading / distance;
                    transform.position += walkSpeed * Time.deltaTime * direction;
                }
                break;

            default:
                break;
        }

        // update line render
        Vector3 startingPoint = idleNode == null ? transform.position : idleNode.transform.position;
        line.positionCount = 1;
        line.SetPosition(0, startingPoint);
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
            PathMarker tempMarker = Instantiate<PathMarker>(marker, startingPoint, Quaternion.identity);
            tempMarker.Owner = this;
            markerTimer += markerCooldown;
        }
        markerTimer -= Time.deltaTime;

        // order sprites correctly in layer
        int order = (int)(transform.position.y * -1000f);
        hair.sortingOrder = order + 2;
        skin.sortingOrder = order;
        clothes.sortingOrder = order + 1;
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

    public void PairUp()
    {
        status = PersonStatus.PAIRED;
        line.positionCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Person partner = collision.GetComponent<Person>();
        if (partner != null)
        {
            Player.instance.CheckForMatch(this, partner);
        }
    }

    private bool MustIdle(Node node)
    {
        return
            (requiredNodes[0] == node && node.IsOpen) ||
            ((node.nodeType == NodeType.SINGLE_ROAD || node.nodeType == NodeType.INTERSECT_ROAD) && node.idleTime > 0);
    }

    private void EnableSprites(bool enable)
    {
        hair.enabled = enable;
        skin.enabled = enable;
        clothes.enabled = enable;
    }
}
