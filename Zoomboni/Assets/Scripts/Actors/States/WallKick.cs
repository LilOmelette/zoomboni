using UnityEngine;

public class WallKick : State
{

    [SerializeField] private State stateExitGround;
    [SerializeField] private State stateExitAir;

    [SerializeField] private Timer duration;

    [SerializeField] private float kickStrength;
    [SerializeField] private string anim;

    [SerializeField] private ParticleSystem fxStart;
    [SerializeField] private ParticleSystem fxTrailOptional;

    [SerializeField] private AudioSource sfxEnterOptional;
    [SerializeField] private AudioSource sfxLoopOptional;

    public static float DISTANCE_TO_CHECK = 3.0f;

    public override void Enter(Component arg)
    {
        duration.Reset();

        fxStart.transform.position = player.transform.position; //create particle effect at player
        fxStart.Play();

        //if optional stuff exitsts, play it
        if (fxTrailOptional) fxTrailOptional.Play();
        if (sfxEnterOptional) sfxEnterOptional.Play();
        if (sfxLoopOptional) sfxLoopOptional.Play();
    }

    public override void Exit()
    {
        if (fxTrailOptional) fxTrailOptional.Stop();
        if (sfxLoopOptional) sfxLoopOptional.Stop();
    }

    public override void GraphicsUpdate()
    {
        player.SetAnimation(anim, true);
    }

    public override void PhysicsUpdate()
    {
        Vector3 velocity = player.cc.velocity;

        if (duration.JustDeactivated())
        {
            RaycastHit hit = CheckSurface(new Vector3(0, player.cc.height / 2, 0), player.GetFacing(), DISTANCE_TO_CHECK);
            if (hit.collider)
            {
                Vector3 normal = hit.normal;
                Vector3 velocityNew = velocity;
                velocityNew.y = 0;
                velocityNew *= velocity.magnitude/velocityNew.magnitude;
                Vector3 angle = Vector3.Reflect(velocityNew.normalized, normal.normalized);
                float power = kickStrength * velocityNew.magnitude;
                velocity = angle * power;
                player.FlipModelRotation(velocity);
            }

            player.cc.Move(velocity * Time.deltaTime);
        }
    }

    public override void TransitionCheck()
    {
        if (!duration.IsActive())
        {
            if (CheckGround())
            {
                stateMachine.Change(stateExitGround);
            }
            else
            {
                stateMachine.Change(stateExitAir);
            }
        }
    }

}
