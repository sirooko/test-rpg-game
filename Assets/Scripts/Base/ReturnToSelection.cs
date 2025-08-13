using UnityEngine;

public class ReturnToSelection : MonoBehaviour
{
    public GameObject currentPanel;
    public GameObject selectionPanel;

    public void GoBack()
    {
        if (currentPanel != null) currentPanel.SetActive(false);
        if (selectionPanel != null) selectionPanel.SetActive(true);
    }
}
