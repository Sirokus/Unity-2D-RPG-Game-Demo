using UnityEngine;

public class ShadowSprite_OP : MonoBehaviour, IPoolable
{
    private Transform player;

    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;

    private Color color;

    [Header("不透明度控制")]
    private float alpha;
    public float startAlpha;
    public float alphaMultiplier;

    EPoolObjState IPoolable.state { get; set; }

    public void OnGet()
    {
        player = PlayerManager.GetPlayer().transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponentInChildren<SpriteRenderer>();

        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        thisSprite.sprite = playerSprite.sprite;

        alpha = startAlpha;
    }

    public void OnReturn()
    {
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        Color color = thisSprite.color;
        color.a = alpha;
        thisSprite.color = color;

        if (alpha <= 0.1f)
        {
            ObjectPool.Return(this);
        }
    }
}
