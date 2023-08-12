using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GestionnaireCameraLocale : MonoBehaviour
{
    public Transform ancrageCamera;

    Vector2 vueInput;

    float cameraRotationX = 0f;
    float cameraRotationY = 0f;


    Camera localCamera;

    NetworkCharacterControllerPrototypeV2 networkCharacterControllerPrototypeV2;

    void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeV2 = GetComponentInParent<NetworkCharacterControllerPrototypeV2>();
    }

    void Start()
    {
        if (localCamera.enabled)
            localCamera.transform.parent = null;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (ancrageCamera == null) return;
        if (!localCamera.enabled) return;

        localCamera.transform.position = ancrageCamera.position;

        cameraRotationX -= vueInput.y * Time.deltaTime * networkCharacterControllerPrototypeV2.vitesseVueHautBas;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += vueInput.x * Time.deltaTime * networkCharacterControllerPrototypeV2.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetInputVue(Vector2 vueInputVecteur)
    {
        vueInput = vueInputVecteur;
    }
}
