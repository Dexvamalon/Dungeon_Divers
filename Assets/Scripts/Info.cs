using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    [Header("blockVariants")]
    [SerializeField] private bool[] obstacleblock = new bool[8];
    [SerializeField] private float blockPosition = 0f;
    [SerializeField] public float damage = 1f;
    [SerializeField] private float length = 1f;

    public float GetLength()
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
