using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovementBar : MonoBehaviour {
    RectTransform rectTransform;
    float initialWidth;
    bool barActive = false;
    WaitForSeconds pollingIntervalSeconds = new WaitForSeconds(.1f);
    public static MovementBar instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.rect.width;
    }

//    void OnDestroy() {
//        instance.StopCoroutine("TrackUnitMovement");
//    }

    public static void ActivateBar() {
        instance.barActive = true;
        instance.transform.parent.gameObject.SetActive(true);
        instance.StartCoroutine("TrackUnitMovement");
    }

    public static void DeactivateBar(bool stopCoroutine=true) {
        instance.barActive = false;
        instance.transform.parent.gameObject.SetActive(false);
        instance.StopCoroutine("TrackUnitMovement");
    }

    IEnumerator TrackUnitMovement() {
        while (true) {
            if (ThirdPersonController.ActiveUnit != null) {
                if (!barActive)
                    ActivateBar();
                Unit unit = ThirdPersonController.ActiveUnit;
                float pctMovementRemaining = unit.currentMovementPoints / unit.maxMovementPoints;
                float desiredWidth = pctMovementRemaining * initialWidth;
                rectTransform.sizeDelta = new Vector2(desiredWidth, rectTransform.rect.height);
            }
            else if (barActive) {
                DeactivateBar();
            }
            yield return pollingIntervalSeconds;
        }
    }
}
