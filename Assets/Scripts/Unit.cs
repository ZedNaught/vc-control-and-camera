using UnityEngine;

public class Unit : MonoBehaviour {

    Rigidbody _rigidbody;
    CapsuleCollider _collider;
    Animator anim;
//    UnityStandardAssets.Cameras.FreeLookCam cameraRig;
//    Transform lookTarget;

    [SerializeField]
    PhysicMaterial zeroFriction;
    //Zero Friction Physic Material (in 2D it's called Physics) we want zero friction when we move

    [SerializeField]
    PhysicMaterial maxFriction;
    //Max Friction Physic Material (in 2D it's called Physics) but maximum friction when we are sationary

    Transform cam;

//    [SerializeField]
    public float walkSpeed = 7f;
    public float runSpeed = 20f;
    public bool walking = false;
//    float movementScalar = 100f;
//    [SerializeField]
    public float maxMovementPoints = 100f;
    [HideInInspector]
    public float currentMovementPoints;
//    float currentSpeed;
    //How fast the player can move
    [SerializeField]
    float turnSpeed = 5f;
    float animDirectionChangeSpeed = 10f;
    //How fast the player can turn
//    [SerializeField]
//    float jumpPower = 5;

    /*What does SerializeField mean? It means you can edit these variables in the editor!*/

//    Vector3 directionPos;
    //The direction the player is facing
    Vector3 previousTargetDirection = new Vector3();
    Vector3 previousPosition;

    float horizontalInput;
    float verticalInput;
//    bool jumpInput;
    bool onGround;
    bool looking = false;

    void Start() {
        //Initialize references
        _rigidbody = GetComponent<Rigidbody>();
//        cam = Camera.main.transform;
//        cameraRig = cam.root.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        _collider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        onGround = true;    //The player starts out on the ground
//        lookTarget = GameObject.FindGameObjectWithTag("LookTarget").transform;
        previousPosition = transform.position;
        currentMovementPoints = maxMovementPoints;

        InitializePhysicMaterials();
    }

    void InitializePhysicMaterials() {
        maxFriction = new PhysicMaterial("Max Friction");
        maxFriction.dynamicFriction = 1f;
        maxFriction.staticFriction = 1f;
        maxFriction.bounciness = 0f;
        maxFriction.frictionCombine = PhysicMaterialCombine.Maximum;
        maxFriction.bounceCombine = PhysicMaterialCombine.Average;

        zeroFriction = new PhysicMaterial("Zero Friction");
        zeroFriction.dynamicFriction = 0f;
        zeroFriction.staticFriction = 0f;
        zeroFriction.bounciness = 0f;
        zeroFriction.frictionCombine = PhysicMaterialCombine.Multiply;
        zeroFriction.bounceCombine = PhysicMaterialCombine.Average;
    }
//    void TestAngleTransformTime() {
//        int testIterations = 100000;
//
//        float mathStart = Time.realtimeSinceStartup;
//        for (int i = 0; i < testIterations; i++) {
//            float a = UnityEngine.Random.Range(-360f, 360f);
//            float mathTransformed = TransformAngleMath(a);
//        }
//        float mathTime = Time.realtimeSinceStartup - mathStart;
//
//        float condStart = Time.realtimeSinceStartup;
//        for (int i = 0; i < testIterations; i++) {
//            float a = UnityEngine.Random.Range(-360f, 360f);
//            float conditionTransformed = TransformAngleConditions(a);
//        }
//        float condTime = Time.realtimeSinceStartup - condStart;
//
//        bool pointless = true;
//    }

    void Update() {
//        HandleUpdateInput();
//        Input.GetButton("Aim").
        HandleFriction();
    }

//    void HandleUpdateInput() {
//        if (Input.GetButtonDown("Aim")) {
//            cameraRig.SetTarget(lookTarget);
//            looking = true;
////            cam.localPosition += aimCameraPositionDelta;
//        }
//        else if (Input.GetButtonUp("Aim")) {
//            cameraRig.SetTarget(transform);
//            looking = false;
////            cam.localPosition -= aimCameraPositionDelta;
//        }
//    }

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

//    Vector3 GetTargetDirection(Vector3 camForwardInGroundPlane) {
//        return (cam.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
//    }
//
//    Vector3 GetTargetDirection() {
//        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
//        return (cam.right * horizontalInput + camForwardInGroundPlane * verticalInput).normalized;
//    }

    public void HandleMovement(Vector3 targetDirection, Quaternion targetRotation) {
        // assumes called from FixedUpdate

        // handle forward movement and jumping, does not rotate
        if (onGround) { 
            float currentSpeed = currentMovementPoints > 0f ? (walking ? walkSpeed : runSpeed) : 0f;
            if (currentSpeed > 0) {
                _rigidbody.AddForce(targetDirection * currentSpeed);
            }
            anim.SetFloat("Speed", _rigidbody.velocity.magnitude);
        }

        // rotate appropriately if the player has given input
//        if (targetRotation) {
            // find the angle (in range [-180, 180]) between the character's rotation and where the camera is looking

        float targetRotationAngle = WrapAnglePlusMinus180(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
        float turnAnimationMagnitude;  // holds value to send to animator for "turning" animation
        // if angle is not zero (to avoid a warning), look towards the camera
        float animationDirection = Mathf.Clamp(targetRotationAngle, -45f, 45f) / 45f;
        if (targetRotationAngle != 0) {
            _rigidbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            turnAnimationMagnitude = Mathf.LerpAngle(anim.GetFloat("Direction"), animationDirection, animDirectionChangeSpeed * Time.fixedDeltaTime);
        }
        else {
            turnAnimationMagnitude = Mathf.LerpAngle(anim.GetFloat("Direction"), 0f, animDirectionChangeSpeed * Time.fixedDeltaTime);
        }
        anim.SetFloat("Direction", turnAnimationMagnitude);
//        }

//        previousTargetDirection = targetDirection;
        float amountMoved = (transform.position - previousPosition).magnitude;
        UseMovementPoints(amountMoved);
        previousPosition = transform.position;
    }

    void UseMovementPoints(float amountMoved) {
        currentMovementPoints = Mathf.Max(0f, currentMovementPoints - amountMoved);

    }

    //If we are touching something (like the ground )
    void OnCollisionEnter(Collision other) {
        //This means we are on the ground

        if (other.gameObject.tag == "Ground") {
            onGround = true;
            _rigidbody.drag = 5;
        }
    }

    //Once we are no longer touching the object we collided with earlier
    void OnCollisionExit(Collision other) {
        //We want to know when we have left the ground (or anything else)
        if (other.gameObject.tag == "Ground") {    //You can copy this if statement and make it "Vehicle" or something to jump off a car.
            onGround = false;
            _rigidbody.drag = 0;
        }
    }


    void HandleFriction() {   //handles the friction physics for the character
        if (horizontalInput == 0 && verticalInput == 0) {
            //We are stationary so we want maximum friction.
            _collider.material = maxFriction;
        } else {
            //We are moving, don't cause friction
            _collider.material = zeroFriction;
        }
    }
}