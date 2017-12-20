using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public CameraManager cameraManager;
    public GameObject player;
    public PlayerController playerController;

    float playerDistanceTravelled;
    float lastPlayerDistance;

    public float startScrollDistance;

    public float startScrollSpeed = 1;
    public float scrollSpeedIncreaseAmount = 0.01f;
    public float scrollSpeedIncreaseTime = 1f;


    public float currentScrollSpeed;

    float currentTime;

    public bool isGameOver;

    bool playerFalling;
    float fallTime;

    public float reductionAmount = 0.1f;

    public Text score;
    public GameObject highscoreDisplay;
    public Text highscorevalue;

    bool stopScroll;
    int intscore;

	bool pressStart;

    // Use this for initialization
    void Start () {

        
        GameStart();
	}


    void GameStart() {
        isGameOver = false;
        stopScroll = false;
        cameraManager.scrollUpSpeed = 0;

        currentScrollSpeed = startScrollSpeed;
        currentTime = 0;

        playerController = player.GetComponent<PlayerController>();

        playerFalling = false;

        playerDistanceTravelled = 2;

        highscoreDisplay.SetActive(false);
        score.gameObject.SetActive(false);
        intscore = 0;

		pressStart = false;
    }

	// Update is called once per frame
	void FixedUpdate () {

        if(!isGameOver) { 
            CalculateDistanceY();

            PrintScore();

            CheckIfFalling();

            if (playerDistanceTravelled > startScrollDistance && stopScroll == false) {
                score.gameObject.SetActive(true);
                ScrollUpdate();

            }

        }
	}

    void CheckIfFalling() {
        if (!playerFalling) {
            if (playerController.currentPlayerState == PlayerController.PlayerState.falling) {
                fallTime = 0;
                playerFalling = true;
            }
        }
        else {
            if (playerController.currentPlayerState == PlayerController.PlayerState.falling) {
                fallTime += Time.fixedDeltaTime;
            }
            else {
                playerFalling = false;
                ReduceScrollSpeed(fallTime);
            }
        }
    }

    void ReduceScrollSpeed(float fallTime) {
        currentScrollSpeed -= fallTime * reductionAmount;
    }


    void PrintScore() {
        intscore = (int)playerDistanceTravelled;
        score.text = intscore.ToString();
    }
    void ScrollUpdate() {

        currentTime += Time.fixedDeltaTime;

        if (currentTime > scrollSpeedIncreaseTime) {
            currentScrollSpeed += scrollSpeedIncreaseAmount;
            currentTime = 0;
        }

        cameraManager.scrollUpSpeed = currentScrollSpeed;
        
    }

    void CalculateDistanceY() {
        playerDistanceTravelled -= player.transform.position.y - lastPlayerDistance ;
        lastPlayerDistance = player.transform.position.y;

        //Debug.Log(playerDistanceTravelled);
    }



    public void GameOver(){

        StartCoroutine(WaitEnd());
    }

    IEnumerator WaitEnd() {

        currentScrollSpeed = 0;
        cameraManager.scrollUpSpeed = currentScrollSpeed;

        stopScroll = true;

        yield return new WaitForSeconds(1f);

        isGameOver = true;

        int highscore = PlayerPrefs.GetInt("highscore");


        highscoreDisplay.SetActive(true);
        if (intscore > highscore) {
            PlayerPrefs.SetInt("highscore", intscore);
        }

        highscorevalue.text = PlayerPrefs.GetInt("highscore").ToString();


    }

    public void PressBegin() {
        if (!isGameOver) {
			if (!pressStart) {
				pressStart = true;
			}
				player.GetComponent<PlayerController> ().PressBegin ();

        }
        else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    public void PressEnd() {
        if (!isGameOver) {
            player.GetComponent<PlayerController>().PressEnd();
        }

    }

    public void PressDrag(Vector2 origin, Vector2 currentPosition) {
        if (!isGameOver && pressStart) {
            playerController.PressDrag(origin, currentPosition);
        }
    }
}
