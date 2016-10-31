using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Unit))]
public class ThirdPersonController : MonoBehaviour {
    public static Unit ActiveUnit {
        get {
            return _activeUnit;
        }
        set {
            Unit previousActiveUnit = _activeUnit;

            // handle cleanup for previous active unit
            if (_activeUnit != null) {
                _activeUnit.gameObject.GetComponent<ThirdPersonController>().enabled = false;
                _activeUnit.StopMovement();
                _activeUnit.gameObject.tag = "FriendlyUnit";
            }

            _activeUnit = value;
            if (_activeUnit != null) {
                _activeUnit.gameObject.GetComponent<ThirdPersonController>().enabled = true;
                CameraManager.Instance.Target = _activeUnit.transform;
                _activeUnit.gameObject.tag = "Player";
            }
            else {
                CameraManager.Instance.Target = GameManager.Instance.transform;
            }

            if (activeUnitChangedEvent != null)
                activeUnitChangedEvent(_activeUnit, previousActiveUnit);
        }
    }
    public delegate void ActiveUnitChangedDelegate(Unit activeUnit, Unit previousActiveUnit);
    public static event ActiveUnitChangedDelegate activeUnitChangedEvent;

    Rigidbody rigidBody;
    new CapsuleCollider collider;
    Animator animator;
//    static UnityStandardAssets.Cameras.FreeLookCam cameraRig;
//    static Transform cam;
    Transform lookTarget;
    Unit unit;

    float horizontalInput;
    float verticalInput;
    bool looking = false;
    bool walking = false;
    Vector3 previousTargetDirection = new Vector3();
//    static GameObject movementBar;

    static Unit _activeUnit;

//    void Awake() {
//        if (enabled) {
//            cameraRig = GameManager.freeLookCamRig;
//            cam = cameraRig.gameObject.GetComponentInChildren<Camera>().transform;
//        }
//    }

    void Start() {
        lookTarget = GameObject.FindGameObjectWithTag("LookTarget").transform;
        unit = GetComponent<Unit>();
//        if (movementBar == null) {
//            movementBar = GameObject.FindObj ////////// TODO
//        }
    }

    void Update() {
        GetInput();
    }

    float WrapAnglePlusMinus180(float a) {
        // given an angle in range [-360, 360], returns the equivalent angle in [-180, 180]
        if (a < -180f) {
            return a + 360f;
        }
        else if (a > 180f) {
            return a - 360f;
        }
        else {
            return a;
        }
    }

    Vector3 GetTargetDirection(Vector3 camForwardInGroundPlane) {
        return (CameraManager.Instance.Camera.transform.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
    }

    Vector3 GetTargetDirection() {
        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(CameraManager.Instance.Camera.transform.forward, Vector3.up).normalized;
        return (CameraManager.Instance.Camera.transform.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
    }

    void FixedUpdate() {
        HandleMovement();
    }

    void GetInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        walking = Input.GetButton("Walk");

        if (Input.GetButtonDown("Aim")) {
            CameraManager.Instance.Target = lookTarget;
            looking = true;
        }
        else if (Input.GetButtonUp("Aim")) {
            CameraManager.Instance.Target = transform;
            looking = false;
        }

        if (Input.GetButtonDown("NextUnit")) {
            List<Unit> friendlyUnits = Unit.FriendlyUnits;
            int indexJump = Input.GetButton("PrevUnitModifier") ? (friendlyUnits.Count - 1) : 1;
            int nextUnitIndex = (indexJump + friendlyUnits.IndexOf(unit)) % friendlyUnits.Count;
//            Debug.Log(nextUnitIndex);
            Unit nextUnit = friendlyUnits[nextUnitIndex];
            ThirdPersonController.ActiveUnit = nextUnit;
        }

    }

    void HandleMovement() {
        // calculate direction  of "forward" movement
        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(CameraManager.Instance.Camera.transform.forward, Vector3.up).normalized;
        Vector3 targetDirection;

        /*
        if the player begins "looking" while running, they will continue moving in
        the same direction until they either stop moving or stop looking
        */
        if (!looking || previousTargetDirection == Vector3.zero)
            targetDirection = GetTargetDirection(camForwardInGroundPlane);
        else
            targetDirection = (previousTargetDirection * new Vector2(horizontalInput, verticalInput).magnitude).normalized;
        // store direction to allow re-use when "looking"
        previousTargetDirection = targetDirection;

        // place "look" target in camera's forward direction from player's position
        lookTarget.position = transform.position + camForwardInGroundPlane.normalized;

        // calculate rotation required to achieve desired target direction
        Quaternion targetRotation;
        if (horizontalInput != 0 || verticalInput != 0) {
            targetRotation = Quaternion.LookRotation(targetDirection);
        }
        else {
            targetRotation = transform.rotation;
        }

        Unit.MovementType movementType = walking ? Unit.MovementType.Walking : Unit.MovementType.Running;
        unit.HandleMovement(targetDirection, targetRotation, movementType);
    }
}