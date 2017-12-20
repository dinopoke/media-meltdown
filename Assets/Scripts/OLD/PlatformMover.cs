using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour {

    Rigidbody2D rb;

    public float moveUpSpeed = 0.05f;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.MovePosition(new Vector2(rb.position.x, rb.position.y + moveUpSpeed));  
	}
}
