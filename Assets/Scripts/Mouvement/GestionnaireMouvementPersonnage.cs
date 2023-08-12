using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

/*
 * Script qui exécute les déplacement du joueur et qui gère la rotatio de la caméra FPS.
 * Dérive de NetworkBehaviour. Utilisation de la fonction réseau FixedUpdateNetwork()
 * Variables
 * - vueInput :Vector2 pour récupérer le déplacement horizontal et vertical de la souris transmis
 * par le GestionnaireInputs
 * 
 * - cameraRotationX : Float qui mémorise la rotation X de la caméra (vue haut/bas seulement)
 * - camLocale : contient la référence à la caméra du joueur actuel
 * - NetworkCharacterControllerPrototypeV2 : pour mémoriser le component NetworkCharacterControllerPrototypeV2 
 * du joueur
 */


public class GestionnaireMouvementPersonnage : NetworkBehaviour
{
    Vector2 vueInput;
    float cameraRotationX = 0f;
    Camera camLocale;
    NetworkCharacterControllerPrototypeV2 networkCharacterControllerPrototypeV2;

    /*
     * Avant le Start(), on mémorise la référence au component networkCharacterControllerPrototypeV2 du joueur
     * On garde en mémoire la camera du joueur courant (GetComponentInChildren)
     */
    void Awake()
    {
        networkCharacterControllerPrototypeV2 = GetComponent<NetworkCharacterControllerPrototypeV2>();
        camLocale = GetComponentInChildren<Camera>();
    }

    /*
     * Fonction locale seulement. Ajustement de la rotation X de la caméra en fonction du déplacement vertical
     * de la souris récupéré dans le script GestionaireInputs.
     * 1.On soustrait le déplacement vertical de la souris (addition pour inverser le mouvement)
     * La variable vitesseVueHautBas qui permet de changer la vitesse de rotation est déclarée dans le script
     * networkCharacterControllerPrototypeV2. Simple question d'uniformité.
     * 2. La valeur est limitée entre -90 et 90 (clamp) et la rotation appliquée à la caméra
     */
    private void Update()
    {
        //1.Action local seulement non synchronisée sur le réseau (vue haut/bas)
        cameraRotationX -= vueInput.y * Time.deltaTime * networkCharacterControllerPrototypeV2.vitesseVueHautBas;

        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);
        camLocale.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
    }

    /*
     * Fonction récursive réseau pour la simulation. À utiliser pour ce qui doit être synchronisé entre
     * les différents clients.
     * 1.Récupération des Inputs mémorisés dans le script GestionnaireReseau (input.set). Ces données enregistrées
     * sous forme de structure de données (struc) doivent être récupérées sous la même forme.
     * 2.Ajustement de la roation du joueur à partir à partir des données de Input enregistrés.La fonction Rotate() 
     * doit être créé dans le script networkCharacterControllerPrototypeV2
     * 3.Calcul du vecteur de direction du déplacement en utilisant les données de Input enregistrés.
     * Avec cette formule,il y a un déplacement latéral (strafe) lié  à l'axe horizontal (mouvementInput.x)
     * Le vecteur est normalisé pour être ramené à une longueur de 1.
     * Appel de la fonction Move() du networkCharacterControllerPrototypeV2 (fonction préexistante)
     * 4.Si les données enregistrées indique un saut, on appelle la fonction Jump() du script
     * networkCharacterControllerPrototypeV2 (fonction préexistante)
     */
    public override void FixedUpdateNetwork()
    {
        // 1.
        GetInput(out DonneesInputReseau donneesInputReseau);

        //2.
        networkCharacterControllerPrototypeV2.Rotate(donneesInputReseau.rotationInput);

        //3.
        Vector3 directionMouvement = transform.forward * donneesInputReseau.mouvementInput.y + transform.right * donneesInputReseau.mouvementInput.x;
        directionMouvement.Normalize();
        networkCharacterControllerPrototypeV2.Move(directionMouvement);

        //4.saut, important de le faire après le déplacement
        if (donneesInputReseau.saute) networkCharacterControllerPrototypeV2.Jump();
    }
    /*
     * Fonction publique appelée de l'extérieur, par le script GestionnaireInput. Permet de recevoir la valeur
     * de rotation de la souris fourni par le Update (hors simulation) pour l'ajustement de la rotation X de la
     * caméra (vue haut/bas) qui n'est pas incluse dans la simulation.
     */
    public void AjustementVue(Vector2 vueInputVecteur)
    {
        vueInput = vueInputVecteur; 
    }
}
