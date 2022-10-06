using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float curHealth;
    private PlayerMovement playerMovement;
    private LevelManager levelManager;
    private UI ui;
    [SerializeField] private GameObject deathScreen;

    private void Start()
    {
        curHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        levelManager = FindObjectOfType<LevelManager>();
        ui = FindObjectOfType<UI>();
        ui.SetStats(maxHealth, 0);
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        ui.SetStats(curHealth, -1);
        Debug.Log("Player took " + damage + " damage.");
        if(curHealth <= 0)
        {
            Death();
            Debug.Log("Player died");
        }
        playerMovement.StartInvicibility();
    }

    private void Death()
    {
        //Pause Game
        levelManager.isDead = true;
        playerMovement.isDead = true;

        //Set Death Screen
        deathScreen.SetActive(true);
        ui.SetDeathStats();
    }
}
