using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GestionnaireArmes : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnTir))] // appelle une fonction statique
    public bool ilTir { get; set; } // variable réseau peuvent seulement être changée par le serveur (stateAuthority)

    float tempsDernierTir = 0;
    float delaiTirLocal = 0.15f;
    float delaiTirServeur = 0.1f;

    public ParticleSystem particulesTir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
       if(GetInput(out DonneesInputReseau donneesInputReseau))
        {
            if (donneesInputReseau.appuieBoutonTir)
                TirLocal(donneesInputReseau.vecteurDevant);

        }
    }

    void TirLocal(Vector3 vecteurTir)
    {
        if (Time.time - tempsDernierTir < delaiTirLocal)
            return;

        StartCoroutine(EffetTirCoroutine());
        tempsDernierTir = Time.time;
    }

    IEnumerator EffetTirCoroutine()
    {
        ilTir = true; // comme la variable networked est changé, la fonction OnTir sera appelée
        particulesTir.Play();
        yield return new WaitForSeconds(delaiTirServeur);
        ilTir = false;
    }

    static void OnTir(Changed<GestionnaireArmes> changed) //static. On doit appeler une fonction non static
    {
        Debug.Log($"{Time.time} Valeur OnTir() = {changed.Behaviour.ilTir}");

        //Dans fonction static, on ne peut pas changer ilTir = true. Utiliser changed.Behaviour.ilTir

        bool ilTirValeurActuelle = changed.Behaviour.ilTir;

        changed.LoadOld(); // charge la valeur précédente de la variable;

        bool ilTirValeurAncienne = changed.Behaviour.ilTir;

        if (ilTirValeurActuelle && !ilTirValeurAncienne) // pour tirer seulement une fois
            changed.Behaviour.TirDistant(); // appel fonction non static dans fonction static


    }

    void TirDistant()
    {
        //seulement pour les objets distants (par pour le joueur local)
        if(!Object.HasInputAuthority)
        {
            particulesTir.Play();
            Debug.Log("Tir du client disant" + Object.Id);
        }
    }
}
