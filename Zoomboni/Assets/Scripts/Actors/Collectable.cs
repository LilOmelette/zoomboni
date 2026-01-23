using UnityEngine;

public class Collectable : MonoBehaviour
{

    public int points = 10;
    public Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            Debug.Log("Collected and item!");
            player.AddPoints(points);
            Destroy(gameObject);
        }
    }
}
