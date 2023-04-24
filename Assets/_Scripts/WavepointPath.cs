using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavepointPath : MonoBehaviour
{
    public Transform GetWavepoint(int WavepointIndex)
    {
        return transform.GetChild(WavepointIndex);
    }

    public int GetNextWavepointIndex(int currentWavepointIndex)
    {
        int nextWavepointIndex = currentWavepointIndex + 1;

        if (nextWavepointIndex == transform.childCount)
        {
            nextWavepointIndex = 0;
        }

        return nextWavepointIndex;
    }
}
