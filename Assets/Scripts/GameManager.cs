using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    UnityStandardAssets.Cameras.FreeLookCam cameraRig;
    Transform mainCamera;

    [SerializeField] Unit _activeUnit;
    public Unit ActiveUnit {
        get {
            return _activeUnit;
        }
        set {
            if (_activeUnit != null) {
                _activeUnit.gameObject.GetComponent<ThirdPersonController>().enabled = false;
                _activeUnit.StopMovement();
                _activeUnit.gameObject.tag = "FriendlyUnit";
            }
            _activeUnit = value;
            if (_activeUnit != null) {
                Debug.Log(_activeUnit);
                _activeUnit.gameObject.GetComponent<ThirdPersonController>().enabled = true;
                cameraRig.SetTarget(_activeUnit.transform);
                MovementBar.ActivateBar();
                _activeUnit.gameObject.tag = "Player";
            }
            else {
                cameraRig.SetTarget(instance.transform);
                MovementBar.DeactivateBar();
            }
        }
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        mainCamera = Camera.main.transform;
        cameraRig = mainCamera.root.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
    }

    void Start() {
        ActiveUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
    }
}
