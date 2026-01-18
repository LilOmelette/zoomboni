using UnityEngine;

public class Sit : State
{

    [SerializeField] private State stateWaddle;
    [SerializeField] private State stateAirborne;
    [SerializeField] private State stateSlide;

    [SerializeField] private float FRICTION = 1.0f;

    public override void Enter(Component arg)
    {
        player.containerForModel.transform.localScale = new Vector3(0.9f, 1.1f, 0.9f);
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

        velocity = ApplyFriction(velocity, FRICTION);

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        Vector3 inputMovement = player.GetInputMovement();
        if (inputMovement.magnitude > DEAD_ZONE)
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