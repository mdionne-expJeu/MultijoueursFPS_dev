using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

/*
 * Script qui exécute les déplacement du joueur et ainsi que l'ajustement de direction
 * Dérive de NetworkBehaviour. Utilisation de la fonction réseau FixedUpdateNetwork()
 * Variables :
 * - camLocale : contient la référence à la caméra du joueur actuel
 * - NetworkCharacterControllerPrototypeV2 : pour mémoriser le component NetworkCharacterControllerPrototypeV2 
 * du joueur
 */

public class GestionnaireMouvementPersonnage : NetworkBehaviour
{
    Camera camLocale;
    NetworkCharacterControllerPrototypeV2 networkCharacterControllerPrototypeV2;

    GestionnairePointsDeVie gestionnairePointsDeVie;
    bool respawnDemande = false;

    /*
     * Avant le Start(), on mémorise la référence au component networkCharacterControllerPrototypeV2 du joueur
     * On garde en mémoire la camera du joueur courant (GetComponentInChildren)
     */
    void Awake()
    {
        networkCharacterControllerPrototypeV2 = GetComponent<NetworkCharacterControllerPrototypeV2>();
        camLocale = GetComponentInChildren<Camera>();
        gestionnairePointsDeVie = GetComponent<GestionnairePointsDeVie>();
    }

    /*
     * Fonction récursive réseau pour la simulation. À utiliser pour ce qui doit être synchronisé entre
     * les différents clients.
     * 1.Récupération des Inputs mémorisés dans le script GestionnaireReseau (input.set). Ces données enregistrées
     * sous forme de structure de données (struc) doivent être récupérées sous la même forme.
     * 2.Ajustement de la direction du joueur à partir à partir des données de Input enregistrés dans les script
     * GestionnaireRéseau et GestionnaireInputs.
     * 3. Correction du vecteur de rotation pour garder seulement la roation Y pour le personnage (la capsule)
     * 4.Calcul du vecteur de direction du déplacement en utilisant les données de Input enregistrés.
     * Avec cette formule,il y a un déplacement latéral (strafe) lié  à l'axe horizontal (mouvementInput.x)
     * Le vecteur est normalisé pour être ramené à une longueur de 1.
     * Appel de la fonction Move() du networkCharacterControllerPrototypeV2 (fonction préexistante)
     * 5.Si les données enregistrées indique un saut, on appelle la fonction Jump() du script
     * networkCharacterControllerPrototypeV2 (fonction préexistante)
     */
    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority)
        {
            if (respawnDemande)
            {
                Respawn();
                return;
            }
                
            if (gestionnairePointsDeVie.estMort)
                return;
        }
        

        // 1.
        GetInput(out DonneesInputReseau donneesInputReseau);
        
        //Debug.Log($"Je suis joueur numéro_{Object.Id} MouvementInput = {donneesInputReseau.mouvementInput}");
       
        //2.
        if (donneesInputReseau.vecteurDevant != Vector3.zero)
        {
            transform.forward = donneesInputReseau.vecteurDevant;
        }
            
        //3.
        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        transform.rotation = rotation;

        //3.
        Vector3 directionMouvement = transform.forward * donneesInputReseau.mouvementInput.y + transform.right * donneesInputReseau.mouvementInput.x;
        directionMouvement.Normalize();
        networkCharacterControllerPrototypeV2.Move(directionMouvement);

        //4.saut, important de le faire après le déplacement
        if (donneesInputReseau.saute) networkCharacterControllerPrototypeV2.Jump();

        VerifieSiPeroTombe();
        
    }

    void VerifieSiPeroTombe()
    {
        if (transform.position.y < -10 && Object.HasStateAuthority)
            Respawn();
    }

    public void DemandeRespawn()
    {
        respawnDemande = true;
    }

    void Respawn()
    {
        networkCharacterControllerPrototypeV2.TeleportToPosition(Utilitaires.GetPositionSpawnAleatoire());
        respawnDemande = false;

        gestionnairePointsDeVie.Respawn();
    }
    
    public void ActivationCharacterController(bool estActif)
    {
        networkCharacterControllerPrototypeV2.Controller.enabled = estActif;
    }
}
