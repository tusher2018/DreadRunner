using UnityEngine;

public class RandomColor : MonoBehaviour
{
    // Reference to the material
    private Material material;

    void Start()
    {
        // Get the Renderer component from the GameObject this script is attached to
        Renderer renderer = GetComponent<Renderer>();

        // Get the material from the renderer
        material = renderer.material;

        // Generate a random color
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        // Apply the random color to the material's Albedo color
        material.color = randomColor;
    }
}
