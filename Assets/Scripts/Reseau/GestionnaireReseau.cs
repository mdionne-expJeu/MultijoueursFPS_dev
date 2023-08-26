using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System;

public class GestionnaireReseau : MonoBehaviour, INetworkRunnerCallbacks
{
    //Contient référence au component NetworkRunner
    NetworkRunner _runner;

    // pour mémoriser le component GestionnaireMouvementPersonnage du joueur
    GestionnaireInputs gestionnaireInputs;

    // Contient la référence au script JoueurReseau du Prefab
    public JoueurReseau joueurPrefab;

    private Dictionary<int, Color> joueursCouleurs = new Dictionary<int, Color>();
    public Color[] couleurTest;
    public int nbJoueurs = 0;
    // Fonction asynchrone pour démarrer Fusion et créer une partie 
    async void CreationPartie(GameMode mode)
    {
        /*  1.Ajout du component NetworkRunne au gameObject. On garde en mémoire
            la référence à ce component dans la variable _runner.
            2.Indique au NetworkRunner qu'il doit fournir les inputs au simulateur (Fusion)
        */
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        /*Méthode du NetworkRunner qui permet d'initialiser une partie
         * GameMode : reçu en argument. Valeur possible : Client, Host, Server, AutoHostOrClient, etc.)
         * SessionName : Nom de la chambre (room) pour cette partie
         * Scene : la scène qui doit être utilisée pour la simulation
         * SceneManager : référence au component script NetworkSceneManagerDefault qui est ajouté au même moment
         */
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Chambre test",
            Scene = SceneManager.GetActiveScene().buildIndex,
            PlayerCount = 10,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()

        });
    }

    void Start()
    {
        // Création d'une partie dès le départ
        CreationPartie(GameMode.AutoHostOrClient);
    }


    /* Lorsqu'un joueur se connecte au serveur
     * 1.On vérifie si ce joueur est aussi le serveur. Si c'est le cas, on spawn un prefab de joueur.
     * Bonne pratique : la commande Spawn() devrait être utilisé seulement par le serveur
    */
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        if(_runner.IsServer)
        {
            
            Debug.Log("Un joueur s'est connecté comme serveur. Spawn d'un joueur");
            Debug.Log($"Création du joueur {player.PlayerId} par le serveur");
           var leNouveu =  _runner.Spawn(joueurPrefab, Utilitaires.GetPositionSpawnAleatoire(), Quaternion.identity, player);
            Debug.Log(leNouveu.GetType());
            
            leNouveu.maCouleur = couleurTest[nbJoueurs];
            nbJoueurs++;
            Debug.Log(couleurTest);
        }
        else
        {
            Debug.Log("Un joueur s'est connecté comme client. Spawn d'un joueur");
        }
    } 

    

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Le joueur {player.PlayerId} a quitté la partie");
    }

    /*
     * Fonction du Runner pour définir les inputs du client dans la simulation
     * 1. On récupère le component GestionnaireInputs du joueur local
     * 2. On définit (set) le paramètre input en lui donnant la structure de données (struc) qu'on récupère
     * en appelant la fonction GestInputReseau du script GestionnaireInputs. Les valeurs seront mémorisées
     * et nous pourrons les utilisées pour le déplacement du joueur dans un autre script.Ouf...
     */
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //1.
        if(gestionnaireInputs == null && JoueurReseau.Local !=null)
        {
            
            gestionnaireInputs = JoueurReseau.Local.GetComponent<GestionnaireInputs>();
        }

        //2.
        if(gestionnaireInputs !=null)
        {
            input.Set(gestionnaireInputs.GetInputReseau());
        }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if(shutdownReason == ShutdownReason.GameIsFull)
        {
            Debug.Log("Le maximum de joueur est atteint. Réessayer plus tard.");
        }
       
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("OnOnDisconnectedFromServer");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Connection demandée par = " + runner.GetPlayerUserId());
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connection refusée par = " + runner.GetPlayerUserId());
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
       
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
       
    }
}
