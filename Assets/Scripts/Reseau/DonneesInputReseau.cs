using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct DonneesInputReseau : INetworkInput
{
    public Vector2 mouvementInput;
    public float rotationInput;
    public NetworkBool saute;
}