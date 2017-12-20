using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCollider : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.transform.tag.Equals("Platform")) {
            DespawnObject(other.gameObject);
        }
        if (other.transform.tag.Equals("StartPlatform")) {
            DespawnObject(other.gameObject);
        }

        if (other.transform.tag.Equals("Player") ) {
            GameObject.Find("Game Manager").GetComponent<GameManager>().GameOver();
        }

    }

    void DespawnObject(GameObject obj) {
        Destroy(obj);
    }
}
