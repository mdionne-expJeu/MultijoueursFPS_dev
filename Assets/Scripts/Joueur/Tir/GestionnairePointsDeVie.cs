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

    // pour la mort du perso
    public GameObject modelJoueur;
    public GameObject particulesMort;
    HitboxRoot hitboxRoot; // pour empêcher collision lorsque mort
    GestionnaireMouvementPersonnage gestionnaireMouvementPersonnage;
    public Material particulesMateriel;
    JoueurReseau joueurReseau;

    private void Awake()
    {
        hitboxRoot = GetComponent<HitboxRoot>();
        gestionnaireMouvementPersonnage = GetComponent<GestionnaireMouvementPersonnage>();
        joueurReseau = GetComponent<JoueurReseau>();
    }

    void Start()
    {
        ptsVie = ptsVieDepart;
        estMort = false;
        estInitialise = true;

        couleurNormalPerso = persoRenderer.material.color;
    }

    IEnumerator EffetTouche_CO()
    {
        persoRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiImageTouche.color = uiCouleurTouche;

        yield return new WaitForSeconds(0.2f);
        persoRenderer.material.color = couleurNormalPerso;

        if (Object.HasInputAuthority && !estMort)
            uiImageTouche.color = new Color(0, 0, 0, 0);
    }

    IEnumerator RessurectionServeur_CO()
    {
        yield return new WaitForSeconds(2);
        gestionnaireMouvementPersonnage.DemandeRespawn();
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
            StartCoroutine(RessurectionServeur_CO());
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

    private void ReductionPtsVie()
    {
        if (!estInitialise)
            return;

        StartCoroutine(EffetTouche_CO());
    }

    static void OnChangeEtat(Changed<GestionnairePointsDeVie> changed)
    {
        Debug.Log($"{Time.time} Valeur estMort = {changed.Behaviour.estMort}");
        bool estMortNouveau = changed.Behaviour.estMort;
        changed.LoadOld();
        bool estMortAncien = changed.Behaviour.estMort;

        if(estMortNouveau)
        {
            changed.Behaviour.Mort();
        }
        else if(!estMortNouveau && estMortAncien)
        {
            changed.Behaviour.Resurrection();
        }
        
    }

    private void Mort()
    {
        modelJoueur.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        gestionnaireMouvementPersonnage.ActivationCharacterController(false);
        GameObject nouvelleParticule =  Instantiate(particulesMort, transform.position, Quaternion.identity);
        particulesMateriel.color = joueurReseau.maCouleur;
        Destroy(nouvelleParticule, 3);
    }

    private void Resurrection()
    {
        if(Object.HasInputAuthority)
            uiImageTouche.color = new Color(0, 0, 0, 0);
        hitboxRoot.HitboxRootActive = true;
        gestionnaireMouvementPersonnage.ActivationCharacterController(true);
        StartCoroutine(JoueurVisible());
    }

    IEnumerator JoueurVisible()
    {
        yield return new WaitForSeconds(0.1f);
        modelJoueur.gameObject.SetActive(true);
    }

    public void Respawn()
    {
        ptsVie = ptsVieDepart;
        estMort = false;
    }
}
