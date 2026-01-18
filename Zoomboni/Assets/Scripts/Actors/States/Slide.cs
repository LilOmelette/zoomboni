using UnityEngine;

public class Slide : State
{

    [SerializeField] private State stateWaddle;
    [SerializeField] private State stateAirborne;

    [SerializeField] private float START_POWER;

    [SerializeField] private float ACCELERATION = 1.0f;
    [SerializeField] private float FRICTION = 1.0f;

    [SerializeField] private float MINIMUM_SPEED = 1.0f;

    [SerializeField] private float STICKY;
    [SerializeField] private float GRAV;

    [SerializeField] private float SLOPE_POWER;
    [SerializeField] private float SPEED_SLOPE_MIN = 8.0f;
    [SerializeField] private float SPEED_SLOPE_POW = 1.0f;
    [SerializeField] private float SPEED_SLOPE_MUL = 1.0f;
    [SerializeField] private float MOMENTUM_SLOPE_RESIST = 0.1f;
    [SerializeField] private float DISTANCE_CHECK_SLOPE = 1.7f;
    [SerializeField] private float DISTANCE_APPLY_STICKY;

    private bool shouldIApplyStartPower = false;
    private bool applySticky = false;

    public override void Enter(Component arg)
    {
        player.containerForModel.transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);

        shouldIApplyStartPower = !(arg is Airborne);
    }

    public override void Exit()
    {

    }

    public override void GraphicsUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        velocity = ApplyAcc(velocity, ACCELERATION);
        velocity = ApplyFriction(velocity, FRICTION);

        RaycastHit hitGround = CheckSurface(new Vector3(0, 0.65f, 0), Vector3.down, DISTANCE_CHECK_SLOPE);
        if (hitGround.collider)
        {
            Vector3 normalGround = hitGround.normal;
            Vector3 groundParallel = Vector3.Cross(transform.up, normalGround);
            Vector3 slopeParallel = Vector3.Cross(groundParallel, normalGround);
            float powSlope = 1.0f - Mathf.Clamp((velocity.magnitude - SPEED_SLOPE_MIN) * SPEED_SLOPE_MUL, 0, SPEED_SLOPE_POW);
            float powSlopeLimiter = Mathf.Clamp(1 - (MOMENTUM_SLOPE_RESIST * velocity.magnitude), 0, 1);

            velocity += powSlope * slopeParallel * SLOPE_POWER * powSlopeLimiter;
        }
        velocity = ApplyForce(velocity);

        if (shouldIApplyStartPower)
        {
            Vector3 startPower = player.containerForModel.transform.forward * START_POWER;
            velocity += startPower;
            shouldIApplyStartPower = false;
        }

        if (applySticky)
        {
            velocity.y = STICKY;
            applySticky = false;
        }
        else
        {
            velocity = ApplyGravity(velocity, GRAV);
        }

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        if (player.cc.velocity.magnitude < MINIMUM_SPEED)
        {
            stateMachine.Change(stateWaddle);
        }

        if (!CheckGround())
        {
            if (CheckGround(distanceToFloor: DISTANCE_APPLY_STICKY))
            {
                applySticky = true;
            }
            else
            {
                stateMachine.Change(stateAirborne);
            }
            
        }
    }

}