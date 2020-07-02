using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionButton : MonoBehaviour
{
    public Obstruction obstruction;
    void OnMouseDown()
    {
        // Destroy the gameObject after clicking on it
        Debug.Log("clicked on me " + this.name);
        Player.instance.SelectObstruction(obstruction);
    }
}
