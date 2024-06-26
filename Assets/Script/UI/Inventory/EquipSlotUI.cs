using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public EquipmentType type;
    [SerializeField] private Image itemImage;
    private Sprite nullSprite;

    public ItemData item;

    private void Awake()
    {
        nullSprite = itemImage.sprite;
    }

    public void Equip(ItemData item)
    {
        if (item != null)
        {
            this.item = item;
            itemImage.sprite = item.itemIcon;
            itemImage.color = Color.white;
        }
    }
    public void UnEquip()
    {
        if (item == null)
            return;

        Inventory.instance.equipmentDictionary.Remove(type);

        Inventory.AddItem(item);
        ((EquipmentData)item).RemoveModifiers();

        item = null;
        itemImage.sprite = nullSprite;
        itemImage.color = Color.black;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UnEquip();
        OnPointerExit(null);
    }

    Timer timer;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
            return;

        Inventory.instance.infoUITracePointer = true;
        Inventory.instance.itemInfoUI.UpdateUI(item);
        timer = TimerManager.addTimer(.1f, false, () =>
        {
            Inventory.instance.itemInfoUI.gameObject.SetActive(true);
            timer = null;
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (timer != null)
        {
            TimerManager.clearTimer(timer);
            timer = null;
        }
        Inventory.instance.itemInfoUI.gameObject.SetActive(false);
        Inventory.instance.infoUITracePointer = false;
    }
}
