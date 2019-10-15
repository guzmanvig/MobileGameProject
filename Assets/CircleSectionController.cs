using System.Collections;
using UnityEngine;

public class CircleSectionController : MonoBehaviour {

    public float startAngleValue = 25f;
    public float endAngleValue = 45f;
    private Angle startAngle;
    private Angle endAngle;

    private Angle targetStartAngle;
    private Angle targetEndAngle;

    private float angleDecreaseIncreaseStep = 1f;

    private SpriteRenderer sr;

    private Color transparentColor = new Color(0f, 1f, 0f, 0f);
    private int radius;
    private int radiusSquared;
    ArrayList pointsInLine = new ArrayList();
    int textureWidth;

    private Color32[] fullColors;

    private Vector2Int center;

    private bool paintTransparent;


    void Start() {

        startAngle = new Angle(startAngleValue);
        endAngle = new Angle(endAngleValue);

        sr = GetComponent<SpriteRenderer>();

        // Get all values at start to imporve performance
        textureWidth = (int)sr.sprite.rect.width;
        radius = textureWidth / 2;
        radiusSquared = radius * radius;
        center = convertVectorFloatToVectorInt(sr.sprite.rect.center);


        Texture2D fullTexture = sr.sprite.texture;
        // Get the original colors of the full texture
        fullColors = fullTexture.GetPixels32();

        // Create a new emptytexture so we do not modify the original,
        Texture2D newTexture = new Texture2D(fullTexture.width, fullTexture.height, fullTexture.format, false);
        Color32[] pixels = new Color32[fullTexture.width * fullTexture.height];
        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = transparentColor;
        }
        newTexture.SetPixels32(pixels);
        newTexture.Apply();
        sr.sprite = Sprite.Create(newTexture, sr.sprite.rect, new Vector2(0.5f, 0.5f), sr.sprite.pixelsPerUnit);

        // Paint the initial angles
        paintTransparent = false;
        paintArc(sr.sprite.texture, startAngle.value, endAngle.value);

        targetStartAngle = startAngle;
        targetEndAngle = endAngle;

    }

    void Update() {

        if (!paintTransparent) {
            // Increaseing angle

            if (startAngle.value != targetStartAngle.value) {
                Angle nextAngle = startAngle.decreaseBy(angleDecreaseIncreaseStep);
                float differenceInAngles = nextAngle.value - targetStartAngle.value;
                // Last check is to solve when passing from 0 to 360
                if ((differenceInAngles < 0) && !(Mathf.Abs(differenceInAngles) > angleDecreaseIncreaseStep)) {
                    nextAngle = targetStartAngle;
                }

                Angle paintUpTo = startAngle.increaseBy(1); // The + 1 is to avoid border pixel not painted issues
                paintArc(sr.sprite.texture, nextAngle.value, paintUpTo.value);
                startAngle = nextAngle;
            }

            if (endAngle.value != targetEndAngle.value) {
                Angle nextAngle = endAngle.increaseBy(angleDecreaseIncreaseStep);
                float differenceInAngles = nextAngle.value - targetEndAngle.value;
                // Last check is to solve when passing from 360 to 0
                if ((differenceInAngles > 0) && !(Mathf.Abs(differenceInAngles) > angleDecreaseIncreaseStep)) {
                    nextAngle = targetEndAngle;
                }

                Angle paintUpTo = endAngle.decreaseBy(1); // The - 1 is to avoid border pixel not painted issues
                paintArc(sr.sprite.texture, paintUpTo.value, nextAngle.value);
                endAngle = nextAngle;
            }

        } else {
            // Decrease angle

            if (startAngle.value != targetStartAngle.value) {
                Angle nextAngle = startAngle.increaseBy(angleDecreaseIncreaseStep);
                float differenceInAngles = nextAngle.value - targetStartAngle.value;
                // Last check is to solve when passing from 0 to 360
                if ((differenceInAngles > 0) && !(Mathf.Abs(differenceInAngles) > angleDecreaseIncreaseStep)) {
                    nextAngle = targetStartAngle;
                }

                Angle paintUpTo = startAngle.decreaseBy(1); // The  -1 is to avoid border pixel not painted issues
                paintArc(sr.sprite.texture, paintUpTo.value, nextAngle.value);
                startAngle = nextAngle;
            }

            if (endAngle.value != targetEndAngle.value) {
                Angle nextAngle = endAngle.decreaseBy(angleDecreaseIncreaseStep);
                float differenceInAngles = nextAngle.value - targetEndAngle.value;
                // Last check is to solve when passing from 360 to 0
                if ((differenceInAngles < 0) && !(Mathf.Abs(differenceInAngles) > angleDecreaseIncreaseStep)) {
                    nextAngle = targetEndAngle;
                }

                Angle paintUpTo = endAngle.increaseBy(1); // The + 1 is to avoid border pixel not painted issues
                paintArc(sr.sprite.texture, nextAngle.value, paintUpTo.value);
                endAngle = nextAngle;
            }

        }

    }


    public bool increaseAngleBy(float angleToIncrease) {
        paintTransparent = false;
        bool reachedMaximumAngle = false;
        float maxAngleToIncrease = 360 - getTotalCurrentAngle();
        if (angleToIncrease > maxAngleToIncrease) {
            angleToIncrease = maxAngleToIncrease;
            reachedMaximumAngle = true;
        }
        targetStartAngle = startAngle.decreaseBy(angleToIncrease / 2);
        targetEndAngle = endAngle.increaseBy(angleToIncrease / 2);
        return reachedMaximumAngle;
    }

    private float getTotalCurrentAngle() {
        if (startAngle.value > endAngle.value) {
            return endAngle.value + 360 - startAngle.value;
        } else {
            return endAngle.value - startAngle.value;
        }
    }

    public bool decreaseAngleBy(float angleToDecrease, bool reduceFromStart) {
        paintTransparent = true;
        bool reachedMinimumAngle = false;
        float maxAngleToDecrease = getTotalCurrentAngle();
        if (angleToDecrease > maxAngleToDecrease) {
            angleToDecrease = maxAngleToDecrease;
            reachedMinimumAngle = true;
            Debug.Log("Reached minimum angle");
        }

        if (reduceFromStart) {
            Debug.Log("Start angle: " + startAngle.value + " will be increased by : " +angleToDecrease);
            targetStartAngle = startAngle.increaseBy(angleToDecrease);
        } else {
            Debug.Log("End angle: " + endAngle.value + " will be decreased by : " + angleToDecrease);
            targetEndAngle = endAngle.decreaseBy(angleToDecrease);
        }

        return reachedMinimumAngle;
    }

    private void paintArc(Texture2D texture, float startAngle, float endAngle) {
        if ((endAngle == startAngle) ||
            (endAngle < 0) ||
            (startAngle < 0) ||
            ((endAngle > startAngle) && (endAngle - startAngle > 90)) ||
            ((endAngle < startAngle) && (endAngle + 360 - startAngle > 90))) {
            throw new System.ArgumentException("Invalid angle");
        }

        Color32[] pixels = texture.GetPixels32();

        if (startAngle >= 0 && startAngle < 90) {
            if (endAngle <= 90) {
                paintStartingFromAndInDirection(pixels, startAngle, endAngle, 0);
            } else {
                paintStartingFromAndInDirection(pixels, startAngle, 90, 0);
                paintStartingFromAndInDirection(pixels, 90, endAngle, 1);
            }

        } else if (startAngle >= 90 && startAngle < 180) {
            if (endAngle <= 180) {
                paintStartingFromAndInDirection(pixels, startAngle, endAngle, 1);
            } else {
                paintStartingFromAndInDirection(pixels, startAngle, 180, 1);
                paintStartingFromAndInDirection(pixels, 180, endAngle, 2);
            }
        } else if (startAngle >= 180 && startAngle < 270) {
            if (endAngle <= 270) {
                paintStartingFromAndInDirection(pixels, startAngle, endAngle, 2);
            } else {
                paintStartingFromAndInDirection(pixels, startAngle, 270, 2);
                paintStartingFromAndInDirection(pixels, 270, endAngle, 3);
            }
        } else if (startAngle >= 270 && startAngle < 360) {
            if (endAngle >= 270 && endAngle <= 360) {
                paintStartingFromAndInDirection(pixels, startAngle, endAngle, 3);
            } else {
                paintStartingFromAndInDirection(pixels, startAngle, 360, 3);
                paintStartingFromAndInDirection(pixels, 0, endAngle, 0);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
    }

    private void paintStartingFromAndInDirection(Color32[] pixels, float startAngle, float endAngle, int quarter) {
        float angleToDrawFirst;
        float angleToDrawSecond;
        bool toRight = quarter == 0 || quarter == 3;
        if (quarter == 1 || quarter == 3) {
            angleToDrawFirst = startAngle;
            angleToDrawSecond = endAngle;
        } else {
            angleToDrawFirst = endAngle;
            angleToDrawSecond = startAngle;
        }

        ArrayList startLine = drawLine(pixels, angleToDrawFirst);

        Vector2Int previousPoint = (Vector2Int)startLine[0];
        bool isFirst = true; // Next point and previous point are the same for the first point, so we bypass the .y check with this boolean
        foreach (Vector2Int pointInStartLine in startLine) {
            Vector2Int nextPoint = pointInStartLine;
            if (isFirst || ((int)nextPoint.y != (int)previousPoint.y)) {
                isFirst = false;
                while (isInsideArc(nextPoint, angleToDrawSecond, quarter)
                // && isInsideRadious(center, nextPoint)){ // It's less expensive to go to the end of texture. Works because original tranpsaernt pixels are ignored
                && isInsideTexture(nextPoint)) {
                    paintPixel(pixels, nextPoint);
                    if (toRight) nextPoint.x++;
                    else nextPoint.x--;
                }
            }
            previousPoint = nextPoint;
        }

    }


    private ArrayList drawLine(Color32[] pixels, float angle) {
        Vector2 direction;
        if (angle == 0 || angle == 360) {  // Do this to avoid floating problems when the value is 0
            direction = new Vector2(1, 0);
        } else if (angle == 90) {
            direction = new Vector2(0, 1);
        } else if (angle == 180) {
            direction = new Vector2(-1, 0);
        } else if (angle == 270) {
            direction = new Vector2(0, -1);
        } else {
            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        Vector2Int currentPixel = center;
        Vector2Int currentRadius = currentPixel - center;

        pointsInLine.Clear();

        int i = 0;
        while (Mathf.Pow(currentRadius.x, 2) + Mathf.Pow(currentRadius.y, 2) < radiusSquared) {
            pointsInLine.Add(currentPixel);
            paintPixel(pixels, currentPixel);
            i++;
            currentPixel = convertVectorFloatToVectorInt(center + direction * i);
            currentRadius = currentPixel - center;
        }

        return pointsInLine;
    }

    private Vector2Int convertVectorFloatToVectorInt(Vector2 vector) {
        return new Vector2Int((int)vector.x, (int)vector.y);
    }

    private void paintPixel(Color32[] textureColors, Vector2 point) {
        int position = getPositionInFlattendArray(point);
        if (paintTransparent) textureColors[position] = transparentColor;
        else textureColors[position] = fullColors[position];
    }

    private int getPositionInFlattendArray(Vector2 point) {
        return textureWidth * (int)point.y + (int)point.x;
    }

    private bool isInsideArc(Vector2 point, float endArcAngle, int quarter) {
        if (center == point) {
            return true;
        }

        float angle = calculatePointAngle(center, point);
        bool isInside;
        if ((quarter == 0) || (quarter == 2)) {
            isInside = angle >= endArcAngle;
        } else {
            // Border case when chaning from 360 to 0
            if (angle > 0 && angle < 90) {
                return false;
            } else if (angle == 0) {
                angle = 360;
            }
            // Normal case
            isInside = angle <= endArcAngle;
        }
        return isInside;
    }

    private bool isInsideTexture(Vector2 nextPoint) {
        bool isInside = Mathf.Abs(nextPoint.x - center.x) < radius;
        return isInside;

    }
    private bool isInsideRadious(Vector2 nextPoint) {
        // Calculations are done in integer so it's less expensive
        int deltaX = (int)nextPoint.x - (int)center.x;
        int deltaY = (int)nextPoint.y - (int)center.y;
        bool isInside = (deltaX * deltaX + deltaY * deltaY) < radiusSquared;
        return isInside;

    }

    private float calculatePointAngle(Vector2 center, Vector2 point) {
        float deltaX = point.x - center.x;
        float deltaY = point.y - center.y;
        float angle = Mathf.Atan2(deltaY, deltaX) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    public float getStartAngle() {
        return startAngle.value;
    }

    public float getEndAngle() {
        return endAngle.value;
    }

    private class Angle {
        public readonly float value;
        public Angle(float angle) {
            if (angle >= 360) {
                value = angle - 360;
            } else if (angle < 0) {
                value = 360 + angle;
            } else {
                value = angle;
            }
        }

        public Angle increaseBy(float increaseAmount) {
            float newAngle = value + increaseAmount;
            return new Angle(newAngle);
        }

        public Angle decreaseBy(float decreaseAmount) {
            float newAngle = value - decreaseAmount;
            return new Angle(newAngle);
        }

    }
}
