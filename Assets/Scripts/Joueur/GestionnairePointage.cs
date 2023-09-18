using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class GestionnairePointage : NetworkBehaviour
{

    byte pointage;

    GestionnaireAffichagePointage gestionnaireAffichagePointage;


    void Awake()
    {
        gestionnaireAffichagePointage = FindFirstObjectByType<GestionnaireAffichagePointage>();
    }

    public void EnregistrementNom(string leNom)
    {
        gestionnaireAffichagePointage.EnregistrementNom(leNom);
    }

    public void ChangementPointage(string nomJoueur, byte valeur)
    {
        
        RPC_ChangementPointage(nomJoueur, valeur);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangementPointage(string nomJoueur, byte valeur, RpcInfo infos = default)
    {
        gestionnaireAffichagePointage.MiseAJourPointage(nomJoueur, valeur);
        if (Object.HasStateAuthority)
        {
            print($"{gameObject.name} Je recois le message comme serveur");
        }
        else
        {
            print($"{gameObject.name} Je recois le message comme client");
        }
        



    }

        

    

    

    
}
