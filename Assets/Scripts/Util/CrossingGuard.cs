using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingGuard : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        int order = (int)(transform.position.y * -1000f);
        GetComponent<SpriteRenderer>().sortingOrder = order;
    }
}
