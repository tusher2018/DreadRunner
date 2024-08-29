using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class notDestroy : MonoBehaviour
{
    void Start()
    {
    DontDestroyOnLoad(transform.gameObject);
    }
}
