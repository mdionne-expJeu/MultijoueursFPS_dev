using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desactiveCameraPrincipale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
