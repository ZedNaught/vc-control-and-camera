using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverviewCam : MonoBehaviour {
    public float moveSpeed = 1f;
//    public float cameraHeight = 8f;
//    float acceleration = 1f;
    float horizontalInput;
    float verticalInput;
    GameObject unitIconContainer;
    [SerializeField] GameObject unitIconPrefab;
    [SerializeField] GameObject canvas;
    List<GameObject> unitIcons = new List<GameObject>();

    void Awake() {
    }

//    void Start() {
//    }

    void OnEnable() {
        CreateUnitIcons();
        CenterAboveUnits();
    }

    void OnDisable() {
        DestroyUnitIcons();
    }

    void Update () {
        GetInput();
	    MoveCamera();
	}

    void CenterAboveUnits() {
        List<Unit> friendlyUnits = Unit.FriendlyUnits;
        Vector3 positionsSum = Vector3.zero;
        foreach (Unit unit in friendlyUnits) {
            positionsSum += unit.transform.position;
        }
        Vector3 avgPosition = positionsSum / friendlyUnits.Count;
        transform.position = new Vector3(avgPosition.x, transform.position.y, avgPosition.z);
    }

    void GetInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void MoveCamera() {
        transform.position += moveSpeed * (new Vector3(horizontalInput, 0f, verticalInput));
    }

    void CreateUnitIcons() {
        unitIconContainer = new GameObject("Unit Icon Container");
        unitIconContainer.transform.parent = canvas.transform;
        foreach (Unit unit in Unit.allUnits) {
            GameObject unitIcon = Instantiate(unitIconPrefab, unit.transform.position, Quaternion.identity, unitIconContainer.transform) as GameObject;
            unitIcon.GetComponent<UnitIcon>().Unit = unit.transform;
            unitIcons.Add(unitIcon);
        }
    }

    void DestroyUnitIcons() {
        foreach (GameObject unitIcon in unitIcons) {
            Destroy(unitIcon);
        }
        unitIcons.Clear();
        Destroy(unitIconContainer);
        unitIconContainer = null;
    }
}
