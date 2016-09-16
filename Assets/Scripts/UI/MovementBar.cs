using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovementBar : MonoBehaviour {
    ThirdPersonController movementScript;
    RectTransform rectTransform;
    float initialWidth;

    void Start() {
        movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.rect.width;
    }

    void Update() {
        float pctMovementRemaining = movementScript.currentMovementPoints / movementScript.maxMovementPoints;
        float desiredWidth = pctMovementRemaining * initialWidth;
//        float deltaWidth = desiredWidth - rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.rect.height);
    }
}
