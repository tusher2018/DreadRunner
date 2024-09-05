using UnityEngine;

public class DestroyBehindPlayer : MonoBehaviour
{
    public Transform player;  // Reference to the player object
    public float destroyDistance = 500f;  // Distance behind the player at which the object should be destroyed

void Start(){
    player=GameObject.FindWithTag("Player").transform;
}
    void Update()
    {
        // Check if the object is behind the player by the specified distance
        if (transform.position.z < player.position.z - destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
