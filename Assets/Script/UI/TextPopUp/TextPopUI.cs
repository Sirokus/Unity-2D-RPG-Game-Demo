using DG.Tweening;
using TMPro;
using UnityEngine;



public class TextPopUI : MonoBehaviour, IPoolable
{
    public TextMeshProUGUI numTxt;
    Tweener tweener;

    public EPoolObjState state { get; set; }

    public void OnGet()
    {
        transform.SetParent(UIManager.ins.popTextParent, false);
        ObjectPool.Return(this, true);
    }

    public void OnReturn()
    {
        tweener?.Kill();
    }

    public void Setup(Vector2 startPos, string content, EPopTextType type)
    {
        TextPopSO so = UIManager.ins.GetTextPopSOByType(type);

        startPos = Camera.main.WorldToScreenPoint(startPos);

        startPos += so.startPosOffset;
        startPos.x += Random.Range(-so.startPosRandomRange.x, so.startPosRandomRange.x);
        startPos.y += Random.Range(-so.startPosRandomRange.y, so.startPosRandomRange.y);

        Vector2 endPos = so.endPos;
        endPos.x += Random.Range(-so.endPosRandomRange.x, so.endPosRandomRange.x);
        endPos.y += Random.Range(-so.endPosRandomRange.y, so.endPosRandomRange.y);
        endPos.x *= Random.Range(0, 2) == 0 ? 1 : -1;
        endPos += startPos;

        numTxt.text = content;
        numTxt.color = so.color;
        transform.localScale = so.size;

        transform.position = startPos;
        tweener = DOTween.To(value =>
        {
            transform.position = Parabola(startPos, endPos, so.height, value);
        }, 0, 1, so.duration)
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
            tweener = null;
            ObjectPool.Return(this);
        });
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        //计算标准化范围0~1内对应值的高度(0~height)，在x=0.5时到达最大值，二次函数
        float CalculateDeltaHeight(float x) => 4 * (-height * x * x + height * x);

        //获取当前直线位移到目标位置的差值位置
        var cur = Vector2.Lerp(start, end, t);

        //构造结果
        return new Vector2(cur.x, cur.y + CalculateDeltaHeight(t));
    }
}
