using System;
using UnityEngine;

public class Timer : MonoBehaviour {

    [SerializeField] protected float max;

    protected float counter;
    protected bool activeOnPreviousFrame = false;

    public float GetPercent() {
        return counter/(float)max;
    }

    public void Update(){
        activeOnPreviousFrame = IsActive();
        counter += 60 * Time.deltaTime;
    }

    public bool IsActive() {
        return 0 <= counter && counter < max;
    }

    public bool ActiveOnPreviousFrame() {
        return activeOnPreviousFrame;
    }

    public bool JustDeactivated() {
        return !IsActive() && activeOnPreviousFrame;
    }

    public void Reset() {
        counter = 0;
    }

    public void Reset(float n) {
        max = n;
        Reset();
    }

    public void End() {
        counter = max;
    }
    
    public float GetCounter() {
        return counter;
    }

    public void SetCounter(float n) {
        counter = n;
    }
    
    public float GetMax() {
        return max;
    }

    internal void Decrement() {
        float counter_new = GetCounter() - (60 * Time.deltaTime);
        if (counter_new < 0) {
            counter_new = 0;
        }
        SetCounter(counter_new);
    }

}
