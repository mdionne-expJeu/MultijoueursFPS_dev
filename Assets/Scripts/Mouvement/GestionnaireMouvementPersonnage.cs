using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GestionnaireMouvementPersonnage : NetworkBehaviour
{
    Vector2 vueInput;
    float cameraRotationX = 0f;
    Camera camLocale;

    NetworkCharacterControllerPrototypeV2 networkCharacterControllerPrototypeV2;


    void Awake()
    {
        networkCharacterControllerPrototypeV2 = GetComponent<NetworkCharacterControllerPrototypeV2>();
        camLocale = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        //Action local seulement non synchronisée sur le réseau (vue haut/bas)
        cameraRotationX -= vueInput.y * Time.deltaTime * networkCharacterControllerPrototypeV2.vitesseVueHautBas;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        camLocale.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    public override void FixedUpdateNetwork()
    {
        // get input from the client
       
        
        GetInput(out DonneesInputReseau donneesInputReseau);


        //ajuster la vue
        networkCharacterControllerPrototypeV2.Rotate(donneesInputReseau.rotationInput);

        // avance recule + mouvement latéral (strafe)
        Vector3 directionMouvement = transform.forward * donneesInputReseau.mouvementInput.y + transform.right * donneesInputReseau.mouvementInput.x;
        directionMouvement.Normalize();
        networkCharacterControllerPrototypeV2.Move(directionMouvement);

        

        //saut, important de le faire après le déplacement
        if (donneesInputReseau.saute) networkCharacterControllerPrototypeV2.Jump();
    }

    public void AjustementVue(Vector2 vueInputVecteur)
    {
        vueInput = vueInputVecteur;
        
    }
}
