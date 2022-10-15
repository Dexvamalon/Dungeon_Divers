using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroller : MonoBehaviour
{
    [SerializeField] Vector2 moveSpeed;

    Vector2 offset;
    Material material;

    LevelManager levelManager;
    public bool var = true;

    void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        levelManager = FindObjectOfType<LevelManager>();
    }

    void Update()
    {
        if(var)
        {
            offset = moveSpeed * levelManager.levelSpeed * Time.deltaTime / 1573 * 115;
            material.mainTextureOffset += offset;
        }
    }
}
