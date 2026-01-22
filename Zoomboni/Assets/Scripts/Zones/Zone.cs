using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Zone : MonoBehaviour{

    internal abstract void Enter(Collider collider);

    internal abstract void Exit(Collider collider);

    public void OnTriggerEnter(Collider collider) {
        if (DoesActivateMe(collider)) {
            Enter(collider);
        }
    }

    public void OnTriggerExit(Collider collider) {
        if (DoesActivateMe(collider)) {
            Exit(collider);
        }
    }

    internal virtual bool DoesActivateMe(Collider collider) {
        return collider.GetComponentInParent<Player>() && collider.gameObject.tag == "Player";
    }

}
