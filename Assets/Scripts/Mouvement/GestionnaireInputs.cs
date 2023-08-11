using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionnaireInputs : MonoBehaviour
{
    Vector2 vecteurMouvement = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        // collecte à chaque frame des inputs
        vecteurMouvement.x = Input.GetAxis("Horizontal");
        vecteurMouvement.y = Input.GetAxis("Vertical");
    }

    public DonneesInputReseau GetInputReseau()
    {
        //création d'un nouveau struc
       
        DonneesInputReseau donneesInputReseau = new DonneesInputReseau();

        donneesInputReseau.mouvementInput = vecteurMouvement;
       
        return donneesInputReseau;
    }
}
