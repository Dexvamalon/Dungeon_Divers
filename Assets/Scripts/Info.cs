using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    private float length = 1f;
    [Header("blockVariants")]
    [SerializeField] private bool[] obstacleblock = new bool[8];
    [SerializeField] private float blockPosition = 0f;

    public float GetLenght()
    {
        return length;
    }

    public bool[] GetObstacleBlock()
    {
        return obstacleblock;
    }

    public float GetBlockPosition()
    {
        return blockPosition;
    }
}
