using UnityEngine;
using System.Collections;

public class TouchControl : MonoBehaviour {

    public GameManager gameManager;

    Vector2 touchOrigin;
	bool touch = false;

	// Use this for initialization
	void Start () {

	}



	// Update is called once per frame
	void FixedUpdate () {
			CheckTouchInput ();

	}

    void CheckTouchInput() {



        if (Input.touchCount >= 1) {

            Touch touch1 = Input.touches[0];

            if (touch1.phase == TouchPhase.Began) {
                gameManager.PressBegin();
                touchOrigin = touch1.position;

            }

            else if (touch1.phase == TouchPhase.Moved){
                gameManager.PressDrag(touchOrigin, touch1.position);
            }

            else if (touch1.phase == TouchPhase.Ended) {

                gameManager.PressEnd();
            }
        }



    }
}
