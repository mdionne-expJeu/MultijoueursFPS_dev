using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/*Script qui gère la fusée, une fois qu'elle est spawné par le serveur
 * Variables :
 * - GameObject prefabExplosion: Le prefab avec particules qui sera utilisé pour l'explosion. À définir dans l'inspecteur.
 * - Transform pointDetectionCollision : point d'origine de la sphere qui sera créée pour la détection de collision.
 * A définir dans l'inspecteur.
 * - LayerMask layersCollision : Détermine le layer qui sera sensible à la détection de collision
 * - TickTimer dureeVieFusee : Timer propre a fusion. Désactivé au départ. Permettra de créer une temporisation pour l'explosion
 * de la fusée après un certain temps.
 * - List<LagCompensatedHit> infosCollisionsList: Liste qui contiendra les objets touchés lors de l'explosion de la fusée
 * - int vitesseFusee : la vitesse de déplacement de la fusée
 * - PlayerRef lanceur: Référence au joueur qui a lancé la fusée
 * - string nomLanceur: Le nom du joueur qui lance la fusée
 * - NetworkObject networkObject: Référence au component NetworkObject de la fusée
 * - NetworkObject lanceurNetworkObject:Référence au component NetworkObject du joueur qui lance la fusée
 */
public class GestionnaireFusee : NetworkBehaviour
{
    [Header("Prefab")]
    public GameObject prefabExplosion;

    [Header("Détection de collisions")]
    public Transform pointDetectionCollision;
    public LayerMask layersCollision;

    TickTimer dureeVieFusee = TickTimer.None;
    List<LagCompensatedHit> infosCollisionsList = new List<LagCompensatedHit>();

    int vitesseFusee = 20;

    PlayerRef lanceur;
    string nomLanceur;
    NetworkObject networkObject;
    NetworkObject lanceurNetworkObject;


    /*
  * Fonction appeler par le script GestionnaireArmes. Elle gère la fusée tout juste après son spawning
  * Paramètres :
  * - PlayerRef lanceur : le joueur qui lance la grenade
  * - NetworkObject lanceurNetworkObject : référence au component NetworkObject du joueur qui lance la fusée
  * - string nomLanceur : le nom du joueur qui lance la grenade
  * 1. On mémorise la référence au component NetworkObject.
  * 2. On mémorise dans des variables : le joueur qui lance la grenade ainsi que son nom et également son component
  * Networkobject. Notez l'utilisation du "this" qui permet de distinguer la variable du paramètre de la fonction qui 
  * porte le même nom.
  * 3.Création d'un timer réseau (TickTimer) d'une durée de 10 secondes
  */
    public void LanceFusee(PlayerRef lanceur, NetworkObject lanceurNetworkObject,  string nomLanceur)
    {
        //1.
        networkObject = GetComponent<NetworkObject>();
        //2.
        this.lanceur = lanceur;
        this.nomLanceur = nomLanceur;
        this.lanceurNetworkObject = lanceurNetworkObject;
        //3.
        dureeVieFusee = TickTimer.CreateFromSeconds(Runner, 10);
    }

/*
    * Fonction qui s'occupe du déplacement et qui gère l'explosion de la fusée et la détection de collision
    * 1.Déplacement de la fusée dans son axe des Z (son devant)
    * 2.Si on est sur le serveur et que le timer de 10 secondes est expiré, le serveur détruit (despawn) la fusée qui explosera.
    * La suite du script s'exécutera seulement si le timer n'est pas expiré
    * 3. Première vérification avec un OverlapSphere tout petit (rayon de 0.5 mètre). L'objectif est d'éviter la détection
    * de proximité du joueur qui tire la fusée lorsqu'il vient juste de tirer la fusée
    * 4. On passe à travers la liste et si on détecte la présence de joueur qui tire, on met la variable contactValide à false
    * 5. On passe à travers la liste des objets détectés par le OverlapSphere. Les objets contenu dans cette liste sont de type
    * LagCompensatedHit qui est propre à Fusion:
    * - On tente de récupérer le component (script) gestionnairePointsDeVie de l'objet touché. Renverra null si l'objet n'en possède pas.
    * - Si l'objet possède un component gestionnairePointsDeVie, c'est qu'il s'agit d'un joueur. On peut alors appeler la fonction
    * PersoEstTouche() présente dans ce script. On envoie en paramètres le nom du lanceur de grenade et le dommage a appliquer
    * 
    * 4.Le serveur fait disparaître (Despawn) la grenade (elle disparaitra sur tous les clients)
    * Pour éviter de répéter plusieurs fois, on s'assure désactive le timer (TickTimer.None)
    */
    public override void FixedUpdateNetwork()
    {
        //1.
        transform.position += transform.forward * Runner.DeltaTime * vitesseFusee;
        //2.
        if(Object.HasStateAuthority)
        {
            if(dureeVieFusee.Expired(Runner))
            {
                Runner.Despawn(networkObject);
                return;
            }
            /*
            int nbObjetsTouches = Runner.LagCompensation.OverlapSphere(pointDetectionCollision.position, 0.5f, lanceur, infosCollisionsList, layersCollision, HitOptions.IncludePhysX);

            //4.
            bool contactValide = false;
            if (nbObjetsTouches > 0)
                contactValide = true;

            foreach (LagCompensatedHit objetTouche in infosCollisionsList)
            {
                if(objetTouche.Hitbox != null)
                {
                    //est-ce qu'on se touche soit même
                    if (objetTouche.Hitbox.Root.GetBehaviour<NetworkObject>() == lanceurNetworkObject)
                        contactValide = true;
                }
            }
            //5.
            if (contactValide)
            { */
               int nbObjetsTouches = Runner.LagCompensation.OverlapSphere(pointDetectionCollision.position, 4f, lanceur, infosCollisionsList, layersCollision, HitOptions.None);
                foreach (LagCompensatedHit objetTouche in infosCollisionsList)
                {
                    GestionnairePointsDeVie gestionnairePointsDeVie = objetTouche.Hitbox.transform.root.GetComponent<GestionnairePointsDeVie>();
                    if (gestionnairePointsDeVie != null)
                        gestionnairePointsDeVie.PersoEstTouche(nomLanceur, 20);
                }
                Runner.Despawn(networkObject);
            //}
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Instantiate(prefabExplosion, pointDetectionCollision.position, Quaternion.identity);
    }
}
