using UnityEngine;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
    public enum MovementType {Stopped, Walking, Running};

    Rigidbody _rigidbody;
    CapsuleCollider _collider;
    Animator anim;

    [SerializeField] PhysicMaterial zeroFriction;
    [SerializeField] PhysicMaterial maxFriction;

    [SerializeField] float walkSpeed = 7f;
    [SerializeField] float runSpeed = 20f;
    [SerializeField] float turnSpeed = 5f;
    float animDirectionChangeSpeed = 20f;
    MovementType currentMovementType = MovementType.Stopped;

    public float maxMovementPoints = 100f;
    [HideInInspector] public float currentMovementPoints;

    Vector3 previousPosition;
    float turnAnimationMagnitude;

    public static List<Unit> FriendlyUnits {
        get {
            List<Unit> friendlyUnits = new List<Unit>();
            foreach (Unit unit in allUnits) {
                if (unit.IsFriendly()) {
                    friendlyUnits.Add(unit);
                }
            }
            return friendlyUnits;
        }
    }
    public static List<Unit> allUnits = new List<Unit>();

    void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        previousPosition = transform.position;
        currentMovementPoints = maxMovementPoints;
    }
    void Start() {
        allUnits.Add(this);
        InitializePhysicMaterials();
    }

    void FixedUpdate() {
        
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

    void Update() {
        UpdateFriction();
        HandleAnimation();
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

    public void HandleMovement(Vector3 targetDirection, Quaternion targetRotation, MovementType movementType) {
        // forward movement - apply force to rigidbody in target direction
        float currentSpeed = currentMovementPoints > 0f ? (movementType == MovementType.Walking ? walkSpeed : runSpeed) : 0f;
        if (currentSpeed > 0) {
            _rigidbody.AddForce(targetDirection * currentSpeed);
            currentMovementType = movementType;
        }
        else {
            currentMovementType = MovementType.Stopped;
        }

        // rotational movement - Slerp between current rotation and target rotation
        float targetRotationAngle = WrapAnglePlusMinus180(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
        // map angle to range [-1, 1] to determine 
        float animationDirection = Mathf.Clamp(targetRotationAngle, -45f, 45f) / 45f;
        // holds value to send to animator for "turning" animation
        if (targetRotationAngle != 0) {
            _rigidbody.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            turnAnimationMagnitude = Mathf.LerpAngle(anim.GetFloat("Direction"), animationDirection, animDirectionChangeSpeed * Time.fixedDeltaTime);
        }

        // movement point accounting
        float amountMoved = (transform.position - previousPosition).magnitude;
        UseMovementPoints(amountMoved);
        previousPosition = transform.position;
    }

    void HandleAnimation() {
        anim.SetFloat("Speed", _rigidbody.velocity.magnitude);
        anim.SetFloat("Direction", turnAnimationMagnitude);
    }

    public void StopMovement() {
        _rigidbody.velocity = Vector3.zero;
        anim.SetFloat("Speed", 0f);
        anim.SetFloat("Direction", 0f);
    }

    void UseMovementPoints(float amountMoved) {
        currentMovementPoints = Mathf.Max(0f, currentMovementPoints - amountMoved);

    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Ground") {
            _rigidbody.drag = 5;
        }
    }

    void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Ground") {
            _rigidbody.drag = 0;
        }
    }

    void UpdateFriction() {
        if (currentMovementType == MovementType.Stopped) {
            _collider.material = maxFriction;
        } else {
            _collider.material = zeroFriction;
        }
    }

    bool IsFriendly() {
        return (tag == "FriendlyUnit" || tag == "Player");
    }
}