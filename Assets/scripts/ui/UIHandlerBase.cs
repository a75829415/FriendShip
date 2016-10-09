using UnityEngine;

public class UIHandlerBase : MonoBehaviour
{
    public RectTransform currentPanel;

    public virtual void ShowGUI(bool showGUI)
    {
        currentPanel.gameObject.SetActive(showGUI);
    }
}
