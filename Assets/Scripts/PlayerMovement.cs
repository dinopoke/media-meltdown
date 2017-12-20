using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    enum PlayerState {onground, falling}

    PlayerState currentPlayerState;

    bool pressed = false;

    public float moveSpeed = 2;

    int jumpDirection = 1;

    Vector2 playerOrigin;
    Vector2 lastPosition;

    float horSpeed;
    float verSpeed;

    Rigidbody2D rb;

    bool leftGrounded;
    bool rightGrounded;


    float clampLeft;
    float clampRight;

	// Use this for initialization
	void Start () {

        currentPlayerState = PlayerState.falling;
        rb = gameObject.GetComponent<Rigidbody2D>();
        ResetClamp();

	}


    void FixedUpdate() {
        CheckGrounded();
        CalculateSpeed();

        Debug.Log(currentPlayerState);
    }

    void ResetClamp() {
        clampLeft = -20f;
        clampRight = 20f;

    }


    public void CalculateSpeed() {
        horSpeed = (rb.position.x - lastPosition.x) / Time.fixedDeltaTime;
        lastPosition = rb.position;

        CalculateJumpDirection(); 

    }

    void CheckGrounded() {

    }

    public void PressBegin() {
        //Debug.Log("press");
        playerOrigin = rb.position;
        lastPosition = rb.position;
        pressed = true;

    }

    public void PressEnd() {
        //Debug.Log("release");
        pressed = false;
        if (currentPlayerState == PlayerState.onground) {

        }



    }

    public void PressDrag(Vector2 origin, Vector2 currentPosition) {
        //Debug.Log("drag " + origin + " " + currentPosition );
        float distance_x = currentPosition.x - origin.x;
        
        Move(distance_x);


    }


    void Move(float distance) {
        rb.position  = new Vector2((ClampMovement(distance)), rb.position.y);
    }

    float ClampMovement(float distance) {
        float clampedDistance = playerOrigin.x + distance * moveSpeed * Time.fixedDeltaTime;

        if (clampedDistance < clampLeft) {
            clampedDistance = clampLeft;
        }
        else if (clampedDistance > clampRight) {
            clampedDistance = clampRight;
        }
        return clampedDistance;
    }


    void CalculateJumpDirection() {
        if (horSpeed != 0) {
            jumpDirection = (int) Mathf.Sign(horSpeed);
        }
    }

    

    void OnGround() {
        ZeroVelocity();
        currentPlayerState = PlayerState.onground;
    }

    void ZeroVelocity() {
        rb.velocity = Vector2.zero;
    }


    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("WallLeft")) {
            clampLeft = transform.position.x;
        }
        else if (other.gameObject.tag.Equals("WallRight")) {
            clampRight = transform.position.x;

        }

        if (other.gameObject.tag.Equals("Platform") || other.gameObject.tag.Equals("StartPlatform")){
            OnGround();


        }
    
    }

}
