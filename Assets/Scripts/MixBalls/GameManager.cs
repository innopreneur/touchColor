using UnityEngine;
using System.Collections;
using System.Diagnostics;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    /*** inspector contolled variables ***/
    public float levelTime = 30f;
    public Text mainTimer;
    public Text levelText;
    public Text startText;
   
    /*** internal variables ****/
    public static GameManager instance;
    private int timeIncrease;
    public bool increaseTime = false;
    private Stopwatch timer;
    private float timeElasped;
    public bool isStarted = false;
    private bool isTimeExpired = false;
    private bool isLevelComplete = false;
    



    public int levelCount = 1;
    private float incrementTimer = 3f;
    private float levelCompleteTimer = 3f;
    private AudioSource backgroundMusic;

    // Use this for initialization
    void Awake () {
	
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }

        timer = new Stopwatch();
        backgroundMusic = GetComponent<AudioSource>();

    }
	
    void Start()
    {

        //set UI text elements
        startText.text = "Tap to Start";
        levelText.text = "Level " + levelCount;
        mainTimer.text = "";

        backgroundMusic.Play();
    }

	// Update is called once per frame
	void Update () {

        ManageTimer();
        LevelCompletionCheck();

        if (isTimeExpired)
        {
            if (!isLevelComplete)
            {
                mainTimer.text = "Game Over";
                mainTimer.GetComponent<RectTransform>().anchoredPosition = Vector3.down * (Screen.height / 2);
            }
        }

        if (isLevelComplete)
        {
            if (levelCompleteTimer > 0)
            {
                startText.text = "Level Complete";
                levelCompleteTimer -= Time.deltaTime;
            }
            else
            {
                LoadNextLevel();
                levelCompleteTimer = 3f;
            }
            
        }

    }

    void ManageTimer()
    {
        if (Input.GetMouseButtonDown(0) && !isStarted)
        {
            timer.Start();
            isStarted = true;
            mainTimer.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            levelText.text = "";
            startText.text = "";

        }

        if (isStarted)
        {
            mainTimer.text = "" + (levelTime + timeIncrease - timer.Elapsed.Seconds);
            mainTimer.fontSize = 20;
        }

        if (increaseTime)
        {
            timeIncrease += 5;
            increaseTime = false;
        }

        if(levelTime + timeIncrease - timer.Elapsed.Seconds <= 0)
        {
            isTimeExpired = true;
        }
    }

    void LevelCompletionCheck()
    {
        //if all pickups are picked up by ball
        if(LevelController.instance.pickupsEnabledCount <= 0)
        {
            isLevelComplete = true;
        }
    }

    void LoadNextLevel()
    {
        levelCount++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }
}
