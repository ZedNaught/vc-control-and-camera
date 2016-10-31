using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovementBar : MonoBehaviour {
    RectTransform rectTransform;
    float initialWidth;
    bool barActive = false;
    WaitForSeconds pollingIntervalSeconds = new WaitForSeconds(.1f);
    public static MovementBar Instance {get; set;}

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }
        rectTransform = GetComponent<RectTransform>();
        initialWidth = rectTransform.rect.width;
    }

//    void OnDestroy() {
//        instance.StopCoroutine("TrackUnitMovement");
//    }

    public void ActivateBar() {
        barActive = true;
        transform.parent.gameObject.SetActive(true);
        StartCoroutine("TrackUnitMovement");
    }

    public void DeactivateBar(bool stopCoroutine=true) {
        barActive = false;
        transform.parent.gameObject.SetActive(false);
        StopCoroutine("TrackUnitMovement");
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
