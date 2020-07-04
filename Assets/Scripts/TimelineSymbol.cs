using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineSymbol : MonoBehaviour
{
    private SpriteRenderer colorSprite;
    public SpriteRenderer ColorSprite => colorSprite;
    private LineRenderer line;
    public LineRenderer Line => line;

    // Start is called before the first frame update
    void Start()
    {
        colorSprite = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }


}
