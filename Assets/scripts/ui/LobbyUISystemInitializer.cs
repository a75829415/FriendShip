using UnityEngine;

public class LobbyUISystemInitializer : MonoBehaviour
{
    public static RectTransform currentActivePanel;

    public RectTransform[] panels;

    void Start()
    {
        foreach (RectTransform panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
        if (currentActivePanel != null)
        {
            currentActivePanel.gameObject.SetActive(true);
        }
    }
}
