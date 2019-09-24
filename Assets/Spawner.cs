using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float spawnDelay = 1.8f;
    public GameObject ballPrefab;
    private float nextTimeToSpawn;

    public float maxAngle = 40f;

    void Update()
    {
        if (nextTimeToSpawn <= Time.time)
        {
            SpawnBall();
            nextTimeToSpawn = Time.time + spawnDelay;
        }

        
    }

    private void SpawnBall()
    {
        var screenhHeight = 2 * Camera.main.orthographicSize;
        var screenWidth = screenhHeight * Camera.main.aspect;
        var halfScreenWidth = screenWidth / 2;
        var halfScreenHeight = screenhHeight / 2;

        float angleFromCenter = Random.Range(-maxAngle, maxAngle);

        float randomScale = Random.Range(0.3f, 1f);

        float ballRadius = ballPrefab.GetComponent<CircleCollider2D>().radius * randomScale;

        float margin = 0.05f; // Margin because the ball radius is a little less than the actual radius
       
        var distanceToOutOfScreen = Mathf.Sign(angleFromCenter) * (halfScreenWidth + ballRadius + margin) / Mathf.Sin(angleFromCenter * Mathf.PI / 180);

        transform.Rotate(0f, 0f, angleFromCenter);


        float startXPosition = transform.position.x + transform.up.x * distanceToOutOfScreen;
        float startYPosition = transform.position.y + transform.up.y * distanceToOutOfScreen;

        if (startYPosition > halfScreenHeight)
        {                                                                          // - because position y is negative
           distanceToOutOfScreen = (halfScreenHeight - transform.position.y + ballRadius + margin) / Mathf.Cos(angleFromCenter * Mathf.PI / 180);
           startXPosition = transform.position.x + transform.up.x * distanceToOutOfScreen;
           startYPosition = transform.position.y + transform.up.y * distanceToOutOfScreen;
        }


        Vector2 position = new Vector2(startXPosition, startYPosition);
        GameObject ballInstance = Instantiate(ballPrefab, position, transform.rotation);

        //TODO: im doing this because i need to know the scale to calculate the position. if I instantate it first, maybe
        // the ball can scale itself. I need to figure out how to move the object, so I can instntiate it first and then move it
        ballInstance.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        transform.Rotate(0f, 0f, -angleFromCenter);


    }


}
