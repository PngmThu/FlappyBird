using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : MonoBehaviour
{
    Text score;

    // Start is called before the first frame update
    void OnEnable()
    {
        score = GetComponent<Text>();
        score.text = "Score: " + GameManager.Instance.getScore + "\nGame Over";
    }
}
