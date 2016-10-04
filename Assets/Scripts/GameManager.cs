using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
//    Transform mainCamera;
    public static UnityStandardAssets.Cameras.FreeLookCam freeLookCamRig;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

//        mainCamera = Camera.main.transform;
//        freeLookCamRig = GameObject.FindGameObjectWithTag("FreeLookCamRig").GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
    }

    void Start() {
//        ThirdPersonController.ActiveUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
    }
}
