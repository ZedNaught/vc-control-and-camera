using UnityEngine;
using System.Collections;
using FreeLookCam = UnityStandardAssets.Cameras.FreeLookCam;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance {get; set;}
    public Transform Target {get; set;}
    public Camera Camera {get; set;}
    public enum Cameras {
        None, FreeLook, Overview
    }
    private Cameras currentCamera = Cameras.None;


    [SerializeField] private FreeLookCam freeLookCamRig;
    private Camera freeLookCam;
    [SerializeField] private OverviewCam overviewCamRig;
    private Camera overviewCam;

//    private static CameraManager instance;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }

//        freeLookCamRig = GameObject.FindGameObjectWithTag("FreeLookCamRig").GetComponent<FreeLookCam>();
        freeLookCam = freeLookCamRig.gameObject.GetComponentInChildren<Camera>();
//
//        overviewCamRig = GameObject.FindGameObjectWithTag("OverviewCamRig").GetComponent<OverviewCam>();
        overviewCam = overviewCamRig.gameObject.GetComponentInChildren<Camera>();
    }

    void Start() {
        SetCamera(Cameras.FreeLook);
        ThirdPersonController.activeUnitChangedEvent += OnActiveUnitChange;
    }

    public void SetCamera(Cameras whichCamera) {
        if (whichCamera == Cameras.FreeLook && currentCamera != Cameras.FreeLook) {
//            overviewCam.gameObject.SetActive(false);
            overviewCamRig.gameObject.SetActive(false);
            freeLookCamRig.gameObject.SetActive(true);
//            freeLookCam.gameObject.SetActive(true);
            Camera = freeLookCam;
            currentCamera = Cameras.FreeLook;
        }
        else if (whichCamera == Cameras.Overview && currentCamera != Cameras.Overview) {
//            freeLookCam.gameObject.SetActive(false);
            freeLookCamRig.gameObject.SetActive(false);
//            overviewCam.gameObject.SetActive(true);
            overviewCamRig.gameObject.SetActive(true);
            Camera = overviewCam;
            currentCamera = Cameras.Overview;
        }
    }

    public void ToggleCamera() {
        if (freeLookCam.gameObject.activeSelf) {
            SetCamera(Cameras.Overview);
        }
        else {
            SetCamera(Cameras.FreeLook);
        }
    }

    private void OnActiveUnitChange(Unit activeUnit, Unit previousActiveUnit) {
        if (currentCamera == Cameras.FreeLook) {
            freeLookCamRig.FindAndTargetPlayer();
        }
    }

//    public void SetTarget(Transform transform) {
//        
//    }
}
