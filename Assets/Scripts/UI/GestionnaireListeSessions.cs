using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using TMPro;

public class GestionnaireListeSessions : MonoBehaviour
{
    public TextMeshProUGUI txtEtat;
    public GameObject ListeItemSessionPrefab;
    public VerticalLayoutGroup verticalLayoutGroup;

    public void EffaceListe()
    {
        foreach(Transform elementListe in verticalLayoutGroup.transform)
        {
            Destroy(elementListe.gameObject);
        }

        txtEtat.gameObject.SetActive(false);
    }

    public void AjouteListe(SessionInfo sessionInfo)
    {
        GameObject nouvelItemListe = Instantiate(ListeItemSessionPrefab, verticalLayoutGroup.transform);
        InfoListeSessionItemUI nouveauInfoListeSessionItemUI = nouvelItemListe.GetComponent<InfoListeSessionItemUI>();

        nouveauInfoListeSessionItemUI.EnregistreInfos(sessionInfo);

        nouveauInfoListeSessionItemUI.OnRejoindreSession += NouveauInfoListeSessionItemUI_OnRejoindreSession;
    }

    private void NouveauInfoListeSessionItemUI_OnRejoindreSession(SessionInfo obj)
    {

    }

    public void AucuneSessionTrouvee()
    {
        txtEtat.text = "Aucune session de jeu trouvée";
        txtEtat.gameObject.SetActive(true);
    }

    public void ChercheDesSessions()
    {
        txtEtat.text = "Recherche de sessions de jeu";
        txtEtat.gameObject.SetActive(true);
    }
}
