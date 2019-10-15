using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateButton : EventTrigger {
    public CircleController circle;

    void Start() {
        circle = GameObject.FindWithTag("circle").GetComponent<CircleController>();
    }


    public override void OnPointerDown(PointerEventData data) {
        circle.StartRotation(tag == "clockWise");
    }

    public override void OnPointerUp(PointerEventData data) {
        circle.StopRotation(tag == "clockWise");
    }


}
