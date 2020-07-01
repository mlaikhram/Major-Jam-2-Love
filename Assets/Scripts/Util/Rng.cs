using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rng
{
    private readonly int m = 994393;
    private readonly int a = 2;

    private int seed;

    public Rng(int seed)
    {
        this.seed = seed;
    }

    public int GetNumber()
    {
        seed = a * seed % m;
        return seed;
    }
}
