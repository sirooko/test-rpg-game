using UnityEngine;

public class SelectionUIManager : MonoBehaviour
{
    public GameObject castleDoorPanel;
    public GameObject summonPortalPanel;
    public GameObject goddessStatuePanel;
    public GameObject demonicMerchantPanel;
    public GameObject storagePanel;
    public GameObject castlePanel;
    public GameObject trainingRoomPanel;
    public GameObject restRoomPanel;
    public GameObject pubPanel;
    public GameObject secretGardenPanel;

    public void OpenPanel(string panelName)
    {
        CloseAllPanels();

        switch (panelName)
        {
            case "castleDoor":
                castleDoorPanel.SetActive(true);
                break;
            case "summonPortal":
                summonPortalPanel.SetActive(true);
                break;
            case "goddessStatue":
                goddessStatuePanel.SetActive(true);
                break;
            case "demonicMerchant":
                demonicMerchantPanel.SetActive(true);
                break;
            case "storage":
                storagePanel.SetActive(true);
                break;
            case "castle":
                castlePanel.SetActive(true);
                break;
            case "trainingRoom":
                trainingRoomPanel.SetActive(true);
                break;
            case "restRoom":
                restRoomPanel.SetActive(true);
                break;
            case "pub":
                pubPanel.SetActive(true);
                break;
            case "secretGarden":
                secretGardenPanel.SetActive(true);
                break;
        }
    }

    void CloseAllPanels()
    {
        castleDoorPanel.SetActive(false);
        summonPortalPanel.SetActive(false);
        goddessStatuePanel.SetActive(false);
        demonicMerchantPanel.SetActive(false);
        storagePanel.SetActive(false);
        castlePanel.SetActive(false);
        trainingRoomPanel.SetActive(false);
        restRoomPanel.SetActive(false);
        pubPanel.SetActive(false);
        secretGardenPanel.SetActive(false);
    }
}
