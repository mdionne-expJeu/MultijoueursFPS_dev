using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireInputs : MonoBehaviour
{
    Vector2 mouvementInputVecteur = Vector2.zero;
    Vector2 vueInputVecteur = Vector2.zero;
    bool ilSaute;

    GestionnaireMouvementPersonnage gestionnaireMouvementPersonnage;

    // Start is called before the first frame update
    void Awake()
    {
        gestionnaireMouvementPersonnage = GetComponent<GestionnaireMouvementPersonnage>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // collecte à chaque frame des inputs

        // Déplacement
        mouvementInputVecteur.x = Input.GetAxis("Horizontal");
        mouvementInputVecteur.y = Input.GetAxis("Vertical");

        // Vue
        vueInputVecteur.x = Input.GetAxis("Mouse X"); //important pour les autres joueurs
        vueInputVecteur.y = Input.GetAxis("Mouse Y"); // pas important pour les autres joueurs
        gestionnaireMouvementPersonnage.AjustementVue(vueInputVecteur);

        //Saut
        ilSaute = Input.GetButtonDown("Jump");


    }

    public DonneesInputReseau GetInputReseau()
    {
        //création d'un nouveau struc
       
        DonneesInputReseau donneesInputReseau = new DonneesInputReseau();

        //Déplacement
        donneesInputReseau.mouvementInput = mouvementInputVecteur;

        //Vue
        donneesInputReseau.rotationInput = vueInputVecteur.x;

        //Saut
        donneesInputReseau.saute = ilSaute;
       
        return donneesInputReseau;
    }
}
