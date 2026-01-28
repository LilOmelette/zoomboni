using UnityEngine;

public class Airborne : State
{

    [SerializeField] private AudioSource sfxLoop;

    [SerializeField] private Timer timerVolume;

    [SerializeField] private State stateLand;
    [SerializeField] private State stateWallKick;

    [SerializeField] private float GRAVITY = 1.0f;
    [SerializeField] private float ACCELERATION = 1.0f;
    [SerializeField] private float FRICTION = 1.0f;

    [SerializeField] private float PITCH_MIN = 0.5f;
    [SerializeField] private float PITCH_MAX = 2.0f;
    [SerializeField] private float PITCH_RANGE = 8f;

    [SerializeField] private float MODEL_TURN_SPEED = 4.0f;

    [SerializeField] private string animNeutral, animLeft, animRight;

    private Vector3 velocityPrior = new Vector3(), deltaVelocity = new Vector3();
    public override void Enter(Component statePrior)
    {
        sfxLoop.Play();
        timerVolume.Reset();
    }

    public override void Exit()
    {
        sfxLoop.Stop();
        player.SetScale();
    }

    private readonly float modYVelocitySquash = 1.0f / 96.0f;
    public override void GraphicsUpdate()
    {
        player.UpdateContainerForModelRotation(MODEL_TURN_SPEED * Time.deltaTime);
        float squash = -Mathf.Abs(player.cc.velocity.y * modYVelocitySquash);
        player.SetScale(squash);

        float t;
        if (player.cc.velocity.y > 0)
        {
            t = (PITCH_RANGE - player.cc.velocity.y) / PITCH_RANGE;
        }
        else
        {
            t = (PITCH_RANGE + player.cc.velocity.y) / PITCH_RANGE;
        }

        float pitch = Mathf.Lerp(PITCH_MIN, PITCH_MAX, t);
        sfxLoop.pitch = pitch;
        sfxLoop.volume = Mathf.Clamp(timerVolume.GetPercent(), 0, 1) / 2.0f;


        Vector3 inputMovement = player.GetInputMovement();
        if (inputMovement.x == 0)
        {
            player.SetAnimation(animNeutral);
        }
        else if (Mathf.Sign(inputMovement.x) == -1)
        {
            player.SetAnimation(animLeft);
        }
        else if (Mathf.Sign(inputMovement.x) == 1)
        {
            player.SetAnimation(animRight);
        }
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        velocity = ApplyGravity(velocity, GRAVITY);
        velocity = ApplyAcc(velocity, ACCELERATION);
        velocity = ApplyFriction(velocity, FRICTION);

        player.cc.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        deltaVelocity = player.cc.velocity - velocityPrior;
        velocityPrior = player.cc.velocity;
    }

    public override void TransitionCheck()
    {
        bool isPlayerCloseToGround = CheckGround();

        if (isPlayerCloseToGround)
        {
            stateMachine.Change(stateLand);
        }

        if (CanWallKick() && player.inputKick.WasPressedThisFrame())
        {
            stateMachine.Change(stateWallKick);
        }
    }
}
