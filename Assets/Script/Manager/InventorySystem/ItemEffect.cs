using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/Effect")]
public class ItemEffect : ScriptableObject
{
    public bool dontShowTips;

    public virtual bool conditionCheck()
    {
        return true;
    }

    public virtual void execute(Transform target)
    {

    }
}
