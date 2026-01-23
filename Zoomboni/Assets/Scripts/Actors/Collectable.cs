using UnityEngine;

public class Collectable : MonoBehaviour
{

    [SerializeField] private int POINTS = 10;

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            LevelManager.GetInstance().AddPoints(POINTS);
            Destroy(gameObject);
        }
    }

}