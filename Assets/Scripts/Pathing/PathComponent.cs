using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathComponent : MonoBehaviour
{
    protected HashSet<Person> listeners = new HashSet<Person>();
    public HashSet<Person> Listeners => listeners;

    protected bool isObstructed = false;

    public void AddListener(Person person)
    {
        listeners.Add(person);
    }

    public void RemoveListener(Person person)
    {
        listeners.Remove(person);
    }
}
