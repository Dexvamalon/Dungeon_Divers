using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float curHealth;

    private void Start()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        Debug.Log("Player took " + damage + " damage.");
        if(curHealth <= 0)
        {
            Death();
            Debug.Log("Player died");
        }
    }

    private void Death()
    {

    }
}
