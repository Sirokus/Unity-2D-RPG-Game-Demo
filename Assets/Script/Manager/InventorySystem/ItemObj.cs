using UnityEngine;

public class ItemObj : MonoBehaviour, IPoolable
{
    private SpriteRenderer sr;

    [SerializeField] private Collider2D TriggerRange;

    [SerializeField] public ItemData data;
    public int stackSize = 1;

    public bool openColliderWhenStart = false;

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = data.itemIcon;
        gameObject.name = "Item object - " + data.itemName;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = data.itemIcon;

        TriggerRange.enabled = false;
    }

    Timer enableTimer = null;
    Timer lifeTimer;

    public EPoolObjState state { get; set; }

    private void Start()
    {
        if (openColliderWhenStart)
        {
            TriggerRange.enabled = true;
        }
    }

    public void OnGet()
    {
        enableTimer = TimerManager.addTimer(.5f, false, () => { TriggerRange.enabled = true; enableTimer = null; });
        lifeTimer = TimerManager.addTimer(15f, false, () => { ObjectPool.Return(this); lifeTimer = null; });
    }

    public void OnReturn()
    {
    }

    public void Setup(ItemData data, int stackSize = 1)
    {
        this.data = data;
        this.stackSize = stackSize;
        GetComponent<SpriteRenderer>().sprite = data.itemIcon;
        gameObject.name = "Item object - " + data.itemName;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.ins.PlaySFX(18);
        onTriggerEnter2D(collision);
    }

    protected virtual void onTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerStat player))
        {
            if (!player.isAlive())
                return;

            TimerManager.clearTimer(lifeTimer);
            Inventory.AddItem(data, stackSize);

            if (openColliderWhenStart)
                Destroy(gameObject);
            else
                ObjectPool.Return(this);
        }
    }
}
