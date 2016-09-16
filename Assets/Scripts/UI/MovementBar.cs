using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovementBar : MonoBehaviour {
    public Unit unit;
    RectTransform rectTransform;
    float initialWidth;

    void Start() {
//        movementScript = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonController>();
        unit = GameManager.instance.activeUnit.GetComponent<Unit>();
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.rect.width;
    }

    void Update() {
        if (unit == null) {
            transform.parent.gameObject.SetActive(false);
            Debug.Log("deactivating movement bar");
            return;
        }
        float pctMovementRemaining = unit.currentMovementPoints / unit.maxMovementPoints;
        float desiredWidth = pctMovementRemaining * initialWidth;
//        float deltaWidth = desiredWidth - rectTransform.rect.width;
        rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.rect.height);
    }
}
