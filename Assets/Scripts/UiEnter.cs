using UnityEngine;
using UnityEngine.EventSystems;

public class UiEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string nameItem;
    public ChestScript chestScript;
    public void OnPointerEnter(PointerEventData eventData)
    {
        chestScript.ShowName(nameItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chestScript.StopShowName();
    }
}
