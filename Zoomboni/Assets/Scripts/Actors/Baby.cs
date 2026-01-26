using UnityEngine;

public class Baby : MonoBehaviour
{

    [SerializeField] private float START_VELOCITY_XZ = 10;

    void Start()
    {
        float a1 = 1 - (2 * Random.value);
        float a2 = 1 - (2 * Random.value);
        GetComponent<Rigidbody>().AddForce(new Vector3(a1* START_VELOCITY_XZ, -10f, a2 * START_VELOCITY_XZ));
    }
}
