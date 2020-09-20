using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Have to drag TapController to a rigidbody2D object
[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce; //Tap Force
    public float tiltSmooth; //Độ smooth của sự nghiêng
    public Vector3 startPos;

    Rigidbody2D rigidbody;    //default: private
    Quaternion downRotation;  //Quaternion is rotation with 4 values
    Quaternion forwardRotation;

    GameManager game;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    //Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90); //Convert a Vector3 to Quaternion
        forwardRotation = Quaternion.Euler(0, 0, 35); 
        game = GameManager.Instance;
        rigidbody.simulated = false; //Not moving until click play
    }

    void OnEnable() {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    //event from GameManager.cs: After done countdown
    void OnGameStarted() { 
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true; //listen to physics
    }

    //event from GameManager.cs: After click ReplayButton
    void OnGameOverConfirmed() { 
        //Back to initial position
        transform.localPosition = startPos; 
        transform.rotation = Quaternion.identity;
    }

    //Update is called once per frame
    void Update()
    {   
        //If game over, don't do any update
        if (game.GameOver)
            return;

        //If tap on mobile devices
        if (Input.GetMouseButtonDown(0)) //0: left click, 1: right click
        {   
            //Rotate upwards
            transform.rotation = forwardRotation;
            //When we click, set velocity to 0
            rigidbody.velocity = Vector3.zero;
            //Add an upward force to the body
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);    
            //Play a sound
            tapAudio.Play();

            //Time.timeScale += 1;  //comment it after done testing 
        }

        //Transfrom from current to downRotation in an amount of time
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "ScoreZone") {
            //register a score event
            OnPlayerScored(); //event sent to GameManager
            //play a sound
            scoreAudio.Play();
        }

        if (col.gameObject.tag == "DeadZone") {
            //freeze the bird: not affected by physics (not let it drop)
            rigidbody.simulated = false; 
            //register a dead event
            OnPlayerDied(); //event sent to GameManager
            //play a sound
            dieAudio.Play();
        }
    }
}
