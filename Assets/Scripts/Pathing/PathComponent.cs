using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathComponent : MonoBehaviour
{
    protected HashSet<Person> listeners = new HashSet<Person>();

    public void AddListener(Person person)
    {
        listeners.Add(person);
    }

    public void RemoveListener(Person person)
    {
        listeners.Remove(person);
    }

    public void NotifyListeners()
    {
        foreach (Person listener in listeners)
        {
            listener.AcknowledgePathChange(this);
        }
    }
}
