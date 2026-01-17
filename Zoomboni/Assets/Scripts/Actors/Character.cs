using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Actor {

    [SerializeField] private float ANIMATION_CROSSFADE;
    [SerializeField] internal Animator animator;

    internal CharacterController cc;

    public void Awake() {
        cc = GetComponent<CharacterController>();
    }
    
    private string anim_prev;

    internal void SetAnimation(string anim, bool crossfade=false) {
        if (!animator) {
            return;
        }
        if (anim != anim_prev) {
            if (crossfade) {
                animator.CrossFade(anim, ANIMATION_CROSSFADE * Time.deltaTime);
            }
            else {
                animator.Play(anim);
            }
            anim_prev = anim;
        }
    }


    internal void SetAnimationSpeed(float speed = 1.0f) {
        if (!animator) {
            return;
        }
        animator.speed = speed;
    }

    internal float GetAnimationTime() {
        if (!animator) {
            return 0;
        }
        else {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }

    public override bool IsGrounded() {
        return cc.isGrounded;
    }

    protected void MoveTo(Vector3 position) {
        if (!cc) {
            print(this.name + " is missing a character controller");
            return;
        }
        cc.enabled = false;
        transform.position = position;
        cc.enabled = true;
    }

}
