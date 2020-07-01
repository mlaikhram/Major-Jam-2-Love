using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Distance
{
    public static readonly int MAX_DISTANCE = 1000;
}

public enum NodeTypes
{
    BASIC,
    COFFEE,
    BREAKFAST,
    NEWS,
    WORK
}