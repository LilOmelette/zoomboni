using UnityEngine;

public class Brake : Slide
{

    [SerializeField] private State stateSlide;
    [SerializeField] private State stateSit;

    [SerializeField] private ParticleSystem fx;

    [SerializeField] private float MINIMUM_SPEED_BEFORE_SLIDE;

    public override void Enter(Component statePrior)
    {
        base.Enter(statePrior);
        fx.Play();
    }

    public override void Exit()
    {
        base.Exit();
        fx.Stop();
    }

    public override void TransitionCheck()
    {

        if (!CheckGround())
        {
            stateMachine.Change(stateAirborne);
        }

        if (!player.inputSlide.IsPressed())
        {
            if (player.cc.velocity.magnitude < MINIMUM_SPEED_BEFORE_SLIDE)
            {
                stateMachine.Change(stateSit);
            }
            else
            {
                stateMachine.Change(stateSlide);
            }
        }
    }

}