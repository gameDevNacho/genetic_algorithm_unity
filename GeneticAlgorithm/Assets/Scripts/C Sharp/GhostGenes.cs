using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGenes
{
    public float[] TimesForImpulses { get; private set; }

    private float minTime;
    private float maxTime;

    public GhostGenes(int size, float minTime, float maxTime)
    {
        TimesForImpulses = new float[size];
        this.minTime = minTime;
        this.maxTime = maxTime;

        RandomizeGenes();
    }

    private void RandomizeGenes()
    {
        for (int i = 0; i < TimesForImpulses.Length; i++)
        {
            TimesForImpulses[i] = UnityEngine.Random.Range(minTime, maxTime);
        }
    }
}
