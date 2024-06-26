using UnityEngine;

//生成的剑的状态，正常，可反弹，可穿刺
public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType;

    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Peirce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private int maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity;
    [SerializeField] private float hitCoolDown = .35f;

    [Header("Skill Info")]
    [SerializeField] private GameObject SwordPrefab;
    [SerializeField] private Vector2 LaunchForce;
    [SerializeField] private float GravityScale;
    [SerializeField] private float freezeTimeDuration = .3f;
    private float defaultGravity;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject[] dots;
    [SerializeField] private Transform dotsParent;

    protected override void Start()
    {
        base.Start();

        GenerateDots();

        defaultGravity = GravityScale;

        SetupGravity();
    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Regular)
        {
            GravityScale = defaultGravity;
        }
        else if (swordType == SwordType.Bounce)
        {
            GravityScale = bounceGravity;
        }
        else if (swordType == SwordType.Pierce)
        {
            GravityScale = pierceGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            GravityScale = spinGravity;
        }
    }

    protected override void Update()
    {
        base.Update();

        SetupGravity();

        //追踪抬起输入，抬起后将会把瞄准的方向设置为最终方向供生成函数调用
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Vector2 aimDir = AimDirection().normalized;
            finalDir = new Vector2(aimDir.x * LaunchForce.x, aimDir.y * LaunchForce.y);
        }

        //如果按住键则会按照公式持续计算每个点的位置
        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }

        if (Input.anyKeyDown) SetupGravity();
        if (Input.GetKeyDown(KeyCode.Alpha1)) swordType = SwordType.Regular;
        if (Input.GetKeyDown(KeyCode.Alpha2)) swordType = SwordType.Bounce;
        if (Input.GetKeyDown(KeyCode.Alpha3)) swordType = SwordType.Pierce;
        if (Input.GetKeyDown(KeyCode.Alpha4)) swordType = SwordType.Spin;
    }

    //创建剑，调用时机为玩家投掷动画的播出过程中
    public void CreateSword()
    {
        //创建剑的预制体，并获得其Controller
        GameObject newSword = Instantiate(SwordPrefab, player.transform.position, player.transform.rotation);
        Sword_Skill_Controller controller = newSword.GetComponent<Sword_Skill_Controller>();

        //若生成状态等于反弹，则执行相关逻辑
        if (swordType == SwordType.Regular)
        {
            GravityScale = defaultGravity;
        }
        else if (swordType == SwordType.Bounce)
        {
            GravityScale = bounceGravity;
            controller.SetupBounce(true, bounceAmount);
        }
        else if (swordType == SwordType.Pierce)
        {
            GravityScale = pierceGravity;
            controller.SetupPierce(pierceAmount);
        }
        else if (swordType == SwordType.Spin)
        {
            GravityScale = spinGravity;
            controller.SetupSpin(true, maxTravelDistance, spinDuration, hitCoolDown);
        }

        //为剑配置所需的基础初始化参数
        controller.SetupSword(finalDir, GravityScale, player, freezeTimeDuration);
        //将剑分配到玩家
        player.AssignNewSword(newSword);
        //将点设置为不可见
        SetDotsActive(false);
    }

    //瞄准的方向（非标准化，即包含距离信息）
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    //便捷设置所有点的激活状态
    public void SetDotsActive(bool isActive)
    {
        foreach (var dot in dots)
        {
            dot.SetActive(isActive);
        }
    }

    //初始化点的数组，调用时机为本skill的Start函数中
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    //实时根据传入的t进行重力公式下的位置计算
    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
                                                                    AimDirection().normalized.x * LaunchForce.x,
                                                                    AimDirection().normalized.y * LaunchForce.y) * t + .5f * (Physics2D.gravity * GravityScale) * (t * t);
        return position;
    }
}
