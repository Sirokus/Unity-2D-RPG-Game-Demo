using UnityEngine;
using UnityEngine.UI.Extensions;

public class SkillTreeLine : MonoBehaviour
{
    public UILineRenderer line;

    public void setupLine(RectTransform start, RectTransform end)
    {
        line.Points = new Vector2[]
        {
            start.anchoredPosition,
            end.anchoredPosition
        };
        transform.SetAsFirstSibling();
    }
}
