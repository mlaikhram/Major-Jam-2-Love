using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourglass : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(Vector3.back, 200f * Time.deltaTime);
        //Debug.Log("rotation: " + transform.rotation.z);
        if (transform.rotation.z > 0)
        {
            //transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            //transform.localScale = new Vector3(1, -1, 1);
        }
    }
}
