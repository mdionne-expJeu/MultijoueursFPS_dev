using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GestionnaireMouvementPersonnage : NetworkBehaviour
{
    NetworkCharacterControllerPrototypeV2 networkCharacterControllerPrototypeV2;


    void Awake()
    {
        networkCharacterControllerPrototypeV2 = GetComponent<NetworkCharacterControllerPrototypeV2>();
    }

  

    public override void FixedUpdateNetwork()
    {
        // get input from the client
       
        
        GetInput(out DonneesInputReseau donneesInputReseau);

        // avance recule + mouvement latéral (strafe)
        Vector3 directionMouvement = transform.forward * donneesInputReseau.mouvementInput.y + transform.right * donneesInputReseau.mouvementInput.x;
        directionMouvement.Normalize();
        networkCharacterControllerPrototypeV2.Move(directionMouvement);

        
    }
}
