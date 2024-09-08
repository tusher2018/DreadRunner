using UnityEngine;

public class DestroyBehindPlayer : MonoBehaviour
{
    public GameObject player;  // Reference to the player object
    public float destroyDistance = 500f;  // Distance behind the player at which the object should be destroyed


    void Update()
    {
        if(player == null){
            player=GameObject.FindWithTag("Player");
        }
        else{
        
            if (transform.position.z < player.transform.position.z - destroyDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
