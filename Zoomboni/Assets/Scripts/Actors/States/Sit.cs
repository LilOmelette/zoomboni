using UnityEngine;

public class Sit : State
{

    [SerializeField] private State stateWaddle;
    [SerializeField] private State stateAirborne;
    [SerializeField] private State stateSlide;

    [SerializeField] private float FRICTION = 1.0f;

    [SerializeField] private float STICKY;
    [SerializeField] private float GRAV;

    [SerializeField] private Timer timerLand;

    public override void Enter(Component statePrior)
    {
        player.SetAnimation("Armature|Sit (Cooler)");
        //player.containerForModel.transform.localScale = new Vector3(1.1f, 0.9f, 0.9f);
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
    }

    public override void Exit()
    {

        player.SetScale();
        timerLand.End();
    }

    public override void GraphicsUpdate()
    {
        player.UpdateContainerForModelRotation();
        Squash(timerLand);
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = new Vector3(player.cc.velocity.x, player.cc.velocity.y, player.cc.velocity.z);

        velocity = ApplyFriction(velocity, FRICTION);
        velocity = ApplyGravitySticky(velocity, GRAV, STICKY);

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        Vector3 inputMovement = player.GetInputMovement();
        if (!timerLand.IsActive() && inputMovement.magnitude > DEAD_ZONE)
        {
            stateMachine.Change(stateWaddle);
        }

        if ( !CheckGround())
        {
            stateMachine.Change(stateAirborne);
        }

        if (player.inputSlide.WasPressedThisFrame())
        {
            stateMachine.Change(stateSlide);
        }
    }

}