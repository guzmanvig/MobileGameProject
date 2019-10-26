using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public float speed = 2.8f;
    public Color possibleColor1;
    public Color possibleColor2;
    public SpriteRenderer spriteRenderer;

    private Color[] possibleColors;
    private Color myColor;
    private Rigidbody2D ballBody;


    void Start()
    {
        possibleColors = new Color[] { possibleColor1, possibleColor2};
        ballBody = GetComponent<Rigidbody2D>();

        RandomColor();
    }

    void FixedUpdate()
    {
        Move();
    }

    public Color GetColor()
    {
        return myColor;
    }

    private void RandomColor()
    {
        int randomIndex = Random.Range(0, 2);
        if (randomIndex == 0)
        {
            this.tag = "type1";
        } else
        {
            this.tag = "type2";
        }
        myColor = possibleColors[randomIndex];
        spriteRenderer.color = myColor;
    }

    private void Move()
    {
        Vector2 direction = new Vector2(-transform.up.x, -transform.up.y);
        ballBody.MovePosition(ballBody.position + direction * Time.fixedDeltaTime * speed);
    }
}
