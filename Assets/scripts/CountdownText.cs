using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Have to drag CountdownText to a Text object
[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CountdownFinished();

    //event sent to GameManager.cs
    public static event CountdownFinished OnCountdownFinished;

    Text countdown;

    //Whenever CountdownPage is enabled, OnEnable is called
    void OnEnable() {
        countdown = GetComponent<Text>();
        countdown.text = "3";
        StartCoroutine("Countdown"); 
    }

    //A coroutine
    IEnumerator Countdown() {
        for (int i = 3 ; i > 0 ; i--) {
            countdown.text = i.ToString();

            //Wait 1s then go to next frame
            yield return new WaitForSeconds(1);
        }

        OnCountdownFinished(); //event sent to GameManager
    }    
    
}
