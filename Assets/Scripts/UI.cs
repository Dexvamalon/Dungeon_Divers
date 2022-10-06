using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    private Canvas canvas;
    private GameObject deathScreen;
    [SerializeField] private string deathScreenName;
    [SerializeField] private string deathScoreName;
    [SerializeField] private string deathScoreStart;
    [SerializeField] private string deathMaxScoreName;
    private Slider healthSlider;
    [SerializeField] private string healthSliderName;
    private TextMeshProUGUI score;
    [SerializeField] private string scoreName;
    [SerializeField] private bool isInGameScene = false;

    void Start()
    {
        FindObjects();
    }

    private void FindObjects()
    {
        if (isInGameScene)
        {
            canvas = FindObjectOfType<Canvas>();
            deathScreen = canvas.transform.Find(deathScreenName).gameObject;
            healthSlider = canvas.transform.Find(healthSliderName).GetComponent<Slider>();
            score = canvas.transform.Find(scoreName).gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        
    }

    public void LoadScene(string x)
    {
        SceneManager.LoadScene(x);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetStats(float health, int newScore)
    {
        if(canvas == null || deathScreen == null || healthSlider == null || score == null)
        {
            FindObjects();
        }
        if(health != -1)
        {
            healthSlider.value = health;
        }
        if(newScore != -1)
        {
            score.text = newScore.ToString();
        }
    }

    public void SetSlicerMax(float max)
    {
        if (canvas == null || deathScreen == null || healthSlider == null || score == null)
        {
            FindObjects();
        }
        healthSlider.maxValue = max;
    }

    public void SetDeathStats()
    {
        if (canvas == null || deathScreen == null || healthSlider == null || score == null)
        {
            FindObjects();
        }

        deathScreen.transform.Find(deathScoreName).GetComponent<TextMeshProUGUI>().text = deathScoreStart + score.text;
    }
}
