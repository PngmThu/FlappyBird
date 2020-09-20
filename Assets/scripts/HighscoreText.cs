using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour
{
    Text highscore;

    //Use OnEnable as text needs to be refreshed 
    //everytime StartPage is activated
    void OnEnable() {
        highscore = GetComponent<Text>();
        highscore.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
    }
}
