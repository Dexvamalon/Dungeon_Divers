using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    private float lenght = 1f;
    [Header("blockVariants")]
    [SerializeField] private bool[] blockVariant1 = new bool[8];
    [SerializeField] private bool[] blockVariant2 = new bool[8];
    [SerializeField] private bool[] blockVariant3 = new bool[8];
    [SerializeField] private bool[] blockVariant4 = new bool[8];
    [SerializeField] private int blockVariantAmount = 1;
    [SerializeField] private List<float> blockPositions = new List<float>();

    public void GetBlockVariants(List<bool[]> obsticleVariants)
    {
        List<bool[]> temporaryList = new List<bool[]> { blockVariant1, blockVariant2, blockVariant3, blockVariant4 };

        for(int i = 0; i < blockVariantAmount; i++)
        {
            obsticleVariants.Add(temporaryList[i]);
        }
    }
}
