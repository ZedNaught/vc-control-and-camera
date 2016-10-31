using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {get; set;}
//    Transform mainCamera;
//    public static UnityStandardAssets.Cameras.FreeLookCam freeLookCamRig;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }

//        mainCamera = Camera.main.transform;
//        freeLookCamRig = GameObject.FindGameObjectWithTag("FreeLookCamRig").GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
    }

    void Start() {
        EnterActionMode();
    }

    void Update() {
        // TEMPORARY // code to test switching between overview and free look cameras
        if (Input.GetKeyDown(KeyCode.O)) {
            EnterOverviewMode();
        }
        else if (Input.GetKeyDown(KeyCode.L)) {
            EnterActionMode();
        }
    }

    void EnterActionMode() {
        CameraManager.Instance.SetCamera(CameraManager.Cameras.FreeLook);
        if (!ThirdPersonController.ActiveUnit)
            ThirdPersonController.ActiveUnit = GameObject.FindGameObjectWithTag("FriendlyUnit").GetComponent<Unit>();
        ThirdPersonController.ActiveUnit.GetComponent<ThirdPersonController>().enabled = true;
        MovementBar.Instance.ActivateBar();
    }

    void EnterOverviewMode() {
        MovementBar.Instance.DeactivateBar();
        CameraManager.Instance.SetCamera(CameraManager.Cameras.Overview);
        FindObjectOfType<ThirdPersonController>().enabled = false;
    }
}
