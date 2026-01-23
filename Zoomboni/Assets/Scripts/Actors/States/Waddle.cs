using UnityEngine;

public class Waddle : State
{

    [SerializeField] private float ACCELERATION = 1.0f;
    [SerializeField] private float FRICTION = 1.0f;
    [SerializeField] private AudioSource sfxWaddleLoop;

    [SerializeField] private State stateSlide;
    [SerializeField] private State stateSit;
    [SerializeField] private State stateAirborne;

    [SerializeField] private float STICKY;
    [SerializeField] private float GRAV;

    public override void Enter(Component statePrior)
    {
        player.SetAnimation("Armature|waddle");
        sfxWaddleLoop.Play();
    }

    public override void Exit()
    {
        sfxWaddleLoop.Stop();
    }

    public override void GraphicsUpdate()
    {
        player.UpdateContainerForModelRotation();
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        velocity = ApplyAcc(velocity, ACCELERATION);
        velocity = ApplyFriction(velocity, FRICTION);
        velocity = ApplyGravitySticky(velocity, GRAV, STICKY);

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        Vector3 inputMovement = player.GetInputMovement();
        if (inputMovement.magnitude <= DEAD_ZONE)
        {
            stateMachine.Change(stateSit);
        }

        if (player.inputSlide.WasPressedThisFrame())
        {
            stateMachine.Change(stateSlide);
        }

        if (!CheckSlide())
        {
            stateMachine.Change(stateAirborne);
        }
    }

}