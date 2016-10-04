using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour {
//    [SerializeField] Transform unit;
    [SerializeField] Vector3 initialIconScale = new Vector3(1f, 1f, 1f);
    private RectTransform rectTransform;
    private Camera overviewCam;
    private Image iconImage;
    private float iconScale = 20f;
    public Transform Unit {get; set;}

	void Start () {
	    overviewCam = GameObject.FindGameObjectWithTag("OverviewCamRig").GetComponentInChildren<Camera>();
        iconImage = GetComponentInChildren<Image>();
        rectTransform = GetComponent<RectTransform>();
	}
	
	void Update () {
        Vector3 screenPoint = overviewCam.WorldToScreenPoint(Unit.position);
        rectTransform.position = screenPoint;
	}

    void ResizeIcon() {
        iconImage.rectTransform.localScale = initialIconScale * iconScale * 1f/overviewCam.orthographicSize;
    }
}
