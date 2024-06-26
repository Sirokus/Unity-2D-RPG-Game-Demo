using UnityEngine;

public class AfterImageFx : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    private float colorLooseRate;

    public void setup(Sprite sprite, float colorLooseRate)
    {
        sr.sprite = sprite;
        this.colorLooseRate = colorLooseRate;
    }

    private void Update()
    {
        Color color = sr.color;
        color.a -= colorLooseRate * Time.deltaTime;
        sr.color = color;

        if (color.a <= 0)
            Destroy(gameObject);
    }
}
