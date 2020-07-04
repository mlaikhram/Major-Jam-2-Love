using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    public SpriteRenderer eye;
    private bool eyeState = true;

    public TimelineSymbol coffeeSymbol;
    public TimelineSymbol breakfastSymbol;
    public TimelineSymbol newsSymbol;
    public TimelineSymbol bankSymbol;
    public TimelineSymbol workSymbol;

    public SpriteRenderer hair;
    public SpriteRenderer head;
    public SpriteRenderer body;
    public SpriteRenderer shirt;
    public SpriteRenderer pants;
    public SpriteRenderer shoes;

    private float startOffset = 1f;
    private float timeGranularity = 0.3f;

    public void InitTimeline(Person person)
    {
        // TODO: player sprite, line match player color
        Debug.Log("initializing sprites");
        hair.sprite = person.hair.sprite;
        hair.color = person.hair.color;
        head.sprite = person.head.sprite;
        head.color = person.head.color;
        body.sprite = person.body.sprite;
        body.color = person.body.color;
        shirt.sprite = person.shirt.sprite;
        shirt.color = person.shirt.color;
        pants.sprite = person.pants.sprite;
        pants.color = person.pants.color;
        shoes.sprite = person.shoes.sprite;
        shoes.color = person.shoes.color;

        coffeeSymbol.Line.startColor = person.shirt.color;
        coffeeSymbol.Line.endColor = person.shirt.color;
        breakfastSymbol.Line.startColor = person.shirt.color;
        breakfastSymbol.Line.endColor = person.shirt.color;
        newsSymbol.Line.startColor = person.shirt.color;
        newsSymbol.Line.endColor = person.shirt.color;
        bankSymbol.Line.startColor = person.shirt.color;
        bankSymbol.Line.endColor = person.shirt.color;

        CleanTimeline();
    }

    private void CleanTimeline()
    {
        Vector3 hiddenPosition = new Vector3(100, 100, 100);
        coffeeSymbol.transform.position = hiddenPosition;
        breakfastSymbol.transform.position = hiddenPosition;
        newsSymbol.transform.position = hiddenPosition;
        bankSymbol.transform.position = hiddenPosition;
        workSymbol.transform.position = hiddenPosition;

        coffeeSymbol.Line.positionCount = 0;
        breakfastSymbol.Line.positionCount = 0;
        newsSymbol.Line.positionCount = 0;
        bankSymbol.Line.positionCount = 0;
        // workSymbol.Line.positionCount = 0; // does not have a line
    }

    public void UpdateTimeline(Vector3 startingPoint, List<PathComponent> path, List<Node> requiredNodes, float walkSpeed, Dictionary<Node, Node> altNodeMap, Node idleNode, float idleTime, Dictionary<NodeType, float> idleMap, Node blockedNode)
    {
        // clean up timeline
        CleanTimeline();

        List<Node> keyNodes = path.Where((comp) => comp is Node).Select((comp) => comp as Node).ToList();
        List<Node> realRequiredNodes = new List<Node>(requiredNodes);
        for (int i = 0; i < realRequiredNodes.Count; ++i)
        {
            if (altNodeMap.ContainsKey(realRequiredNodes[i]))
            {
                realRequiredNodes[i] = altNodeMap[realRequiredNodes[i]];
            }
        }
        Vector3 previousPoint = startingPoint;
        float currentTime = 0;
        if (idleNode != null)
        {
            TimelineSymbol timelineSymbol = GetTimelineSymbol(idleNode.nodeType);
            if (timelineSymbol != null)
            {
                timelineSymbol.transform.position = new Vector3(transform.position.x + startOffset + currentTime * timeGranularity, transform.position.y, 0);
                timelineSymbol.Line.positionCount = 2;
                timelineSymbol.Line.SetPosition(0, timelineSymbol.transform.position);
                timelineSymbol.Line.SetPosition(1, new Vector3(timelineSymbol.transform.position.x + idleTime * timeGranularity, transform.position.y, 0));
                timelineSymbol.ColorSprite.color = idleNode.buildingColor;
            }
            currentTime += idleTime;

            keyNodes.RemoveAt(0);
            realRequiredNodes.Remove(idleNode);
        }
        foreach (Node node in keyNodes)
        {
            float travelDistance = (node.transform.position - previousPoint).magnitude;
            currentTime += travelDistance / walkSpeed;

            // check if you must idle here
            float previewIdleTime = 0;
            if (node == blockedNode)
            {
                switch (Player.instance.MouseState)
                {
                    case Obstruction.RUSH_HOUR:
                        previewIdleTime = Delay.RUSH_HOUR;
                        break;

                    case Obstruction.CROSSING_GUARD:
                        previewIdleTime = Delay.CROSSING_GUARD;
                        break;

                    default:
                        break;
                }
            }
            TimelineSymbol timelineSymbol = GetTimelineSymbol(node.nodeType);
            if (timelineSymbol != null && realRequiredNodes[0] == node)
            {
                timelineSymbol.transform.position = new Vector3(transform.position.x + startOffset + currentTime * timeGranularity, transform.position.y, 0);

                if (timelineSymbol.Line != null)
                {
                    float currentIdleTime = node.idleTime + idleMap[node.nodeType] + previewIdleTime;
                    timelineSymbol.Line.positionCount = 2;
                    timelineSymbol.Line.SetPosition(0, timelineSymbol.transform.position);
                    timelineSymbol.Line.SetPosition(1, new Vector3(timelineSymbol.transform.position.x + currentIdleTime * timeGranularity, transform.position.y, 0));
                    currentTime += currentIdleTime;
                }
                timelineSymbol.ColorSprite.color = node.buildingColor;
                realRequiredNodes.RemoveAt(0);
            }
            else if (timelineSymbol == null)
            {
                currentTime += node.idleTime + previewIdleTime;
            }
            previousPoint = node.transform.position;
        }

    }

    private void OnMouseEnter()
    {
        int newColor = eyeState ? 0 : 1;
        eye.color = new Color(newColor, newColor, newColor, 1);
    }

    private void OnMouseExit()
    {
        int newColor = eyeState ? 1 : 0;
        eye.color = new Color(newColor, newColor, newColor, 1);
    }

    private void OnMouseDown()
    {
        eyeState = !eyeState;
    }

    private TimelineSymbol GetTimelineSymbol(NodeType nodeType)
    {
        switch (nodeType) 
        {
            case NodeType.COFFEE:
                return coffeeSymbol;
            case NodeType.BREAKFAST:
                return breakfastSymbol;
            case NodeType.NEWS:
                return newsSymbol;
            case NodeType.BANK:
                return bankSymbol;
            case NodeType.WORK:
                return workSymbol;
            default:
                return null;
        }
    }
}
