using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Distance
{
    public static readonly int MAX_DISTANCE = 1000;
}

public enum NodeType
{
    SINGLE_ROAD,
    INTERSECT_ROAD,
    COFFEE,
    BREAKFAST,
    NEWS,
    BANK,
    WORK
}

public enum Obstruction
{
    NONE,
    ROAD_BLOCK,
    RUSH_HOUR,
    SHUTDOWN,
    FORGOT_ITEM,
    CROSSING_GUARD
}

public enum PersonStatus
{
    WALKING,
    IDLE,
    PAIRED
}