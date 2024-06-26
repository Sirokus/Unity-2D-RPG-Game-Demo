using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public string ID;
    private Animator anim;

    public bool activated;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkPoint ID")]
    private void GenerateCheckPointID()
    {
        ID = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated)
            return;

        if (collision.TryGetComponent(out Player player))
        {
            ActivateCheckPoint();
            AudioManager.ins.PlaySFX(5);
            TipsUI.AddTip("重生点已激活！", false);
            GameManager.ins.lastCheckPoint = this;
            SaveManager.ins.gameDatas[0].vectorDictionary["Player"] = new SerVector3(transform.position);
            SaveManager.AutoSave();
        }
    }

    public void ActivateCheckPoint()
    {
        anim.SetBool("active", true);
        activated = true;
    }
}
