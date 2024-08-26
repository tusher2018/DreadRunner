using UnityEngine;

public class DestroyBehindObject : MonoBehaviour
{
    private GameObject referenceObject; // The reference object used to determine what is behind
    public float destroyDistance = 100f; // Distance behind the reference object where objects will be destroyed



        void Start()
        {
            referenceObject = GameObject.FindWithTag("MainCamera");
        }


    void Update()
    {
        if (referenceObject == null)
        {
            Debug.LogError("Reference object is not assigned.");
            return;
        }

        // Get all colliders in the scene
        Collider[] colliders = FindObjectsOfType<Collider>();

        foreach (Collider col in colliders)
        {
            // Calculate the distance from the reference object to the object
            Vector3 objectPosition = col.transform.position;
            float distanceFromReference = Vector3.Dot(referenceObject.transform.forward, objectPosition - referenceObject.transform.position);

            // Check if the object is behind the reference object and beyond the destroy distance
            if (distanceFromReference < -destroyDistance)
            {
                // Destroy the object
                Destroy(col.gameObject);
            }
        }
    }
}
