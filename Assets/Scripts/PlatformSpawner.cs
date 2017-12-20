using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour {

    public GameObject platform;

    public Transform spawnStartArea;
    public Transform spawnEndArea;


    public float spawnDistanceMin;
    public float spawnDistanceMax;

    float spawnDistance;

    float currentPositionY;

    float spawnX;

	// Use this for initialization
	void Start () {
        currentPositionY = transform.position.y;
        spawnDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.y < currentPositionY - spawnDistance) {

            SpawnRandomPlatform();
            spawnDistance = Random.Range(spawnDistanceMin, spawnDistanceMax);
            currentPositionY = transform.position.y;
        }
	}

    void SpawnRandomPlatform () {
        spawnX = Random.Range(spawnStartArea.position.x, spawnEndArea.position.x);
        Instantiate(platform,new Vector3(spawnX, spawnStartArea.position.y, 0) , Quaternion.identity);
    }

}
