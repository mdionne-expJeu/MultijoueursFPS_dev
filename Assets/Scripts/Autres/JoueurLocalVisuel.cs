using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class JoueurLocalVisuel : NetworkBehaviour
{
    public Material[] listeMatVisuel;

    /*[Networked] public int JoueurBleu  { get; set; }
    [Networked] public  int JoueurRouge { get; set; }
    [Networked] public  int JoueurVert { get; set; }
    [Networked] public  int JoueurNoir { get; set; }*/

    void Start()
    {
        if(!Object.HasInputAuthority)
        GetComponent<MeshRenderer>().material = listeMatVisuel[Random.Range(0, listeMatVisuel.Length - 1)];

        /*if (JoueurBleu == 0)
        {
            GetComponent<MeshRenderer>().material = listeMatVisuel[0];
            JoueurBleu = 1;
        }

        if (JoueurRouge == 0)
        {
            GetComponent<MeshRenderer>().material = listeMatVisuel[1];
            JoueurRouge = 1;
        }

        if (JoueurVert == 0)
        {
            GetComponent<MeshRenderer>().material = listeMatVisuel[2];
            JoueurVert = 1;
        }

        if (JoueurNoir == 0)
        {
            GetComponent<MeshRenderer>().material = listeMatVisuel[3];
            JoueurNoir = 1;
        }*/
    }
        



}
