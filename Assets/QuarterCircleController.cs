using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuarterCircleController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ball")
        {
            Color ballColor = collision.GetComponent<Ball>().GetColor();
            if (ballColor == spriteRenderer.color)
            {
                // increase size
            } else if (spriteRenderer.color == Color.black)
            {
                // increase size
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }


            Destroy(collision.gameObject);
        }
    }

}
