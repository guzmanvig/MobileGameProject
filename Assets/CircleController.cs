using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CircleController : MonoBehaviour {
    public float speed = 100f; //100 degrees per second
    public bool rotateClockwise;
    public bool rotate = false;
    public GameObject winText;

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
            //TODO: increase both sides only if there is a not complete neighbor section, if not, only resize  the other neighbor
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

    private bool decreaseSection(CircleSectionController section, float angle, bool fromStart) {
        Debug.Log("Decreaseing angle by: " + angle + " from start: " + fromStart);
        return section.decreaseAngleBy(angle, fromStart);
    }

    private bool increaseSection(CircleSectionController section, float angle) {
        Debug.Log("Increasing angle by: " + angle);
        return section.increaseAngleBy(angle);
    }

    private void resizeNeighborSections(CircleSectionController section, float angle) {
        CircleSectionController previousSection = getPreviousSection(section);
        if (previousSection.isAtMinimum()) {
            // If this is already at minimum, skip it
            previousSection = getPreviousSection(previousSection);
        }
        CircleSectionController nextSection = getNextSection(section);
        if (nextSection.isAtMinimum()) {
            // If this is already at minimum, skip it
            nextSection = getNextSection(nextSection);
        }
        bool previousReachedMinimum = decreaseSection(previousSection, angle, false);
        bool nextReachedMinimum = decreaseSection(nextSection, angle, true);
        if (previousReachedMinimum) {
           if (previousSection.tag == "typeNeutral") {
                if (nextReachedMinimum) {
                    // Both neutral sections are gone. This needs to be changed if more than 2 neutrals
                    Debug.Log("WON!");
                    winText.SetActive(true);
                }
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("LOST!");
            }
        }
        if (nextReachedMinimum) {
            if (nextSection.tag == "typeNeutral") {
                if (previousReachedMinimum) {
                    // Both neutral sections are gone. This needs to be changed if more than 2 neutrals
                    winText.SetActive(true);
                    Debug.Log("WON!");
                }
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("LOST!");
            }
        }
    }

    private CircleSectionController getNextSection(CircleSectionController section) {
        CircleSectionController nextSection;
        if (circleSections.IndexOf(section) == 0) {
            nextSection = circleSections[circleSections.IndexOf(section) + 1];
        } else if (circleSections.IndexOf(section) == circleSections.Capacity - 1) {
            nextSection = circleSections[0];
        } else {
            nextSection = circleSections[circleSections.IndexOf(section) + 1];
        }
        return nextSection;
    }

    private CircleSectionController getPreviousSection(CircleSectionController section) {
        CircleSectionController previousSection;
        if (circleSections.IndexOf(section) == 0) {
            previousSection = circleSections[circleSections.Capacity - 1];
        } else if (circleSections.IndexOf(section) == circleSections.Capacity - 1) {
            previousSection = circleSections[circleSections.IndexOf(section) - 1];
        } else {
            previousSection = circleSections[circleSections.IndexOf(section) - 1];
        }
        return previousSection;
    }
}
