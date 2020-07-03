using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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

    private Node blockedNode = null;
    private HashSet<Edge> blockedEdges = new HashSet<Edge>();
    private List<PathComponent> previewPath = new List<PathComponent>();

    public PathMarker marker;
    public float markerCooldown;
    private float markerTimer = 0;

    private PersonStatus status = PersonStatus.WALKING; // TODO: start in idle

    // Start is called before the first frame update
    private void Start()
    {
        line = GetComponent<LineRenderer>();

        // setup idle times
        idleMap[NodeType.SINGLE_ROAD_H] = 0;
        idleMap[NodeType.SINGLE_ROAD_V] = 0;
        idleMap[NodeType.SINGLE_ROAD_L] = 0;
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
            comp.AddListener(this);
        }
        // tp to initial node
        Debug.Log("moving to ");
        Debug.Log(requiredNodes[0].transform.position);
        transform.position = requiredNodes[0].transform.position;
        requiredNodes.RemoveAt(0);
        // remove first edge
        path.RemoveAt(0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ClearPreview();
        }
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
                    RemovePathNode(idleNode);
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
                        if (blockedNode == targetNode)
                        {
                            blockedNode = null;
                        }
                        idleNode = targetNode;
                        idleTimer = idleMap[targetNode.nodeType] + targetNode.idleTime;
                        transform.position = targetNode.idlePosition;
                        if (targetNode.hideIdle)
                        {
                            EnableSprites(false);
                        }
                    }
                    else
                    {
                        // otherwise delete node
                        RemovePathNode(targetNode);
                    }
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

        // update timeline
        //      include check for blockedNode when paired with MouseState for delays

        // update line render
        Vector3 startingPoint = idleNode == null ? transform.position : idleNode.transform.position;

        line.positionCount = 1;
        line.SetPosition(0, startingPoint);
        int i = 1;
        List<PathComponent> pathToDraw = previewPath.Any() ? previewPath : path;
        foreach (PathComponent comp in pathToDraw)
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

    public void AcknowledgePathChange()
    {
        // if edge, re-roll from previous node, checking paths to all subsequent important nodes
        //      if important node is not the finish, and important node path length > 100, 
        //      then find a similar nearby important node and insert it after this node, shifting idle time from this node to new 

        // if node, check the type of node and act accordingly
        // if node is locked, then find a similar nearby important node and insert it after this node, shifting idle time from this node to new node
        // if node wait time is increased, do nothing
        switch(Player.instance.MouseState)
        {
            case Obstruction.ROAD_BLOCK:
                foreach (PathComponent comp in path)
                {
                    comp.RemoveListener(this);
                }
                foreach (PathComponent comp in previewPath)
                {
                    comp.AddListener(this);
                }
                path = new List<PathComponent>(previewPath);
                previewPath.Clear();
                blockedEdges.Clear();
                break;

            default:
                break;
        }
    }

    public void CheckPreview(params PathComponent[] comps)
    {
        // check if road block is placeable by seeing if the preview route is above the Distance.LIMIT
        switch (Player.instance.MouseState)
        {
            case Obstruction.ROAD_BLOCK:
                // figure out which required nodes are affected by this road block
                List<Node> requiredPreviewNodes = new List<Node>(requiredNodes);
                int pathBlockedEdgeIndex = -1;
                for (int i = 0; i < path.Count; ++i)
                {
                    if (comps.Contains(path[i]))
                    {
                        pathBlockedEdgeIndex = i;
                        requiredPreviewNodes.Insert(0, path[i - 1] as Node);
                        break;
                    }
                    else if (requiredPreviewNodes[0] == path[i] || requiredPreviewNodes[0] == idleNode)
                    {
                        requiredPreviewNodes.RemoveAt(0);
                    }
                }

                blockedEdges.Clear();
                foreach (PathComponent comp in comps)
                {
                    blockedEdges.Add(comp as Edge);
                }

                // calculate preview path
                previewPath.Clear();
                for (int i = 1; i < requiredPreviewNodes.Count; ++i)
                {
                    // TODO: check if a path cost is >= MAX_DISTANCE and attempt to go to another building, or prevent user from placing obstruction
                    previewPath.AddRange(Map.instance.CalculateShortestPaths(requiredPreviewNodes[i - 1], seed, blockedEdges)[requiredPreviewNodes[i]]);
                }

                previewPath.InsertRange(0, path.GetRange(0, pathBlockedEdgeIndex));
                break;

            case Obstruction.RUSH_HOUR:
                if (idleNode != comps[0])
                {
                    blockedNode = comps[0] as Node;
                }
                break;

            case Obstruction.CROSSING_GUARD:
                if (idleNode != comps[0])
                {
                    blockedNode = comps[0] as Node;
                }
                break;

            default:
                break;
        }
    }

    public void ClearPreview()
    {
        previewPath.Clear();
        blockedEdges.Clear();
        blockedNode = null;
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
            ((node.nodeType == NodeType.SINGLE_ROAD_H || node.nodeType == NodeType.SINGLE_ROAD_V || node.nodeType == NodeType.SINGLE_ROAD_L || node.nodeType == NodeType.INTERSECT_ROAD) && node.idleTime > 0);
    }

    private void EnableSprites(bool enable)
    {
        hair.enabled = enable;
        skin.enabled = enable;
        clothes.enabled = enable;
    }

    private void RemovePathNode(Node node)
    {
        path.Remove(node);
        if (!path.Contains(node))
        {
            node.RemoveListener(this);
        }
        if (previewPath.Any())
        {
            previewPath.Remove(node);
        }
        if (requiredNodes[0] == node)
        {
            requiredNodes.Remove(node);
            if (!requiredNodes.Any())
            {
                Destroy(gameObject);
                return;
            }
        }
        // remove the next edge
        PathComponent nextEdge = path[0];
        path.Remove(nextEdge);
        if (!path.Contains(nextEdge))
        {
            nextEdge.RemoveListener(this);
        }
        if (previewPath.Any())
        {
            previewPath.Remove(nextEdge);
        }
        // check if preview is affected
        if (blockedEdges.Contains(nextEdge))
        {
            if (path.Contains(nextEdge))
            {
                Debug.Log("updating preview");
                CheckPreview(blockedEdges.ToArray());
            }
            else
            {
                ClearPreview();
            }
        }
    }
}
