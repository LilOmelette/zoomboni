using UnityEngine;

public class Waddle : State
{

    [SerializeField] private float ACCELERATION = 1.0f;
    [SerializeField] private float FRICTION = 1.0f;

    [SerializeField] private State stateSlide;
    [SerializeField] private State stateSit;
    [SerializeField] private State stateAirborne;

    public override void Enter(Component arg)
    {

        player.containerForModel.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public override void Exit()
    {

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
            stateMachine.Change(stateSlide, this);
        }

        if (!CheckGround())
        {
            stateMachine.Change(stateAirborne);
        }
    }

}