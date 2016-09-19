using UnityEngine;

[RequireComponent(typeof(Unit))]
public class ThirdPersonController : MonoBehaviour {
    Rigidbody rigidBody;
    CapsuleCollider capCol;
    Animator anim;
    UnityStandardAssets.Cameras.FreeLookCam cameraRig;
    Transform lookTarget;
    Transform cam;
    Unit unit;

    float horizontalInput;
    float verticalInput;
    bool looking = false;
    bool walking = false;
    Vector3 previousTargetDirection = new Vector3();

    void Start() {
        cam = Camera.main.transform;
        cameraRig = cam.root.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        lookTarget = GameObject.FindGameObjectWithTag("LookTarget").transform;
        unit = GetComponent<Unit>();
    }

    void Update() {
        HandleUpdateInput();
    }

    void HandleUpdateInput() {
        if (Input.GetButtonDown("Aim")) {
            cameraRig.SetTarget(lookTarget);
            looking = true;
        }
        else if (Input.GetButtonUp("Aim")) {
            cameraRig.SetTarget(transform);
            looking = false;
        }

        if (Input.GetButtonDown("NextUnit")) {
            int indexJump = Input.GetButton("PrevUnitModifier") ? (Unit.friendlyUnits.Count - 1) : 1;
            int nextUnitIndex = (indexJump + Unit.friendlyUnits.IndexOf(unit)) % Unit.friendlyUnits.Count;
            Unit nextUnit = Unit.friendlyUnits[nextUnitIndex];
            GameManager.instance.ActiveUnit = nextUnit;
        }
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
        return (cam.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
    }

    Vector3 GetTargetDirection() {
        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        return (cam.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
    }

    void FixedUpdate() {
        HandleFixedUpdateInput();
        HandleMovement();
    }

    void HandleFixedUpdateInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        walking = Input.GetButton("Walk");
    }

    void HandleMovement() {
        // calculate direction  of "forward" movement
        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
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