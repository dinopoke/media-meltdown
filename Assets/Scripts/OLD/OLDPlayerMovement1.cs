using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OLDPlayerMovement : MonoBehaviour {

    enum PlayerState {onground, falling, jumping }

    PlayerState currentPlayerState;

    bool pressed = false;

    public float moveSpeed = 1f;
    public float jumpSpeedUp = 200f;
    public float jumpSpeedSide = 1f;

    int jumpDirection = 1;

    Vector2 playerOrigin;
    Vector2 lastPosition;

    float horSpeed;
    float verSpeed;

    Rigidbody2D rb;

    public Transform groundCheckL;
    public Transform groundCheckR;

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

    void ResetClamp() {
        clampLeft = -20f;
        clampRight = 20f;

    }

    void FixedUpdate() {
        CheckGrounded();
        CalculateSpeed();

        Debug.Log(currentPlayerState);
    }

    void CheckGrounded() {

        if (Physics2D.Linecast(transform.position, groundCheckL.position,  1 << LayerMask.NameToLayer("Platform"))) {
            leftGrounded = true;
        }
        else {
            leftGrounded = false;
        }
        if (Physics2D.Linecast(transform.position, groundCheckR.position,  1 << LayerMask.NameToLayer("Platform"))) {
            rightGrounded = true;

        }
        else {
            rightGrounded = false;
        }

        if (rightGrounded && !leftGrounded) {
            //clampLeft = transform.position.x;
        }
        else if(!rightGrounded && leftGrounded){
            //clampRight = transform.position.x;
        }

        Debug.Log(leftGrounded + " " + rightGrounded);

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
            Jump();
        }
        //ZeroVelocityX();


    }

    public void PressDrag(Vector2 origin, Vector2 currentPosition) {
        //Debug.Log("drag " + origin + " " + currentPosition );
        float distance_x = currentPosition.x - origin.x;
        
        if(currentPlayerState == PlayerState.onground) {
            MoveGround(distance_x);
        }
        else if( currentPlayerState == PlayerState.falling) {

            Move(distance_x);

        }

    }

    public void CalculateSpeed() {
        horSpeed = (rb.position.x - lastPosition.x) / Time.fixedDeltaTime;
        lastPosition = rb.position;

        CalculateJumpDirection(); 

    }


    
    void MoveGround(float distance) {


        Move(distance);
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



    void Jump() {
        //Debug.Log("up");
        ResetClamp();
        ZeroVelocity();


        rb.AddForce(new Vector2(jumpSpeedSide * jumpDirection *  Time.fixedDeltaTime, jumpSpeedUp * Time.fixedDeltaTime));


        currentPlayerState = PlayerState.falling;
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

    void ZeroVelocityX() {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }


    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag.Equals("WallLeft")) {
            clampLeft = transform.position.x;
        }
        else if (other.gameObject.tag.Equals("WallRight")) {
            clampRight = transform.position.x;

        }

        if (other.gameObject.tag.Equals("Platform") || other.gameObject.tag.Equals("StartPlatform")){
            if(leftGrounded || rightGrounded) {
                OnGround();
            }

        }
    
    }

}
