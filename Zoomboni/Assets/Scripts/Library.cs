using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Library : MonoBehaviour
{

    private static Library instance;
    public void Awake() {
        if (instance == null) {
            instance = this;
        }
    }
    public static Library GetInstance() {
        if (instance == null) {
            Debug.LogError("Tried to access Library before its instantiation");
        }
        return instance;
    }

    public static bool IsBetweenGivenSlopes(Vector3 normal, float min, float max) {
        const float angleMax = 90;
        var slopeAngle = Vector3.Angle(Vector3.up, normal)/angleMax;
        bool tooSteep = slopeAngle >= min && slopeAngle < max;
        //Debug.Log("Normal: " + normal + " Angle: " + slopeAngle + "Min: " + min +  "Max: " + max + " Too steep?: " + tooSteep);
        return tooSteep;
    }

     public static T GetRandomElementFromList<T>(List<T> lst) {
        int index = (int) (Random.value * lst.Count);
        if (index >= lst.Count) {
            Debug.LogError("GetRandomElementFromList got illegal index " + index + " from list of size " + lst.Count);
            return default;
        }
        return lst[index];
    }
}
