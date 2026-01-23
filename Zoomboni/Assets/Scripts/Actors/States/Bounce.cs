using UnityEngine;

public class Bounce : State {

    [SerializeField] private State stateFall;
    [SerializeField] private Timer duration;

    [SerializeField] private float ACC;
    [SerializeField] private float FRIC;
    [SerializeField] private float GRAV;
    [SerializeField] private float BOUNCE_INIT;
    [SerializeField] private float BOUNCE_XZ_INIT;
    [SerializeField] private bool FIXED_BOUNCE_XZ_INIT = false;
    [SerializeField] private string anim;
    
    [SerializeField] private ParticleSystem fxStart;
    [SerializeField] private ParticleSystem fxTrailOptional;
    
    [SerializeField] private AudioSource sfxEnterOptional;
    [SerializeField] private AudioSource sfxLoopOptional;

    private bool init = false;

    public override void Enter(Component arg) {
        duration.Reset();

        fxStart.transform.position = player.transform.position;
        fxStart.Play();

        if (fxTrailOptional) fxTrailOptional.Play();
        if (sfxEnterOptional) sfxEnterOptional.Play();
        if (sfxLoopOptional) sfxLoopOptional.Play();
        init = true;
    }

    public override void Exit() {
        if (fxTrailOptional) fxTrailOptional.Stop();
        if (sfxLoopOptional) sfxLoopOptional.Stop();
    }

    public override void GraphicsUpdate() {
        player.SetAnimation(anim, true);
    }

    public override void PhysicsUpdate() {
        Vector3 velocity = player.cc.velocity;

        if (init) {
            Vector3 velocityBounceInit = player.containerForModel.transform.forward * BOUNCE_XZ_INIT;
            if (FIXED_BOUNCE_XZ_INIT) {
                velocity = velocityBounceInit;
            }
            else {
                velocity = velocity + velocityBounceInit;
            }
            velocity.y = BOUNCE_INIT;
            init = false;
        }
        else {
            velocity = ApplyAcc(velocity, ACC);
            velocity = ApplyFriction(velocity, FRIC);
            velocity = ApplyGravity(velocity, GRAV);
            velocity = ApplyForce(velocity);
        }

        player.cc.Move(velocity * Time.deltaTime);
    }

    public override void TransitionCheck() {
        if (!duration.IsActive()) {
            stateMachine.Change(stateFall);
        }
    }

}