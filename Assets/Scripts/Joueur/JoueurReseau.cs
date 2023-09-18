using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; // namespace pour utiliser les classes de Fusion
using TMPro;
/* 
 * 1.Les objets réseau ne doivent pas dériver de MonoBehavior, mais bien de NetworkBehavior
 * Importation de l'interface IPlayerLeft
 * 2.Variable pour mémoriser l'instance du joueur
 * 3.Fonction Spawned() : Semblable au Start(), mais pour les objets réseaux
 * Sera exécuté lorsque le personnage sera créé (spawn)
 * Test si le personnage créé est le personnage contrôlé par l'utilisateur local.
 * HasInputAuthority permet de vérifier cela.
 * Retourne true si on est sur le client qui a généré la création du joueur
 * Retourne false pour les autres clients
 * 4. Lorsqu'un joueur se déconnecte du réseau, on élimine (Despawn) son joueur.
 */
public class JoueurReseau : NetworkBehaviour, IPlayerLeft //1.
{
    public TextMeshProUGUI nomDuJoueurTxt;
    [Networked(OnChanged = nameof(ChangementDeNom_static))]
    public NetworkString<_16> nomDujoueur { get; set; }

    //Variable qui sera automatiquement synchronisée par le serveur sur tous les clients
    [Networked] public Color maCouleur { get; set; }

    bool messageEnvoieDuNom;
    MessagesJeuReseau messagesJeuReseau;

    public static JoueurReseau Local;  //.2

    //Ajout d'une variable public Transform. Dans Unity, glisser l'objet "visuel" du prefab du joueur
    public Transform modeleJoueur;

    public GestionnaireCameraLocale gestionnaireCameraLocale;
    public GameObject LocalUI;

    

    private void Awake()
    {
        messagesJeuReseau = GetComponent<MessagesJeuReseau>();
    }

    /*
     * Au départ, on change la couleur du joueur. La variable maCouleur sera définie
     * par le serveur dans le script GestionnaireReseau. La fonction Start() sera
     * appelée après la fonction Spawned().
     */
    private void Start()
    {
        GetComponentInChildren<MeshRenderer>().material.color = maCouleur;
    }

    /*public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority)
        {
            Debug.Log($"FUN du StateAuthority pour le joueur {Object.Id}");
        }
        if (Object.HasInputAuthority)
        {
            Debug.Log($"FUN du InputAuthority pour le joueur {Object.Id}");
        }
        if(!Object.HasInputAuthority && !Object.HasStateAuthority)
        {
            Debug.Log($"FUN de AucuneAutorité pour le joueur {Object.Id}");
        }
    }*/

    public override void Spawned() //3.
    {
        if(Object.HasInputAuthority)
        {
            Local = this;

            //Si c'est le joueur du client, on appel la fonction pour le rendre invisible
            Utilitaires.SetRenderLayerInChildren(modeleJoueur, LayerMask.NameToLayer("JoueurLocal"));

            //On désactive la mainCamera. Assurez-vous que la caméra de départ possède bien le tag MainCamera
            Camera.main.gameObject.SetActive(false);

            Debug.Log("Un joueur local a été créé");
            RPC_ChangementdeNom(PlayerPrefs.GetString("NomDuJoueur"));
           
           
        }
        else
        {
            //Si le joueur créé est contrôlé par un autre joueur, on désactive le component caméra de cet objet
            Camera camLocale = GetComponentInChildren<Camera>();
            camLocale.enabled = false;

            // On désactive aussi le component AudioListener
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            LocalUI.SetActive(false);

            Debug.Log("Un joueur réseau a été créé");
        }

        //Définir le joueur pour le joueur local
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        transform.name = $"Joueur_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player) //.4
    {
        if (Object.HasStateAuthority)
        {
            if(Runner.TryGetPlayerObject(player, out NetworkObject leJoueurQuiQuitte))
            {
                if (leJoueurQuiQuitte == Object)
                    Local.GetComponent<MessagesJeuReseau>().EnvoieMessageJeuRPC(leJoueurQuiQuitte.GetComponent<JoueurReseau>().nomDujoueur.ToString(), "a quitté la partie");
            }
        }
            //messagesJeuReseau.EnvoieMessageJeuRPC(nomDujoueur.ToString(), "a quitté la partie");

        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    static void ChangementDeNom_static(Changed<JoueurReseau> changed)
    {
        print("static changement de nom");
        changed.Behaviour.ChangementDeNom();
    }

    private void ChangementDeNom()
    {
        print("instance changement de nom");
        Debug.Log($"Le nom du joueur {gameObject.name} est changé pour {nomDujoueur}");
        nomDuJoueurTxt.text = nomDujoueur.ToString();
        GetComponent<GestionnairePointage>().EnregistrementNom(nomDujoueur.ToString());
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ChangementdeNom(string leNom, RpcInfo infos = default)
    {
        //éxécuté sur serveur uniquement
        this.nomDujoueur = leNom;
        Debug.Log("RPC_ChangementdeNom");
        if(!messageEnvoieDuNom)
        {
            
            messagesJeuReseau.EnvoieMessageJeuRPC(leNom, "a rejoint la partie");
            messageEnvoieDuNom = true;
        }
    }
}
