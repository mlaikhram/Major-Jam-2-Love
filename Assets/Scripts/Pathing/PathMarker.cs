using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMarker : MonoBehaviour
{
    private Person owner;
    public Person Owner
    {
        set
        {
            owner = value;
            GetComponent<SpriteRenderer>().color = owner.Line.startColor;
            foreach (PathComponent comp in owner.Path)
            {
                if (comp is Node)
                {
                    targetNode = comp;
                    visitCount[targetNode] = 1;
                    break;
                }
            }
        }
        get
        {
            return owner;
        }
    }

    private PathComponent targetNode;
    private Dictionary<PathComponent, int> visitCount = new Dictionary<PathComponent, int>();

    void FixedUpdate()
    {
        Vector3 heading = targetNode.transform.position - transform.position;
        float distance = heading.magnitude;
        // check if you've reached the center of the current node
        if (distance <= 0.05f)
        {
            // do idle if necessary


            // otherwise find next node
            int startIndex = 0;
            for (int i = 0; i < visitCount[targetNode]; ++i)
            {
                startIndex = owner.Path.IndexOf(targetNode, startIndex) + 1;
            }
            targetNode = null;
            for (int i = startIndex; i < owner.Path.Count; ++i)
            {
                if (owner.Path[i] is Node)
                {
                    targetNode = owner.Path[i];
                    if (!visitCount.ContainsKey(targetNode))
                    {
                        visitCount[targetNode] = 0;
                    }
                    ++visitCount[targetNode];
                    break;
                }
            }
            if (targetNode == null)
            {
                Destroy(gameObject);
            }
        }
        // otherwise keep walking
        else
        {
            Vector3 direction = heading / distance;
            transform.position += 10f * owner.walkSpeed * Time.deltaTime * direction;
        }
    }
}
