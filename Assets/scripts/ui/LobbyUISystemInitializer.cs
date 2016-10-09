using UnityEngine;

public class LobbyUISystemInitializer : MonoBehaviour
{
    public static LobbyUISystemInitializer instance;

    private static uint indexOfPanelToShow = 0;

    public RectTransform[] panels;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (NetHub.instance != null)
        {
            Destroy(NetHub.instance.gameObject);
        }
        foreach (RectTransform panel in panels)
        {
            panel.gameObject.SetActive(false);
        }
        panels[indexOfPanelToShow].gameObject.SetActive(true);
    }

    public void SetPanelToShow(RectTransform panel)
    {
        for (uint i = 0; i < panels.Length; i++)
        {
            if (panel == panels[i])
            {
                indexOfPanelToShow = i;
                return;
            }
        }
    }
}
