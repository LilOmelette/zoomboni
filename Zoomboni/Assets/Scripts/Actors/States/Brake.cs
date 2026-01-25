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
        player.SetScale();
        player.model.transform.localPosition = new Vector3(0, 0, 0);
    }

    public override void GraphicsUpdate()
    {
        base.GraphicsUpdate();
        if (timerCharge.JustDeactivated())
        {
            fx.Stop();
            fxTurbo.Play();
        }

        float t = Mathf.Clamp(timerCharge.GetPercent(), 0, 1);
        player.SetScale(0 + (0.6f * t));
        player.model.transform.localPosition = new Vector3(0, -0.25f, 0);
    }

    public override void TransitionCheck()
    {
        if (!HoldingMove())
        {
            timerCharge.Reset();
        }

        if (!CheckGround())
        {
            stateMachine.Change(stateAirborne);
        }

        if (!player.inputSlide.IsPressed())
        {
            if (
                !HoldingMove() ||
                (timerCharge.IsActive() && (player.cc.velocity.magnitude < MINIMUM_SPEED_BEFORE_SLIDE))
                )
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