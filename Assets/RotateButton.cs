using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    public CircleController circle;

    private void OnMouseDown()
    {
        circle.StartRotation(tag == "clockWise");
    }

    private void OnMouseUp()
    {
        circle.StopRotation();
    }


}
