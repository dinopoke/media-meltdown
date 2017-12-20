using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour {

    Vector2 mouseOrigin;

    
    public GameManager gameManager;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
            gameManager.PressBegin();
            mouseOrigin = Input.mousePosition;
        }
        
        if (Input.GetMouseButton(0)) {
            gameManager.PressDrag(mouseOrigin, Input.mousePosition);
        }
        
        if (Input.GetMouseButtonUp(0)) {
            gameManager.PressEnd();
        }
    }




}
