public class Enemy_Chest : Enemy
{
    bool isOpen = false;
    public override void Die()
    {
        base.Die();

        if (isOpen)
            return;

        anim.SetTrigger("Open");
        isOpen = true;
    }

    public override void SaveData(GameData data)
    {
        base.SaveData(data);

        data.addValue(id + "isOpen", isOpen);
    }

    public override void LoadData(GameData data)
    {
        base.LoadData(data);

        if (data.haveValueb(id + "isOpen"))
        {
            if (data.getValueb(id + "isOpen"))
            {
                anim.SetTrigger("Open");
                isOpen = true;
                GetComponent<EnemyStat>().DropNum = 0;
                GetComponent<EnemyStat>().DecreseHealth(9999);
            }
        }
    }
}
