using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.Xml;

public class GestionnairePointage : NetworkBehaviour
{
    [SerializeField]
    static Dictionary<string, byte> infosJoueursPointages = new Dictionary<string, byte>();

    [SerializeField]
    [Networked(OnChanged = nameof(ChangementPointageStatic))]
    byte pointage { get; set; }

    /*[SerializeField]
    [Networked(OnChanged = nameof(ChangementPointage))]
    NetworkDictionary<NetworkString<_16>, byte> infoPointage => default;*/
    //byte pointageJoueur { get; set; }

    public TextMeshProUGUI[] txt_InfoPointageJoueurs;
    

    void Start()
    {
        pointage = 1;
    }

    public void EnregistrementNom(NetworkString<_16> leNom)
    {
        print($"enregistrement nom {gameObject.name}");
        //infoPointage.Add(leNom, 0);
        pointage = 0;
        infosJoueursPointages.Add(leNom.ToString(), 0);
        AffichePointage();
        //print($"le dictionnaire static contient {infosJoueursPointages.Count} valeurs");
    }

    public void ChangementPointage(string nomJoueur, byte valeur)
    {
        RPC_ChangementPointage(nomJoueur, valeur);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ChangementPointage(string nomJoueur, byte valeur, RpcInfo infos = default)
    {
        if (Object.HasInputAuthority)
        {
            print("Je recois le message comme serveur");
        }
        else
        {
            print("Je recois le message comme client");
        }

        pointage += valeur;
        infosJoueursPointages[nomJoueur] += valeur;
        AffichePointage();
    }

        

    public void AffichePointage()
    {
        print($"Dictionnaire = {infosJoueursPointages.Count}");
        var i = 0;

        if(infosJoueursPointages.Count == 2)
        {
            //print(infosJoueursPointages.g);
        }
        foreach (KeyValuePair<string, byte> itemDictio in infosJoueursPointages)
        {
            txt_InfoPointageJoueurs[i].text = $"{itemDictio.Key} : {itemDictio.Value} points";
            i ++;
        }
    }

    static void ChangementPointageStatic(Changed<GestionnairePointage> changed)
    {
        changed.Behaviour.AffichePointage();
    }

    public override void FixedUpdateNetwork()
    {
       AffichePointage();
    }
}
