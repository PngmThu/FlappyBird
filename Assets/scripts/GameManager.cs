using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();

    //event sent to TapController.cs
    public static event GameDelegate OnGameStarted;

    //event sent to TapController.cs
    public static event GameDelegate OnGameOverConfirmed;

    //Use GameMangager.Instance to access public members of the class
    public static GameManager Instance;
    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState {
        None,    //None of others
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = true; //Initially not start game

    public bool GameOver {
        get { return gameOver; }
    }

    public int getScore {
        get { return score; }
    }

    void Awake() {
        Instance = this;
    }

    void OnEnable() {
        // += to subscribe event in other C# script
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    void OnDisable() {
        // -= to unsubscribe event in other C# script
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    //event from CountdownText.cs
    void OnCountdownFinished() {
        SetPageState(PageState.None);
        OnGameStarted(); //event sent to TapController.cs
        score = 0;
        gameOver = false;
    }

    //event from TapController.cs
    void OnPlayerDied() {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    //event from TapController.cs
    void OnPlayerScored() {
        score++;
        scoreText.text = score.ToString();
    }

    void SetPageState(PageState state) {
        switch (state) {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }
    }

    //activated when replay button is hit
    public void ConfirmGameOver() {
        OnGameOverConfirmed(); //event sent to TapController.cs
        scoreText.text = "0";
        SetPageState(PageState.Start);
    }

    //activated when play button is hit
    public void StartGame() {
        SetPageState(PageState.Countdown);
    }
}
