using UnityEngine;

public enum EPopTextType
{
    normal,
    damage,
    critDamage,
    Heal
}

[CreateAssetMenu(fileName = "NewTextPopData", menuName = "Data/UI/TextPop")]
public class TextPopSO : ScriptableObject
{
    public EPopTextType popTextType = EPopTextType.normal;
    public Color color = Color.white;
    public Vector2 size = new Vector2(1, 1);

    public Vector2 startPosOffset, endPos;
    public Vector2 startPosRandomRange, endPosRandomRange;
    public float height, duration;
}
