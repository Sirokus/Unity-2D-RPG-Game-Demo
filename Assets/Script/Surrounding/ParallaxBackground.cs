using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cam;

    [SerializeField] private Vector2 ParallaxEffect;

    private Vector2 position;
    private float length;

    [SerializeField] private bool cameraChild;

    // Start is called before the first frame update
    void Start()
    {
        ChangeCamera(cam);

        //背景的x轴长度
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float xMove = position.x + cam.position.x * ParallaxEffect.x;
        float yMove = position.y + cam.position.y * ParallaxEffect.y;
        transform.position = new Vector2(xMove, yMove);

        float distanceMoved = cam.position.x * (1 - ParallaxEffect.x);

        if (distanceMoved > position.x + length)
        {
            position.x = position.x + length;
        }
        else if (distanceMoved < position.x - length)
        {
            position.x = position.x - length;
        }

    }

    public void ChangeCamera(Transform cam)
    {
        this.cam = cam;

        if (cameraChild)
            transform.SetParent(cam, false);

        Vector2 Offset = new Vector2();
        Offset.x -= cam.position.x * ParallaxEffect.x;
        Offset.y -= cam.position.y * ParallaxEffect.y;

        //默认位置
        position = (Vector2)transform.position + Offset;
    }
}
