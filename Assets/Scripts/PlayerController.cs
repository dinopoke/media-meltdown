using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    public enum PlayerState { onground, falling, rolling, dead }

    public PlayerState currentPlayerState;

    [Tooltip("What layer the player collides with")]
    public LayerMask collisionMask;

    const float skinWidth = 0.5f;

    [Tooltip("Number of horizontal rays for player collision")]
    public int horizontalRayCount = 4;
    [Tooltip("Number of vertical rays for player collision")]
    public int verticalRayCount = 4;

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;

    Animator animator;
    SpriteRenderer spriteRenderer;

	public Camera playerCamera;

    public CollisionInfo collisions;

    bool pressed = false;

    Vector2 playerOrigin;
    Vector2 lastPosition;

    

    public float clampLeft;
    public float clampRight;

    [Header("Movement")]
    [Tooltip("Not sure what this is here for")]
    public float moveSpeed = 1f;
    public float moveDelay = 0.05f;
    [Tooltip("Gravity of the player (negative is down)")]
    public float gravity = -20;
    [Header("Roll")]
    [Tooltip("Distance traveled when you roll")]
    public float rollDistance;
    [Tooltip("Duration of roll")]
    public float rollTime = 1f;
    [Tooltip("Duration of roll immunity")]
    public float rollImmunity;

    Vector3 velocity;
    float horSpeed;
    bool walking;
    int moveDirection;

    public float fallThreshold = -0.005f;

    float hitVelocityY;

	public float rollCooldownLength = 0.5f;
    public float rollSlowAmount = 0.5f;
	bool canRoll;

	float moveToPosition;

    void Start()
    {
		velocity = Vector2.zero;

        boxCollider = GetComponent<BoxCollider2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        CalculateRaySpacing();

        currentPlayerState = PlayerState.onground;

        ResetClamp();
    }

    void FixedUpdate()
    {   
        if(currentPlayerState != PlayerState.dead) {

            if (collisions.below)
            {
                velocity.y = 0;
            }



            velocity.y += gravity * Time.deltaTime;

            Move(velocity * Time.deltaTime);

            //Debug.Log(currentPlayerState);
            //Debug.Log(velocity);
            transform.position = new Vector2(ClampMovementX(transform.position.x), transform.position.y);

            CalculateSpeed();
            SetPlayerFaceDirection();
            CheckPlayerWalk();

            CheckDead();

        }
    }

    void CheckDead() {
        if (currentPlayerState == PlayerState.dead) {
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator KillPlayer() {
        animator.SetTrigger("Death");
        yield return null;
        gameManager.GameOver();
    }

    void SetPlayerFaceDirection() {
        if (moveDirection > 0) {

            spriteRenderer.flipX = true;
        }
        else {
            spriteRenderer.flipX = false;

        }
    }

    void CheckPlayerWalk() {

        if (currentPlayerState == PlayerState.onground) {
            if (horSpeed != 0) {
                if (!walking) {
                    animator.ResetTrigger("OnGround");

                    animator.SetTrigger("Walk");
                    walking = true;
                }
            }
            else {
                animator.SetTrigger("OnGround");

                walking = false;

            }
        }
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        //if (velocity.x != 0)
        //{
            HorizontalCollisions(ref velocity);
        //}
        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        bool stateHit = false;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                hitVelocityY = velocity.y;
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;

                stateHit = true;
            }
        }   
       	if(stateHit)
            {
                if (currentPlayerState == PlayerState.falling) {
                    animator.ResetTrigger("Fall");

                    currentPlayerState = PlayerState.dead;
                    //currentPlayerState = PlayerState.onground;


                }
                else if(currentPlayerState == PlayerState.rolling) {

                    //animator.SetTrigger("OnGround");
                    currentPlayerState = PlayerState.onground;
					canRoll = true;
					

                }

                velocity.x = 0;
            }
            else
            {
                if (currentPlayerState == PlayerState.onground) {




                    if (velocity.y < fallThreshold) {

                        currentPlayerState = PlayerState.falling;
                        animator.SetTrigger("Fall");
                        animator.ResetTrigger("OnGround");

                    }
                    
                }
            }
        
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        bool stateHit = false;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {


                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                stateHit = true;
            }


        }
        if (stateHit) {
//			if (directionX == -1) {
//
//
//				clampRight= transform.position.x - 0.001f;
//				Debug.Log ("hitright " + clampRight);
//			}
//			else if (directionX == 1) {
//
//				clampLeft = transform.position.x + 0.001f;
//				Debug.Log ("hitleft " + clampLeft);
//			}	
        }
        else {
            
        }
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public void PressBegin()
    {
        //Debug.Log("press");
        playerOrigin = transform.position;
        lastPosition = transform.position;
        pressed = true;

    }

    public void PressEnd()
    {
        //Debug.Log("release");
        pressed = false;
        if (currentPlayerState == PlayerState.onground)
        {
            velocity.x = 0;
        }
        else if (currentPlayerState == PlayerState.falling){
            Roll();
        }


    }

    public void PressDrag(Vector2 origin, Vector2 currentPosition)
    {
        //Debug.Log("drag " + origin + " " + currentPosition );
        float distance_x = currentPosition.x - origin.x;

        if (currentPlayerState == PlayerState.onground)
        {
            MoveGround(distance_x);
        }
        else if (currentPlayerState == PlayerState.falling)
        {
            Move(distance_x);
        }

    }

    void Roll()
    {
        if(currentPlayerState == PlayerState.onground)
        {
            
        }
        else if(currentPlayerState == PlayerState.falling) {
            currentPlayerState = PlayerState.rolling;

            velocity.y = velocity.y * rollSlowAmount;
            StartCoroutine(MovingRoll());
			StartCoroutine (RollCooldown());
        }



    }

	IEnumerator RollCooldown(){
		canRoll = false;
		yield return new WaitForSeconds (rollCooldownLength);
		canRoll = true;

	}

    void Move(float distance)
    {
        //transform.position = new Vector2((ClampMovement(distance)), transform.position.y);

		//transform.Translate = new

		moveToPosition = playerOrigin.x + distance * moveSpeed * Time.fixedDeltaTime;
        moveToPosition = ClampMovementX(moveToPosition);

		transform.position =  new Vector2(Mathf.SmoothDamp(transform.position.x, moveToPosition, ref velocity.x, moveDelay), transform.position.y);



    }

    void MoveGround(float distance)
    {
        Move(distance);
    }

    float ClampMovementX(float position)
    {

        if (position <= clampLeft)
        {

            position = clampLeft;
        }
        else if (position >= clampRight)
        {
            position = clampRight;
        }
		//Debug.Log (clampedDistance);
        return position;
    }

    IEnumerator MovingRoll()
    {
        //transform.position = Vector3.Lerp(playerOrigin, new Vector3(playerOrigin.x + rollDistance, transform.position.y), 1);
        //yield return null;
        //Debug.Log("Roll Start");

        

        //playerOrigin = transform.position;
        //lastPosition = transform.position;
        //
        //for (float i = 0; i < rollTime; i += Time.deltaTime)
        //{
        //    Move((rollDistance / rollTime));
        //    yield return null;
        //}

        //if (currentPlayerState != PlayerState.onground)
        //    currentPlayerState = PlayerState.falling;

        //Debug.Log("Roll End");
        animator.SetBool("Rool", true);
        spriteRenderer.color = new Color32(254, 174,52, 255);

        for (float i = 0; i < rollTime; i += Time.fixedDeltaTime) {


            yield return null;
        }
        animator.SetBool("Rool", false);
        spriteRenderer.color = Color.white;

        if (currentPlayerState != PlayerState.onground && currentPlayerState != PlayerState.dead) {
            currentPlayerState = PlayerState.falling;
            
            animator.SetTrigger("Fall");
        }

        if (currentPlayerState == PlayerState.dead) {
            animator.SetTrigger("Dead");

        }
        if (currentPlayerState == PlayerState.onground) {
            animator.SetTrigger("OnGround");

        }

    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below, left, right;

        public void Reset()
        {
            above = below = left = right = false;
        }
    }

    void CalculateSpeed() {
        horSpeed = (transform.position.x - lastPosition.x) / Time.fixedDeltaTime;
        lastPosition = transform.position;

        CalculateMoveDirection(); 

    }

    void CalculateMoveDirection() {
        if (horSpeed != 0) {
            moveDirection = (int) Mathf.Sign(horSpeed);
        }
    }

    void ResetClamp() {
		clampLeft = playerCamera.ViewportToWorldPoint(new Vector3 (0, 0, 0)).x + boxCollider.size.x;
		clampRight = playerCamera.ViewportToWorldPoint(new Vector3 (1, 0, 0)).x - boxCollider.size.x;
	}

}