using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

using UnityEngine.UI;

public class GestionnairePointsDeVie : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnPtsVieChange))]
    byte ptsVie { get; set; }

    [Networked(OnChanged = nameof(OnChangeEtat))]
    public bool estMort { get; set;}

    bool estInitialise = false;
    const byte ptsVieDepart = 5;

    public Color uiCouleurTouche;
    public Image uiImageTouche;
    public MeshRenderer persoRenderer; // pour signaler aux autres qu'on est touché
    Color couleurNormalPerso;



    private void Awake()
    {
          
    }

    void Start()
    {
        ptsVie = ptsVieDepart;
        estMort = false;
        estInitialise = true;

        couleurNormalPerso = persoRenderer.material.color;
    }

    IEnumerator EffetTouche_co()
    {
        persoRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiImageTouche.color = uiCouleurTouche;

        yield return new WaitForSeconds(0.2f);
        persoRenderer.material.color = couleurNormalPerso;

        if (Object.HasInputAuthority && !estMort)
            uiImageTouche.color = new Color(0, 0, 0, 0);

    }

    // Fonction appelée uniquement par le serveur
    public void PersoEstTouche()
    {
        if (estMort)
            return;
        ptsVie -= 1;
        
        Debug.Log($"{Time.time} {transform.name} est touché. Il lui reste {ptsVie} points de vie");

        if(ptsVie <=0)
        {
            Debug.Log($"{Time.time} {transform.name} est mort");
            estMort = true;
        }
    }

    // Fonction statiques : ne peuvent accèder au variable non static
    static void OnPtsVieChange(Changed<GestionnairePointsDeVie> changed)
    {
        Debug.Log($"{Time.time} Valeur PtsVie = {changed.Behaviour.ptsVie}");

        byte nouveauPtsvie = changed.Behaviour.ptsVie;
        changed.LoadOld();
        byte ancienPtsVie = changed.Behaviour.ptsVie;
        if (nouveauPtsvie < ancienPtsVie)
            changed.Behaviour.ReductionPtsVie(); // pour appeler fonction non statique
    }

    static void OnChangeEtat(Changed<GestionnairePointsDeVie> changed)
    {
        Debug.Log($"{Time.time} Valeur estMort = {changed.Behaviour.estMort}");
    }

    private void ReductionPtsVie()
    {
        if (!estInitialise)
            return;

        StartCoroutine(EffetTouche_co());
    }
}
