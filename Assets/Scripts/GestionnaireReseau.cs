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

    // Contient la référence au script JoueurReseau du Prefab
    public JoueurReseau joueurPrefab;

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
    */
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        if(_runner.IsServer)
        {
            Debug.Log("Un joueur s'est connecté comme serveur. Spawn d'un joueur");
            _runner.Spawn(joueurPrefab, Utilitaires.GetPositionSpawnAleatoire(), Quaternion.identity, player);
        }
        else
        {
            Debug.Log("Un joueur s'est connecté comme clien. Spawn d'un joueur");
        }
    } 

    

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
       
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
       
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
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
