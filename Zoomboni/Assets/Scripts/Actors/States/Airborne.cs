using UnityEngine;

public class Airborne : State
{

    [SerializeField] private AudioSource sfxLoop;

    [SerializeField] private State stateLand;

    [SerializeField] private float GRAVITY = 1.0f;
    [SerializeField] private float ACCELERATION = 1.0f;
    [SerializeField] private float FRICTION = 1.0f;
    [SerializeField] private float RADIUS_CHECK_WALL = 1.0f;
    public override void Enter(Component statePrior)
    {
        sfxLoop.Play();
    }

    public override void Exit()
    {
        sfxLoop.Stop();
        player.SetScale();
    }

    private readonly float modYVelocitySquash = 1.0f / 96.0f;
    public override void GraphicsUpdate()
    {

        float squash = -Mathf.Abs(player.cc.velocity.y * modYVelocitySquash);
        player.SetScale(squash);
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        //if (Physics.CheckSphere(player.transform.position, RADIUS_CHECK_WALL) && velocity.magnitude < 1)
        //{
        //    print("Push away push away");
        //    velocity -= player.directionContainerForModel;
        //}

        velocity = ApplyGravity(velocity, GRAVITY);
        velocity = ApplyAcc(velocity, ACCELERATION);
        velocity = ApplyFriction(velocity, FRICTION);


        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        bool isPlayerCloseToGround = CheckGround();

        if (isPlayerCloseToGround)
        {
            stateMachine.Change(stateLand, this);
        }
    }
}
