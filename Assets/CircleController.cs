using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleController : MonoBehaviour
{
    public float speed = 100f; //100 degrees per second
    public bool rotateClockwise;
    public bool rotate = false;

    List<Shapes2D.Shape> circleSections = new List<Shapes2D.Shape>();


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject circleComponent = gameObject.transform.GetChild(i).gameObject;
            Shapes2D.Shape shape = circleComponent.GetComponent<Shapes2D.Shape>();
            circleSections.Add(shape);
        }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        float deltaX = collision.transform.position.x - transform.position.x;
        float deltaY = collision.transform.position.y - transform.position.y;
        float angle;
        if (deltaX > 0 && deltaY > 0) {
            angle = Mathf.Atan(deltaY / deltaX) * 180 / Mathf.PI;
        } else if (deltaX < 0 && deltaY > 0) {
            angle = 180 - (Mathf.Atan(deltaY / -deltaX) * 180 / Mathf.PI);
        } else if (deltaX < 0 && deltaY < 0) {
            angle = 270 - (Mathf.Atan(-deltaY / -deltaX) * 180 / Mathf.PI);
        } else {
            angle = 360 - (Mathf.Atan(-deltaY / deltaX) * 180 / Mathf.PI);
        }

        float rotation = transform.rotation.eulerAngles.z;
        if (rotation < 0) {
            rotation = 360 - rotation;
        }

        float collisionAngleLessRotation = angle - rotation;
        if (collisionAngleLessRotation < 0) {
            collisionAngleLessRotation = 360 + collisionAngleLessRotation;
        }

        Shapes2D.Shape sectionHit = null;
        foreach (Shapes2D.Shape circleSection in circleSections) {
            float sectionStartAngle = circleSection.settings.startAngle;
            float sectionEndAngle = circleSection.settings.endAngle;
            if (sectionStartAngle <= sectionEndAngle) {
                if (collisionAngleLessRotation >= sectionStartAngle && collisionAngleLessRotation < sectionEndAngle)
                {
                    sectionHit = circleSection;
                    break;
                }
            } else { // The arc is inverted. split it two
                if ((collisionAngleLessRotation >= 0 && collisionAngleLessRotation < sectionEndAngle) 
                    || (collisionAngleLessRotation >= sectionStartAngle && collisionAngleLessRotation < 360)) {

                    sectionHit = circleSection;
                    break;
                }
            }
            
        }

        float ballSize = collision.bounds.size.x;
        float expandBy = ballSize * 20;
        if (collision.tag == sectionHit.tag) {
            Debug.Log("Hit correct section");
            resizeHitSection(sectionHit, expandBy);
            resizeNeighborSections(sectionHit, expandBy / 2);
        } else if (sectionHit.tag == "typeNeutral") {
            Debug.Log("Hit neutral section");
            resizeHitSection(sectionHit, expandBy);
            resizeNeighborSections(sectionHit, expandBy / 2);
        }
        else {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        Destroy(collision.gameObject);

    }

    private void resizeSection(Shapes2D.Shape section, float deltaStart, float deltaEnd) {
        float sectionStartAngle = section.settings.startAngle;
        float sectionEndAngle = section.settings.endAngle;

        float startAngleCandidate = sectionStartAngle - deltaStart;
        float endAngleCandidate = sectionEndAngle + deltaEnd;

        if (startAngleCandidate < 0) {
            startAngleCandidate = 360 + startAngleCandidate;
            section.settings.startAngle = startAngleCandidate;
            section.settings.endAngle = endAngleCandidate;
            section.settings.invertArc = true;
        } else if (endAngleCandidate > 360) {
            endAngleCandidate = endAngleCandidate - 360;
            section.settings.startAngle = startAngleCandidate;
            section.settings.endAngle = endAngleCandidate;
            section.settings.invertArc = true;
        } else {
            section.settings.startAngle = startAngleCandidate;
            section.settings.endAngle = endAngleCandidate;
        }
    }

    private void resizeHitSection(Shapes2D.Shape section, float angle) {
        resizeSection(section, angle / 2, angle / 2);
    }

    private void resizeNeighborSections(Shapes2D.Shape section, float angle) {
        if (circleSections.IndexOf(section) == 0) {
            resizeSection(circleSections[circleSections.Capacity - 1], 0, -angle);
            resizeSection(circleSections[circleSections.IndexOf(section) + 1], -angle, 0);
        } else if (circleSections.IndexOf(section) == circleSections.Capacity - 1) {
            resizeSection(circleSections[circleSections.IndexOf(section) - 1], 0, -angle);
            resizeSection(circleSections[0], -angle, 0);
        } else {
            resizeSection(circleSections[circleSections.IndexOf(section) - 1], 0, -angle);
            resizeSection(circleSections[circleSections.IndexOf(section) + 1], -angle, 0);
        }
    }
}
