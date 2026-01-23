using UnityEngine;

public class Brake : Slide
{

    [SerializeField] private State stateSlide;
    [SerializeField] private State stateSit;

    [SerializeField] private ParticleSystem fx;
    [SerializeField] private ParticleSystem fxTurbo;

    [SerializeField] private float MINIMUM_SPEED_BEFORE_SLIDE;

    [SerializeField] private Timer timerCharge;

    public override void Enter(Component statePrior)
    {
        sfxStart.Play();
        sfxLoop.Play();
        fx.Play();
        timerCharge.Reset();
    }

    public override void Exit()
    {
        base.Exit();
        fx.Stop();
        fxTurbo.Stop();
    }

    public override void GraphicsUpdate()
    {
        base.GraphicsUpdate();
        if (timerCharge.JustDeactivated())
        {
            fx.Stop();
            fxTurbo.Play();
        }
    }

    public override void TransitionCheck()
    {

        if (!CheckGround())
        {
            stateMachine.Change(stateAirborne);
        }

        if (!player.inputSlide.IsPressed())
        {
            if (!timerCharge.IsActive() && player.cc.velocity.magnitude < MINIMUM_SPEED_BEFORE_SLIDE)
            {
                stateMachine.Change(stateSit);
            }
            else
            {
                stateMachine.Change(stateSlide);
            }
        }
    }

    public float GetChargeTime()
    {
        float chargeTime = Mathf.Clamp(timerCharge.GetPercent(), 0, 1);
        return chargeTime;
    }

}