using UnityEngine;

public class Sit : State
{
    public override void Enter(Component arg)
    {

    }

    public override void Exit()
    {

    }

    public override void GraphicsUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        owner.cc.Move(owner.transform.forward * Time.deltaTime);
    }

    public override void TransitionCheck()
    {

    }

}