using UnityEngine;

public class Collectable : MonoBehaviour
{

    [SerializeField] private int POINTS = 10;
    [SerializeField] private GameObject prefabOnCollect;

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            LevelManager.GetInstance().AddPoints(POINTS);
            // Create collect FX then set its position to ours
            GameObject fx = Instantiate(prefabOnCollect, transform);
            fx.transform.localPosition = new Vector3();
            fx.transform.parent = null;

            Destroy(gameObject);
        }
    }

}