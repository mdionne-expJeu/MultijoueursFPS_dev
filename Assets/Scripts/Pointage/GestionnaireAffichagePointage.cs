using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class GestionnaireAffichagePointage : MonoBehaviour
{
    [SerializeField]
    static Dictionary<string, byte> infosJoueursPointages = new Dictionary<string, byte>();

    public TextMeshProUGUI[] txt_InfoPointageJoueurs;

    public void EnregistrementNom(string leNom)
    {
       // print($"enregistrement nom {gameObject.name}");
        //infoPointage.Add(leNom, 0);
        
        infosJoueursPointages.Add(leNom.ToString(), 0);
        AffichePointage();
        //print($"le dictionnaire static contient {infosJoueursPointages.Count} valeurs");
    }

    public void MiseAJourPointage(string nomJoueur, byte valeur)
    {
        infosJoueursPointages[nomJoueur] += valeur;
        AffichePointage();
    }

    void AffichePointage()
    {
        print($"Dictionnaire = {infosJoueursPointages.Count}");
        foreach(var zonTexte in txt_InfoPointageJoueurs)
        {
            zonTexte.text = string.Empty;
        }
        var i = 0;
        foreach (KeyValuePair<string, byte> itemDictio in infosJoueursPointages)
        {
            txt_InfoPointageJoueurs[i].text = $"{itemDictio.Key} : {itemDictio.Value} points";
            i++;
        }
    }

    public void SupprimeJoueur(string nomJoueur)
    {
        infosJoueursPointages.Remove(nomJoueur);
        AffichePointage();
    }

    void Update()
    {
        
    }
}
