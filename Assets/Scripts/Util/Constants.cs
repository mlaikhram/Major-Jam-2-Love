using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;

public class Distance
{
    public static readonly int MAX_DISTANCE = 1000000;
}

public class Delay
{
    public static readonly float RUSH_HOUR = 8;
    public static readonly float CROSSING_GUARD = 3;
}

public enum NodeType
{
    SINGLE_ROAD_H,
    SINGLE_ROAD_V,
    SINGLE_ROAD_L,
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

public class Colors
{
    public Color[] SKIN = { 
        new Color(0.5377358f, 0.3810211f, 0.04312033f, 1f),
        new Color(0.8584906f, 0.6720858f, 0.2713154f, 1f)
    };

    public Color[] HAIR =
    {

    };

    public Color[] SHIRT =
    {

    };

    public Color[] PANTS =
    {

    };

    public Color[] SHOES =
    {

    };
}