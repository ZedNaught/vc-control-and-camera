using UnityEngine;

public class ThirdPersonController : MonoBehaviour {

    Rigidbody rigidBody;
    CapsuleCollider capCol;
    Animator anim;
    UnityStandardAssets.Cameras.FreeLookCam cameraRig;
    Transform lookTarget;

//    [SerializeField]
    PhysicMaterial zeroFriction;
    //Zero Friction Physic Material (in 2D it's called Physics) we want zero friction when we move

//    [SerializeField]
    PhysicMaterial maxFriction;
    //Max Friction Physic Material (in 2D it's called Physics) but maximum friction when we are sationary

    Transform cam;

//    [SerializeField]
    float walkSpeed = 7f;
    float runSpeed = 20f;
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
        rigidBody = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        cameraRig = cam.root.GetComponent<UnityStandardAssets.Cameras.FreeLookCam>();
        capCol = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        onGround = true;    //The player starts out on the ground
        lookTarget = GameObject.FindGameObjectWithTag("LookTarget").transform;
        previousPosition = transform.position;
        currentMovementPoints = maxMovementPoints;

        InitializePhysicMaterials();
    }

    void InitializePhysicMaterials() {
        maxFriction = new PhysicMaterial();
        maxFriction.dynamicFriction = 1f;
        maxFriction.staticFriction = 1f;
        maxFriction.bounciness = 0f;
        maxFriction.frictionCombine = PhysicMaterialCombine.Maximum;
        maxFriction.bounceCombine = PhysicMaterialCombine.Average;

        zeroFriction = new PhysicMaterial();
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
        HandleUpdateInput();
//        Input.GetButton("Aim").
        HandleFriction();
    }

    void HandleUpdateInput() {
        if (Input.GetButtonDown("Aim")) {
            cameraRig.SetTarget(lookTarget);
            looking = true;
//            cam.localPosition += aimCameraPositionDelta;
        }
        else if (Input.GetButtonUp("Aim")) {
            cameraRig.SetTarget(transform);
            looking = false;
//            cam.localPosition -= aimCameraPositionDelta;
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
        //Inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        Vector3 camForwardInGroundPlane = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
        Vector3 targetDirection;
        if (!looking || previousTargetDirection == Vector3.zero)
            targetDirection = GetTargetDirection(camForwardInGroundPlane);
        else
            targetDirection = (previousTargetDirection * new Vector2(horizontalInput, verticalInput).magnitude).normalized;
//        jumpInput = Input.GetButtonDown("Jump");
        // place "look" target in camera's forward direction from player's position
        lookTarget.position = transform.position + camForwardInGroundPlane.normalized;
        

        /*
            This if statment is how tolerant we are on changing the direction based on where the camera is looking.
            For example, if the player is moving to the left/right of where the camera is looking and then he rotates the camera
            so it looks towards where he is going, we will keep moving at the same direction as before
        */
//        storDir = cam.right;        //This means, the player can keep moving in the same direction they were before even if they change the camera angle

        // handle forward movement and jumping, does not rotate
        if (onGround) { 
//            Vector3 targ
            float currentSpeed = currentMovementPoints > 0 ? (Input.GetButton("Walk") ? walkSpeed : runSpeed) : 0f;
            if (currentSpeed > 0) {
                rigidBody.AddForce(targetDirection * currentSpeed);
            }
            anim.SetFloat("Speed", rigidBody.velocity.magnitude);

            //Jump controls
//            if (jumpInput && onGround)
//            {
//                rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);      //ForceMode.Impulse (I think) gives all the force to the jump for only one frame.
//            }
        }

        // rotate appropriately if the player has given input
        if (horizontalInput != 0 || verticalInput != 0) {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            // find the angle (in range [-180, 180]) between the character's rotation and where the camera is looking
            float targetRotationAngle = WrapAnglePlusMinus180(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);

            float turnAnimationMagnitude;  // holds value to send to animator for "turning" animation
            // if angle is not zero (to avoid a warning), look towards the camera
            float animationDirection = Mathf.Clamp(targetRotationAngle, -45f, 45f) / 45f;
            if (targetRotationAngle != 0) {
                rigidBody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
                turnAnimationMagnitude = Mathf.LerpAngle(anim.GetFloat("Direction"), animationDirection, animDirectionChangeSpeed * Time.fixedDeltaTime);
            }
            else {
                turnAnimationMagnitude = Mathf.LerpAngle(anim.GetFloat("Direction"), 0f, animDirectionChangeSpeed * Time.fixedDeltaTime);
            }
            anim.SetFloat("Direction", turnAnimationMagnitude);
        }

        previousTargetDirection = targetDirection;
        float amountMoved = (transform.position - previousPosition).magnitude;
        currentMovementPoints = Mathf.Max(0f, currentMovementPoints - amountMoved);
        previousPosition = transform.position;
    }

    //If we are touching something (like the ground )
    void OnCollisionEnter(Collision other) {
        //This means we are on the ground

        if (other.gameObject.tag == "Ground") {
            onGround = true;
            rigidBody.drag = 5;
        }
    }

    //Once we are no longer touching the object we collided with earlier
    void OnCollisionExit(Collision other) {
        //We want to know when we have left the ground (or anything else)
        if (other.gameObject.tag == "Ground") {    //You can copy this if statement and make it "Vehicle" or something to jump off a car.
            onGround = false;
            rigidBody.drag = 0;
        }
    }


    void HandleFriction() {   //handles the friction physics for the character
        if (horizontalInput == 0 && verticalInput == 0) {
            //We are stationary so we want maximum friction.
            capCol.material = maxFriction;
        } else {
            //We are moving, don't cause friction
            capCol.material = zeroFriction;
        }
    }
}