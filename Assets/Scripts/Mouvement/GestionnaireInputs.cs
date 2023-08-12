using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script qui récupère la valeur des inputs (clavier et souris) à chaque update()
 * Dérive de MonoBehavior
 * Exécution locale. Les données seront transmises au réseau sur demande
 * Variables
 * - mouvementInputVecteur :Vector2 pour mémoriser axes vertical et horizontal
 * - vueInputVecteur : Vector2 pour mémoriser les déplacements de la souris, horizontal et vertical.
 * - ilSaute : bool qui sera activée lorsque le joueur saute
 * - gestionnaireMouvementPersonnage : pour mémoriser le component GestionnaireMouvementPersonnage du joueur
 */

public class GestionnaireInputs : MonoBehaviour
{
    Vector2 mouvementInputVecteur = Vector2.zero;
    Vector2 vueInputVecteur = Vector2.zero;
    bool ilSaute;

    GestionnaireMouvementPersonnage gestionnaireMouvementPersonnage;

    /*
     * Avant le Start(), on mémorise la référence au component GestionnaireMouvementPersonnage du joueur
     */
    void Awake()
    {
        gestionnaireMouvementPersonnage = GetComponent<GestionnaireMouvementPersonnage>();
    }

    /*
     * On s'assure que le curseur est invisible et verrouillé au centre
     */
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * On mémorise à chaque frame la valeurs des inputs
     * 1.La rotation Y du personnage est importante pour les autres joueurs. Devra être synchronisée par le serveur
     * La rotation X (vue haut/bas) n'est pas importante pour les autres joueurs. Pourra se faire localement seulement
     * Appel de la fonction AjustementVue dans le script gestionnaireMouvementPersonnage en lui passant l'information
     * nécessaire pour la rotation (vueInputVecteur)
     */
    void Update()
    {
        // Déplacement
        mouvementInputVecteur.x = Input.GetAxis("Horizontal");
        mouvementInputVecteur.y = Input.GetAxis("Vertical");

        // 1. Vue
        vueInputVecteur.x = Input.GetAxis("Mouse X"); //important pour les autres joueurs
        vueInputVecteur.y = Input.GetAxis("Mouse Y"); // pas important pour les autres joueurs
        gestionnaireMouvementPersonnage.AjustementVue(vueInputVecteur);

        //Saut
        if (Input.GetButtonDown("Jump"))
            ilSaute = true;
    }

    /*
     * Fonction qui sera appelée par le Runner qui gère la simulation (GestionnaireReseau). 
     * Lorsqu'elle est appelée, son rôle est de :
     * 1. créer une structure de données (struc) à partir du modèle DonneesInputReseau;
     * 2. définir les trois variables de la structure (mouvement, rotation et saute);
     * 3. retourne au Runner la structure de données
     */
    public DonneesInputReseau GetInputReseau()
    {
        //1.
        DonneesInputReseau donneesInputReseau = new DonneesInputReseau();

        //2.
        donneesInputReseau.mouvementInput = mouvementInputVecteur;
        donneesInputReseau.rotationInput = vueInputVecteur.x;
        donneesInputReseau.saute = ilSaute;
        ilSaute = false;
       //3.
        return donneesInputReseau;
    }
}
