using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class JoueurReseau : NetworkBehaviour, IPlayerLeft
{
    public static JoueurReseau Local;

    public override void Spawned()
    {
        if(Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Un joueur local a été créé");
        }
        else
        {
            Debug.Log("Un joueur réseau a été créé");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

}
