using UnityEngine;
using System.Collections;

public class OverviewCam : MonoBehaviour {
    public float moveSpeed = 1f;
    public float cameraHeight = 8f;
//    float acceleration = 1f;
    float horizontalInput;
    float verticalInput;
    GameObject unitIconContainer;
    [SerializeField] GameObject unitIconPrefab;
    [SerializeField] GameObject canvas;

    void Start() {
        unitIconContainer = new GameObject("Unit Icon Container");
        unitIconContainer.transform.parent = canvas.transform;
        DrawUnitIcons();
    }

    void Update () {
        GetInput();
	    MoveCamera();
	}

    void GetInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    void MoveCamera() {
        transform.position += moveSpeed * (new Vector3(horizontalInput, 0f, verticalInput));
    }

    void DrawUnitIcons() {
        foreach (Unit unit in Unit.allUnits) {
            GameObject unitIcon = Instantiate(unitIconPrefab, unit.transform.position, Quaternion.identity, unitIconContainer.transform) as GameObject;
            unitIcon.GetComponent<UnitIcon>().Unit = unit.transform;
        }
    }
}
