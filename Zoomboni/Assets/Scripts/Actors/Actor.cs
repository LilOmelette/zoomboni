using UnityEngine;

public abstract class Actor : MonoBehaviour {


    public abstract bool IsGrounded();

    internal float GetDistance(Actor actor) {
        return (transform.position - actor.transform.position).magnitude;
    }

}
