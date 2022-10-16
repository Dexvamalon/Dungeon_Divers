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
    [SerializeField] private float deathScreenDelay;
    [SerializeField] private float cameraDeathScrollAccelerator = 0.9f;

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
        //Debug.Log("Player took " + damage + " damage.");

        playerMovement.StartInvicibility();
        if (curHealth <= 0)
        {
            StartCoroutine(Death());
            StartCoroutine(CameraDeathScroll());
            Debug.Log("Player died");
            FindObjectOfType<AudioManager>().Play("Death");
            return;
        }
        FindObjectOfType<AudioManager>().Play("Hit");
    }

    IEnumerator Death()
    {
        //Pause Game
        levelManager.isDead = true;
        playerMovement.isDead = true;
        SpriteScroller[] array = FindObjectsOfType<SpriteScroller>();
        for(int i = 0; i < array.Length; i++)
        {
            array[i].var = false;
        }
        GetComponent<Animator>().SetBool("dead", true);

        //Set Death Screen
        yield return new WaitForSeconds(deathScreenDelay);
        deathScreen.SetActive(true);
        ui.SetDeathStats();
    }

    IEnumerator CameraDeathScroll()
    {
        while (Camera.main.transform.position.y > 0)
        {
            Transform tra = Camera.main.transform;
            tra.position = new Vector3(tra.position.x, tra.position.y * Mathf.Pow(cameraDeathScrollAccelerator, Time.deltaTime), tra.position.z);
            yield return null;
        }
    }
}
