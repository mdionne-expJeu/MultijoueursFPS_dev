using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MessagesJeuReseau : NetworkBehaviour
{
    public GestionnaireMessagesJeu gestionnaireMessagesJeu;

    
    void Start()
    {
        
    }

    public void EnvoieMessageJeuRPC(string nomDuJoueur, string leMessage)
    {
        print("fonction pour envoie de message RPC activée");
        RPC_MessagesJeu($"<b>{nomDuJoueur}</b> {leMessage}");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_MessagesJeu(string leMessage, RpcInfo infos = default)
    {
        Debug.Log($"RPC MessageJeu {leMessage}");
        if(gestionnaireMessagesJeu != null)
        {
            gestionnaireMessagesJeu.ReceptionMessage(leMessage);
        }
    }

    
}
