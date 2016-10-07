using UnityEngine;
using UnityEngine.UI;

public class PopupUIHandler : MonoBehaviour
{
    public static PopupUIHandler instance;

    public RectTransform popupPanel;
    public Text popupText;
    public Button okButton;

    void Awake()
    {
        instance = this;
    }

    public void Popup(string message, bool interactable = true)
    {
        popupText.text = message;
        popupPanel.gameObject.SetActive(true);
        okButton.interactable = interactable;
    }

    // ---- UI event handlers ----
    public void OnOkButtonClick()
    {
        popupPanel.gameObject.SetActive(false);
    }
}
