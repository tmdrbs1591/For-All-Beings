using UnityEngine;
using UnityEngine.EventSystems;

public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject explanationPanel;

    // Called when the pointer enters the button's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Activate the explanation panel when the button is highlighted
        if (explanationPanel != null)
        {
            explanationPanel.SetActive(true);
        }
    }

    // Called when the pointer exits the button's area
    public void OnPointerExit(PointerEventData eventData)
    {
        // Deactivate the explanation panel when the button is no longer highlighted
        if (explanationPanel != null)
        {
            explanationPanel.SetActive(false);
        }
    }
}
