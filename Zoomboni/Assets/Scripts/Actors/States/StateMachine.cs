using System;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    [SerializeField] internal State stateInit;

    protected State state;

    private void Awake() {
        state = stateInit;
    }

    private void OnEnable() {
        state.Enter(null);
    }

    private void Start() {
        state.Enter(null);
    }
    
    private void Update() {
        state.GraphicsUpdate();

        state.PhysicsUpdate();
        state.TransitionCheck();
        state.OnLateUpdate();
    }

    private void FixedUpdate() {
    }

    private void LateUpdate() {
    }

    public virtual void Change(State to) {
        if (to == null) {
            Debug.LogError("State is null!");
            return;
        }
        if (to == state ) {
            to.ReEnter(null);
            return;
        }
        Debug.Log(transform.parent.name + " TO: " + to.name + " FROM: " + state.name);
        state.Exit();
        state = to;
        state.Enter(to);
    }

    public void OnReset() {
        if (state) {
            Change(stateInit);
        }
        else {
            state = stateInit;
            if (stateInit == null) {
                print("State init is null for " + name);
            }
            state.Enter(null);
        }
    }

    public State GetState() {
        return state;
    }

}
