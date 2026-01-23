using UnityEngine;

public class Slide : State
{

    [SerializeField] internal State stateWaddle;
    [SerializeField] internal State stateAirborne;
    [SerializeField] internal State stateBrake;

    [SerializeField] private float START_POWER;
    [SerializeField] private float BOOST_MOD = 3.0f;
    [SerializeField] private float BOOST_EXP = 2.0f;

    [SerializeField] private float ACCELERATION = 10.0f;
    [SerializeField] private float FRICTION = 0.99f;
    [SerializeField] private float STICKY = -40.0f;
    [SerializeField] private float GRAV = -10.0f;

    [SerializeField] private float EXIT_SPEED = 4.0f;
    [SerializeField] private float MODEL_TURN_SPEED = 4.0f;

    [SerializeField] private float SLOPE_POWER;
    [SerializeField] private float SPEED_SLOPE_MIN = 8.0f;
    [SerializeField] private float SPEED_SLOPE_POW = 1.0f;
    [SerializeField] private float SPEED_SLOPE_MUL = 1.0f;
    [SerializeField] private float MOMENTUM_SLOPE_RESIST = 0.1f;
    [SerializeField] private float DISTANCE_CHECK_SLOPE = 1.7f;

    [SerializeField] private Timer timerLand;

    [SerializeField] protected AudioSource sfxStart;
    [SerializeField] protected AudioSource sfxLoop;

    private float startPower = -1;

    public override void Enter(Component statePrior)
    {
        player.SetAnimation("Armature|Slide");

        startPower = -1f;
        if (statePrior is Brake)
        {
            Brake brake = (Brake)statePrior;
            float chargeTime = BOOST_MOD * Mathf.Pow(brake.GetChargeTime(), BOOST_EXP);
            startPower = chargeTime;
        }
        else if (statePrior is Airborne)
        {
            startPower = 0;
        }
        else
        {
            startPower = 1;
        }

        if (startPower >= 1)
        {
            sfxStart.Play();
        }

        if (statePrior is Airborne)
        {
            if (player.cc.velocity.y < 0)
            {
                timerLand.Reset();
            }
            
        }
        else
        {

            timerLand.End();
        }

        sfxLoop.Play();
    }

    public override void Exit()
    {
        player.SetScale();
        timerLand.End();
        sfxLoop.Stop();
    }

    public override void GraphicsUpdate()
    {
        player.UpdateContainerForModelRotation(MODEL_TURN_SPEED * Time.deltaTime);
        Squash(timerLand);
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        velocity = ApplyAcc(velocity, ACCELERATION);
        velocity = ApplyFriction(velocity, FRICTION);

        RaycastHit hitGround = CheckSurface(new Vector3(0, player.cc.height/2, 0), Vector3.down, DISTANCE_CHECK_SLOPE);
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

        if (startPower > 0)
        {
            Vector3 startSpeed = player.containerForModel.transform.forward * START_POWER;
            startSpeed.y = -START_POWER;
            velocity += startPower * startSpeed;
            startPower = -1;

        }

        velocity = ApplyGravitySticky(velocity, GRAV, STICKY);

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        if (player.cc.velocity.magnitude < EXIT_SPEED)
        {
            stateMachine.Change(stateWaddle);
        }

        if (!CheckSlide())
        {
            stateMachine.Change(stateAirborne);
        }

        if (player.inputSlide.WasPressedThisFrame())
        {
            stateMachine.Change(stateBrake);
        }
    }

}