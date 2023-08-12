using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
/*
 * Ce script n'est pas une classe, mais bien une structure de données (struct)
 * Dérive de INetworkInput qui est une interface de Fusion
 * 
 * Permet de mémoriser des valeurs avec de variables
 * mouvementInput : un vector2 qui servira au déplacement
 * rotatioIput : un float qui servira à faire pivoter le personnage
 * saute : un booleenne pour savoir le si personnage saute
 * Notez l'utilisation du type NetworkBool qui est une variable réseau qui sera automatiquement synchronisée
 * pour tous les clients
 */
public struct DonneesInputReseau : INetworkInput
{
    public Vector2 mouvementInput;
    public float rotationInput;
    public NetworkBool saute;
}