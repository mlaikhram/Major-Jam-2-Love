using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEdge : MonoBehaviour
{
    public Edge[] edges;
    public NodeType nodeType;

    private bool isObstructed = false;

    private GameObject roadblock1 = null;
    private GameObject roadblock2 = null;
    private BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!isObstructed && Input.GetMouseButtonDown(1))
        {
            DestroyRoadblocks();
        }
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
                        CreateRoadblocks();
                        HashSet<Person> listeners = new HashSet<Person>();
                        foreach (Edge edge in edges)
                        {
                            listeners.UnionWith(edge.Listeners);
                        }
                        foreach (Person person in listeners)
                        {
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
            DestroyRoadblocks();
            if (Player.instance.MouseState == Obstruction.ROAD_BLOCK)
            {
                foreach (Edge edge in edges)
                {
                    HashSet<Person> tempListeners = new HashSet<Person>(edge.Listeners);
                    foreach (Person person in tempListeners)
                    {
                        person.ClearPreview();
                    }
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
                        foreach (Person person in listeners)
                        {
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

    private void CreateRoadblocks()
    {
        if (nodeType == NodeType.SINGLE_ROAD_H)
        {
            roadblock1 = Instantiate(Player.instance.roadblockV, new Vector3(transform.position.x - 0.5f * boxCollider.size.x + 0.3f, transform.position.y + 0.1f, 0), Quaternion.identity);
            roadblock2 = Instantiate(Player.instance.roadblockV, new Vector3(transform.position.x + 0.5f * boxCollider.size.x - 0.3f, transform.position.y + 0.1f, 0), Quaternion.identity);
        }
        else if (nodeType == NodeType.SINGLE_ROAD_V)
        {
            roadblock1 = Instantiate(Player.instance.roadblockH, new Vector3(transform.position.x, transform.position.y - 0.5f * boxCollider.size.y + 0.2f, 0), Quaternion.identity);
            roadblock2 = Instantiate(Player.instance.roadblockH, new Vector3(transform.position.x, transform.position.y + 0.5f * boxCollider.size.y + 0.2f, 0), Quaternion.identity);
        }
    }

    private void DestroyRoadblocks()
    {
        if (roadblock1 != null)
        {
            Destroy(roadblock1);
            roadblock1 = null;
        }
        if (roadblock2 != null)
        {
            Destroy(roadblock2);
            roadblock2 = null;
        }
    }
}
