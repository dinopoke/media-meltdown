using UnityEngine;
using System.Collections;
     
public class CameraManager : MonoBehaviour {
         
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;

    public Camera playerCamera;
    
    public float scrollUpSpeed = 1f;
    public float clampPositionY = 0.65f;
     
    void Start() {


    }

    // Update is called once per frame
    void LateUpdate () 
    {
        if (target)
        {
            Vector3 point = playerCamera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - playerCamera.ViewportToWorldPoint(new Vector3(0.5f, clampPositionY, point.z)); 
            Vector3 destination = transform.position + delta;

            destination.x = 0;

            if (point.y < clampPositionY) {
                transform.position = Vector3.Lerp(transform.position, destination, 1);
                //transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime );

            }

            transform.Translate(-Vector3.up * scrollUpSpeed * Time.fixedDeltaTime);
        }
    }
}
