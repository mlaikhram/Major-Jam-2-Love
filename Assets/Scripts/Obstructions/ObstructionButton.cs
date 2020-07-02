using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionButton : MonoBehaviour
{
    public Obstruction obstruction;
    private void OnMouseDown()
    {
        Debug.Log("clicked on me " + this.name);
        Player.instance.SelectObstruction(obstruction);
    }
}
