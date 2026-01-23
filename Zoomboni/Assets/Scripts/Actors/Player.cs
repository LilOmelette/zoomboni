using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class Player : Character
{

    [Header("Stats")]
    [SerializeField] internal float GRAVITY = -40.0f;
    [SerializeField] internal float GROUND_HEIGHT = 0.65f;

    [SerializeField] internal float GROUND_RANGE = 0.85f;
    [SerializeField] internal float DISTANCE_TO_FLOOR = 0.75f;

    [Header("Misc")]
    [SerializeField] internal float DEAD_ZONE;
    [SerializeField] internal float ANGLE_MAX;
    [SerializeField] internal Camera cam;
    [SerializeField] internal PlayerInput playerInput;

    internal InputAction
        inputMove,
        inputSlide
        ;

    [SerializeField] internal GameObject containerForModel;
    [SerializeField] internal GameObject model;

    [SerializeField] internal StateMachine stateMachine;
    [SerializeField] internal List<State> statesGrounded;
    [SerializeField] internal State stateBounce;

    internal Vector3 directionContainerForModel = new Vector3();
    internal Vector3 velocityFromForce = new Vector3();
    internal Vector3 directionMovement = new Vector3();

    internal const float OOB_FLOOR = -500.0f;
    internal Material mat_last_touched;
    internal bool focusing = false;
    internal Quaternion baseModelRotation;

    private int score = 0;
    public TextMeshProUGUI scoreText;

    void Start(){
        scoreText.text = "Score: " + score;
    }

    private new void Awake()
    {
        base.Awake();
        inputMove = playerInput.actions.FindAction("Move");
        inputSlide = playerInput.actions.FindAction("Slide");
        baseModelRotation = model.transform.rotation;
    }

    private void Update()
    {
        UpdateModel();
        UpdateInputs();
    }

    private void UpdateModel()
    {
        Vector2 inputMovement = GetInputMovement();
        Vector3 normalRotation = Vector3.up;
        Vector3 crossVector = Vector3.Cross(directionContainerForModel, normalRotation);
        if (crossVector.magnitude > 0)
        {
            float lerpRotation = stateMachine.GetState().GetLerpRotation();
            Quaternion rotationTarget = Quaternion.LookRotation(crossVector, normalRotation) * Quaternion.Euler(0, 90, 0);
            containerForModel.transform.rotation = Quaternion.Lerp(containerForModel.transform.rotation, rotationTarget, lerpRotation * Time.deltaTime);
        }

        //containerForModel.transform.up = Vector3.down;
    }

    private void UpdateInputs()
    {
        Vector2 inputMovement = GetInputMovement();

        Vector3 fVector = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        fVector.Normalize();
        Vector3 fVector2 = new Vector3(cam.transform.right.x, 0, cam.transform.right.z);
        fVector2.Normalize();

        directionMovement = new Vector3();
        directionMovement += fVector * inputMovement.y;
        directionMovement += fVector2 * inputMovement.x;

    }

    internal RaycastHit GetHitBeneath()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(
            ray,
            out RaycastHit hitFloor,
            1.0f,
            1 << LayerMask.NameToLayer("Default"),
            queryTriggerInteraction: QueryTriggerInteraction.Ignore
        );
        return hitFloor;
    }

    internal Vector3 GetVelocity()
    {
        return cc.velocity;
    }


    internal void UpdateContainerForModelRotation()
    {
        if (directionMovement.magnitude > DEAD_ZONE)
        {
            directionContainerForModel = directionMovement;
        }
    }

    internal void UpdateContainerForModelRotation(float t)
    {
        if (directionMovement.magnitude > DEAD_ZONE)
        {
            directionContainerForModel = Vector3.Lerp(directionContainerForModel, directionMovement, t);
        }
    }

    internal void SetScale(float squash = 0)
    {
        Vector3 scaleTarget = new Vector3(1.0f + squash / 2.0f, 1.0f - squash, 1.0f + squash / 2.0f);
        model.transform.localScale = scaleTarget;
    }

    public void SetForce(Vector3 force)
    {
        velocityFromForce += force;
    }

    public void LateUpdate()
    {
        velocityFromForce = new Vector3();
    }

    public Vector3 GetFacing()
    {
        return containerForModel.transform.forward;
    }

    internal void Teleport(Transform target)
    {
        cc.enabled = false;
        transform.position = target.position;
        directionContainerForModel = target.forward;
        cc.enabled = true;
        cc.Move(new Vector3()); 

        stateMachine.OnReset();
    }

    internal void SetPosition(Vector3 position)
    {
        cc.enabled = false;
        transform.position = position;
        cc.enabled = true;
    }

    internal bool InGroundState()
    {
        return statesGrounded.Contains(stateMachine.GetState());
    }

    internal bool IsGroundState(State state)
    {
        return statesGrounded.Contains(state);
    }

    internal Vector3 GetInputMovement()
    {
        Vector3 inputMovement = new Vector3(inputMove.ReadValue<Vector2>().x, inputMove.ReadValue<Vector2>().y);
        if (Mathf.Abs(inputMovement.x) < DEAD_ZONE)
        {
            inputMovement.x = 0;
        }
        if (Mathf.Abs(inputMovement.y) < DEAD_ZONE)
        {
            inputMovement.y = 0;
        }
        return inputMovement;
    }

    internal bool IsFocusing()
    {
        return focusing;
    }

    public override bool IsGrounded()
    {
        return InGroundState();
    }

    internal void SignalBounce()
    {
        if (stateMachine.GetState() != stateBounce)
        {
            stateMachine.Change(stateBounce); 
        }
        
    }
    public void AddPoints(int points)
    {
        score = score + points;
        scoreText.text = "Score: " + score;
        Debug.Log(score);
    }

}
