using System;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public static Player instance;

    [Serializable]
    public struct LoverPair
    {
        public Person p1;
        public Person p2;
    }
    public LoverPair[] loverPairs;
    private Dictionary<Person, Person> loverMap = new Dictionary<Person, Person>();

    private Obstruction mouseState = Obstruction.NONE;

    // Start is called before the first frame update
    void Start()
    {
        Player.instance = this;
        foreach (LoverPair loverPair in loverPairs)
        {
            loverMap[loverPair.p1] = loverPair.p2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("right click");
            mouseState = Obstruction.NONE;
        }
    }

    public void CheckForMatch(Person caller, Person partner)
    {
        if (loverMap.ContainsKey(caller) && loverMap[caller] == partner)
        {
            // match!
            caller.PairUp();
            partner.PairUp();
        }
    }

    public void SelectObstruction(Obstruction obstruction)
    {
        mouseState = obstruction;
    }
}
