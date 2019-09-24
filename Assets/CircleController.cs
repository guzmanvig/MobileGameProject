using UnityEngine;

public class CircleController : MonoBehaviour
{
    public float speed = 100f; //100 degrees per second
    public bool rotateClockwise;
    public bool rotate = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            int sign = rotateClockwise ? -1 : 1;
            transform.Rotate(0, 0, sign * speed * Time.deltaTime); // delta time is to make it 100 per second, and not per frame
        }

    }

    public void StartRotation(bool clockwise)
    {
        rotateClockwise = clockwise;
        rotate = true;
    }

    public void StopRotation()
    {
        rotate = false;
    }
}
