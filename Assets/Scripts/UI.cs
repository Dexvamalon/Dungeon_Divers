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
    [SerializeField] private string deathMaxScoreStart;
    private Slider healthSlider;
    [SerializeField] private string healthSliderName;
    private TextMeshProUGUI score;
    [SerializeField] private string scoreName;
    [SerializeField] private bool isInGameScene = false;
    private bool isInvicible;
    public bool IsInvicible
    {
        get { return isInvicible; }   
        set { isInvicible = value;
            if (isInvicible)
            {
                SetSliderColor(invicibilityColor);
            }
            else
            {
                SetSliderColor(defaultColor);
            }
        }
    }
    [SerializeField] Color invicibilityColor = new Color();
    Color defaultColor;

    [SerializeField] private SOInt sOInt;

    private int _storedScore;

    void Start()
    {
        FindObjects();
        defaultColor = healthSlider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color;
    }

    void SetSliderColor(Color col)
    {
        healthSlider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = col;
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
            _storedScore = newScore;
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

        if(sOInt.Score < _storedScore)
        {
            sOInt.Score = _storedScore;
        }
        deathScreen.transform.Find(deathMaxScoreName).GetComponent<TextMeshProUGUI>().text = deathMaxScoreStart + sOInt.Score.ToString();
    }
}
