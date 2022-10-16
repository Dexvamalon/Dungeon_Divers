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

        FindObjectOfType<AudioManager>().Play("Run");
        FindObjectOfType<AudioManager>().Play("Ambiance");
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        ui.SetStats(curHealth, -1);
        //Debug.Log("Player took " + damage + " damage.");

        playerMovement.StartInvicibility();
        if (curHealth <= 0)
        {
            FindObjectOfType<AudioManager>().Play("Death");
            FindObjectOfType<AudioManager>().Stop("Run");
            StartCoroutine(FindObjectOfType<AudioManager>().MusicFade("LevelMusic", "DeathMusic"));
            StartCoroutine(Death());
            StartCoroutine(CameraDeathScroll());
            Debug.Log("Player died");
            
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
        FindObjectOfType<AudioManager>().DeathScreenVolume();
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
