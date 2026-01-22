using System;
using UnityEngine;

public abstract class State : MonoBehaviour
{

    protected static float DEAD_ZONE = 0.1f;

    public Player player;
    protected StateMachine stateMachine;
    protected bool applySticky = false;

    public void Awake() {
        stateMachine = GetComponentInParent<StateMachine>();
    }
    
    public bool IsObjectInRange(GameObject obj, float min, float max) {
        Vector3 differenceInPosition = player.transform.position - obj.transform.position;
        differenceInPosition.y = 0;
        float distance = differenceInPosition.magnitude;

        return min <= distance && max >= distance;
    }
    
    protected Vector3 GetMovementTowardsPosition(Vector3 position, float SPEED_MOVE, float GRAVITY = 0, float DISTANCE_MINIMUM = 0, bool ignoreVertical = false) {
        Vector3 diffPosition = position - player.transform.position;
        if (ignoreVertical) {
            diffPosition.y = 0;
        }

        Vector3 movement = new Vector3();
        movement.y += GRAVITY;

        if (diffPosition.magnitude >= DISTANCE_MINIMUM){
            movement += diffPosition.normalized * SPEED_MOVE;
        }
        return movement;
    }

    protected bool IsAnimationFinished(){
        if (!player.animator) {
            return true;
        }
        else {
            return player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !player.animator.IsInTransition(0);
        }
    }

    internal Vector3 ApplyAcc(Vector3 velocity, float ACC)
    {
        velocity += player.directionMovement * ACC * Time.deltaTime;
        return velocity;
    }

    internal Vector3 ApplyGravity(Vector3 velocity, float GRAV) {
        velocity.y += GRAV * Time.deltaTime;
        return velocity;
    }

    internal Vector3 ApplyGravitySticky(Vector3 velocity, float GRAV, float STICKY)
    {
        if (applySticky) {
            velocity.y = STICKY;
        }
        else
        {
            velocity = ApplyGravity(velocity, GRAV);
        }
        return velocity;
    }

    internal Vector3 ApplyFriction(Vector3 velocity, float FRIC) {
        float fricFrame = GetFrictionFrame(FRIC);

        velocity.x *= fricFrame;
        velocity.z *= fricFrame;
        return velocity;
    }

    internal float GetFrictionFrame(float FRIC) {
        float pow = Mathf.Clamp(60 * Time.deltaTime, 0.01f, 100.0f);
        float fricFrame = Mathf.Pow(FRIC, pow);
        return fricFrame;
    }

    internal bool CheckSlide()
    {
        float rangeCC = player.GROUND_RANGE * player.cc.radius;
        float heightCC = player.GROUND_HEIGHT * player.cc.height;
        float distanceCC = player.DISTANCE_TO_FLOOR * player.cc.height;

        float stuckInTheMiddleWithYou = CheckFloorHelper(
            new Vector3(0, heightCC, 0), Vector3.down,
            distanceCC, true);

        Vector3 frontFacing = Vector3.Cross(new Vector3(rangeCC, 0, rangeCC), player.GetFacing());
        frontFacing.y += heightCC;
        float heyDownInFront = CheckFloorHelper( frontFacing, Vector3.down,
                                                    distanceCC, true);

        Vector3 backFacing = Vector3.Cross(new Vector3(rangeCC, 0, rangeCC), -player.GetFacing());
        backFacing.y += heightCC;
        float itCameFromBehind = CheckFloorHelper( backFacing, Vector3.down,
                                                    distanceCC, true);

        float[] values;
        float[] smalues = { stuckInTheMiddleWithYou, heyDownInFront, itCameFromBehind };
        values = smalues;

        return Mathf.Max(values) > -100;
    }

    internal bool CheckGround(bool checkSlope = true, Vector3 mod = new Vector3()) {
        float rangeCC = player.GROUND_RANGE * player.cc.radius;
        float heightCC = player.GROUND_HEIGHT * player.cc.height;
        float distanceCC = player.DISTANCE_TO_FLOOR * player.cc.height;

        float a = CheckFloorHelper(
            new Vector3(rangeCC, heightCC, rangeCC) 
            + mod, Vector3.down, 
            distanceCC, checkSlope);

        float b = CheckFloorHelper(
            new Vector3(-rangeCC, heightCC, rangeCC)
            + mod, Vector3.down, 
            distanceCC, checkSlope);

        float c = CheckFloorHelper(
            new Vector3(rangeCC, heightCC, -rangeCC) 
            + mod, Vector3.down, 
            distanceCC, checkSlope);

        float d = CheckFloorHelper(
            new Vector3(-rangeCC, heightCC, -rangeCC) 
            + mod, Vector3.down, 
            distanceCC, checkSlope);

        float[] values;
        float[] smalues = { a,b,c,d };
        values = smalues;
        
        return Mathf.Max(values) > -100;
    }

    internal float CheckFloorHelper(Vector3 distanceFromTransform, Vector3 direction, float distance, bool checkSlope) {
        RaycastHit hit = CheckSurface(distanceFromTransform, direction, distance);
        const int none = -100;
        if (hit.collider == null) {
            return none;
        }
        else{
            // Floor is not too steep
            float ANGLE_MAX = 70.0f/90.0f;
            // TODO: THIS IS STUPID - only PlayerState should check with AngleMax. make a protected function to overwrite in player state
            if (!checkSlope || Library.IsBetweenGivenSlopes(hit.normal, 0, ANGLE_MAX)) {
                return player.transform.position.y - hit.transform.position.y;
            }
            else {
                return none;
            }
            
        }
    }

    internal RaycastHit CheckSurface(
        Vector3 distanceFromTransform, Vector3 direction, float distance, bool debugOn = false) {
        Ray ray = new Ray(player.transform.position + distanceFromTransform, direction);
        Physics.Raycast(
            ray, 
            out RaycastHit hitFloor, 
            distance, 
            1 << LayerMask.NameToLayer("Default"), 
            queryTriggerInteraction:QueryTriggerInteraction.Ignore
        );

        if (debugOn) {
            Debug.DrawRay(ray.origin, ray.direction, Color.white, distance);
            //print(distanceFromTransform);
        }

        return hitFloor;
    }

    public virtual void ReEnter(Component arg) {
        /**/
    }

    private readonly float lerpRotation = 45.0f;
    public virtual float GetLerpRotation()
    {
        return lerpRotation;
    }

    public virtual bool CanBeHurt() {
        return true;
    }

    public abstract void Enter(Component arg);
    public abstract void PhysicsUpdate();
    public abstract void GraphicsUpdate();
    public abstract void TransitionCheck();
    public abstract void Exit();
    
    public virtual void OnLateUpdate() {
        /**/
    }


    internal Vector3 ApplyForce(Vector3 velocity)
    {
        velocity += player.velocityFromForce * Time.deltaTime;
        return velocity;
    }
    internal void Squash(Timer timer)
    {
        float squash = 0;
        if (timer.IsActive())
        {
            float t = timer.GetPercent();
            squash = (1.0f - t) / 2.0f;
        }
        player.SetScale(squash);
    }

}
