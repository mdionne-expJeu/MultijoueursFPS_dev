using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GestionnaireGrenade : NetworkBehaviour
{
    [Header("Prefab")]
    public GameObject prefabExplosion;

    [Header("Détection de collisions")]
    public LayerMask layersCollision;

    NetworkObject networkObject;
    NetworkRigidbody networkRigidbody;

    PlayerRef lanceur;
    string nomLanceur;

    TickTimer timerExplosion = TickTimer.None;
    List<LagCompensatedHit> infosCollisionsList = new List<LagCompensatedHit>();
    
 
    public void LanceGrenade(Vector3 forceDuLance, PlayerRef lanceur, string nomLanceur)
    {

        networkObject = GetComponent<NetworkObject>();
        networkRigidbody = GetComponent<NetworkRigidbody>();

        networkRigidbody.Rigidbody.AddForce(forceDuLance, ForceMode.Impulse);

        this.lanceur = lanceur;
        this.nomLanceur = nomLanceur;

        timerExplosion = TickTimer.CreateFromSeconds(Runner, 2);
    }

    public override void FixedUpdateNetwork()
    {
        if(Object.HasStateAuthority)
        {
            if(timerExplosion.Expired(Runner))
            {
                //vérification des joueurs à proximité
                int nbJoueursTouche = Runner.LagCompensation.OverlapSphere(transform.position, 4, lanceur, infosCollisionsList, layersCollision);

                print(infosCollisionsList.Count);
                foreach(LagCompensatedHit objetTouche in infosCollisionsList)
                {
                    GestionnairePointsDeVie gestionnairePointsDeVie = objetTouche.Hitbox.transform.root.GetComponent<GestionnairePointsDeVie>();
                    if (gestionnairePointsDeVie != null)
                        gestionnairePointsDeVie.PersoEstTouche(nomLanceur,10);
                }
          
                //despawn et explosion
                Runner.Despawn(networkObject);
                timerExplosion = TickTimer.None; // pour éviter de boucler
            }
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        MeshRenderer meshGrenade = GetComponentInChildren<MeshRenderer>(); // pour éviter un décalage
        Instantiate(prefabExplosion, meshGrenade.transform.position, Quaternion.identity);
    }
}
