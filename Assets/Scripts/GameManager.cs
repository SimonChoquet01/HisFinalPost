using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using Random = UnityEngine.Random;

//The game manager handles: pausing, spawning monsters, keeping track of score, start and end of game.
public class GameManager : MonoBehaviour
{
    //--- Singleton Pattern ---

    private static GameManager _managerInstance = null;
    public static GameManager Instance { get => _managerInstance; } //GET ONLY

    private void Awake()
    {
        if (_managerInstance == null)
        {
            _managerInstance = this; //We assign our first instance
        }
        else
        {
            Destroy(gameObject); //Get rid of the rest. (if there is any others)
        }
    }

    //--- Editor Parameters ---
    [SerializeField] [Tooltip("Game object containing the possible monster spawn points.")] private GameObject _spawnPoints;
    [SerializeField] [Tooltip("Reference to the monster that will be spawned.")] private GameObject _monsterPrefab;
    [Tooltip("The player's game object.")] public GameObject Player;
    [SerializeField] [Tooltip("The audio mixer used for the volume settings.")] private AudioMixer _audioM;

    [SerializeField] [Tooltip("Text representing how much time has passed.")] private Text _timeText;
    [SerializeField] [Tooltip("The game over screen.")] private GameObject _failMenu;
    [SerializeField] [Tooltip("The pause screen.")] private GameObject _pauseMenu;
    [SerializeField] [Tooltip("First UI item selected when pause screen appears.")] private Selectable _pauseFirst; //First UI item selected when we pause.
    [SerializeField] [Tooltip("First UI item selected when fail screen appears.")] private Selectable _failFirst; //First UI item selected on gameover.
    [SerializeField] [Tooltip("The stats text.")] private Text _winText;

    //--- Public Variables ---
    [HideInInspector] public bool Paused = false;
    [HideInInspector] public bool Started = false;
    [HideInInspector] public bool Ended = false;

    //--- Private Variables ---
    private float _currentTimeElasped = 0.0f;
    private float _highscore = 0.0f;

    //--- Public Methods ---

    //Called when the player leaves the safe zone for the first time.
    public void StartGame()
    {
        this.Started = true;
        AddRandomMonsters(3);
        InvokeRepeating("AddRandomMonster", 30, 30);
        _highscore = PlayerPrefs.GetFloat("Score", 0.0f);
    }

    //UI events
    public void ButtonRetry()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); //Load this scenes loading screen
    }
    public void ButtonMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(2); //Load this scenes loading screen
    }

    // Event for when the player presses pause
    public void TogglePause()
    {
        if (!Ended)
        {
            if (Paused)
            {
                Time.timeScale = 1.0f;
                Paused = false;
                PlayerBehaviour.LockMouse();
                _pauseMenu.SetActive(false);
            }
            else
            {
                Input.ResetInputAxes(); //No double press
                Time.timeScale = 0.0f;
                Paused = true;
                PlayerBehaviour.UnlockMouse();
                _pauseMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_pauseFirst.gameObject);
            }
        }
    }

    //Called when the player fails (Death or Out of Bounds)
    public void GameOver()
    {
        if (this.Ended) return;
        CancelInvoke();
        this.Ended = true;
        PlayerBehaviour.UnlockMouse();
        _pauseMenu.SetActive(false);
        _failMenu.SetActive(true);
        if (_currentTimeElasped > _highscore)
        {
            _highscore = _currentTimeElasped;
            PlayerPrefs.SetFloat("Score", _highscore);
        }
        _winText.text = "Time: " + calculateTimeIntoString(_currentTimeElasped) + "\nRecord: " + calculateTimeIntoString(_highscore);
        Input.ResetInputAxes();
        EventSystem.current.SetSelectedGameObject(_failFirst.gameObject);
    }

    public void AddRandomMonsters(int count = 1)
    {
        if (Ended) return;
        List<Transform> sPoints = new List<Transform>();
        foreach (Transform t in _spawnPoints.GetComponentsInChildren<Transform>())
        {
            if (t != _spawnPoints.transform) //Do not add seld
            {
                sPoints.Add(t);
            }
        }
        for ( int i = 0; i < (count>sPoints.Count ? sPoints.Count : count ); i++) //Make sure we can only an amount = to the amount of spawn points we found
        {
            int index = Random.Range(0, sPoints.Count);
            GameObject monster = Instantiate(_monsterPrefab, sPoints[index].position, sPoints[index].rotation);
            monster.GetComponent<Monster>().Target = Player;
            sPoints.RemoveAt(index);
        }
    }

    //Adds one random monster (created without parameters for the invoke)
    public void AddRandomMonster()
    {
        AddRandomMonsters(1);
    }

    //--- Private Method ---

    private string calculateTimeIntoString(float timeSeconds)
    {
        int total_seconds = (int)Math.Truncate(timeSeconds); //Drop the decimal
        int seconds = total_seconds % 60;
        int minutes = (total_seconds - seconds) / 60;
        return minutes + ":" + seconds.ToString("D2");
    }

    //--- Unity Methods ---
    void Start()
    {
        _audioM.SetFloat("Vol", PlayerPrefs.GetFloat("Vol", 0.0f));
    }
    void Update()
    {
        if (this.Started && !this.Ended)
        {
            _currentTimeElasped += Time.deltaTime;
        }
        _timeText.text = calculateTimeIntoString(_currentTimeElasped);

    }
}
