using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleController : MonoBehaviour {
    public float speed = 100f; //100 degrees per second
    public bool rotateClockwise;
    public bool rotate = false;

    List<CircleSectionController> circleSections = new List<CircleSectionController>();


    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject circleComponent = gameObject.transform.GetChild(i).gameObject;
            CircleSectionController section = circleComponent.GetComponent<CircleSectionController>();
            circleSections.Add(section);
        }

    }

    // Update is called once per frame
    void Update() {
        if (rotate) {
            int sign = rotateClockwise ? -1 : 1;
            transform.Rotate(0, 0, sign * speed * Time.deltaTime); // delta time is to make it 100 per second, and not per frame
        }

    }

    public void StartRotation(bool clockwise) {
        rotateClockwise = clockwise;
        rotate = true;
    }

    public void StopRotation(bool clockwise) {
        // Only stop if its the current rotation
        if ((rotateClockwise && clockwise) || (!rotateClockwise && !clockwise)) {
            rotate = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        float deltaX = collision.transform.position.x - transform.position.x;
        float deltaY = collision.transform.position.y - transform.position.y;
        
        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        float rotation = transform.rotation.eulerAngles.z;
        if (rotation < 0) {
            rotation = 360 - rotation;
        }

        float collisionAngleLessRotation = angle - rotation;
        if (collisionAngleLessRotation < 0) {
            collisionAngleLessRotation = 360 + collisionAngleLessRotation;
        }

        CircleSectionController sectionHit = null;
        foreach (CircleSectionController circleSection in circleSections) {
            float sectionStartAngle = circleSection.getStartAngle();
            float sectionEndAngle = circleSection.getEndAngle();
            if (sectionStartAngle <= sectionEndAngle) {
                if (collisionAngleLessRotation >= sectionStartAngle && collisionAngleLessRotation < sectionEndAngle) {
                    sectionHit = circleSection;
                    break;
                }
            } else { // The arc is inverted. Split the check in two.
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
            increaseSection(sectionHit, expandBy);
            resizeNeighborSections(sectionHit, expandBy / 2);
        } else if (sectionHit.tag == "typeNeutral") {
            Debug.Log("Hit neutral section");
            increaseSection(sectionHit, expandBy);
            resizeNeighborSections(sectionHit, expandBy / 2);
        } else {
            Debug.Log("Hit incorrect section. Tag hit: " + sectionHit.tag + " with ball: "+ collision.tag);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        Destroy(collision.gameObject);

    }

    private void decreaseSection(CircleSectionController section, float angle, bool fromStart) {
        Debug.Log("Decreaseing angle by: " + angle + " from start: " + fromStart);
        section.decreaseAngleBy(angle, fromStart);
    }

    private void increaseSection(CircleSectionController section, float angle) {
        Debug.Log("Increasing angle by: " + angle);
        section.increaseAngleBy(angle);
    }

    private void resizeNeighborSections(CircleSectionController section, float angle) {
        if (circleSections.IndexOf(section) == 0) {
            decreaseSection(circleSections[circleSections.Capacity - 1], angle, false);
            decreaseSection(circleSections[circleSections.IndexOf(section) + 1], angle, true);
        } else if (circleSections.IndexOf(section) == circleSections.Capacity - 1) {
            decreaseSection(circleSections[circleSections.IndexOf(section) - 1], angle, false);
            decreaseSection(circleSections[0], angle, true);
        } else {
            decreaseSection(circleSections[circleSections.IndexOf(section) - 1], angle, false);
            decreaseSection(circleSections[circleSections.IndexOf(section) + 1], angle, true);
        }
    }
}
