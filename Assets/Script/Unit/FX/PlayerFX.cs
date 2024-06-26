using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("Screen shake Fx")]
    private CinemachineImpulseSource screenShake;
    [SerializeField] private float shakeMultiplier;
    [SerializeField] public Vector3 shakeSwordPower;
    [SerializeField] public Vector3 shakeHighDamage;

    [Header("After image Fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    [SerializeField] private float afterImageCooldownTimer;

    [Header("Hit Fx")]
    [SerializeField] private ParticleSystem dustFx;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        screenShake = GetComponent<CinemachineImpulseSource>();
    }

    protected override void Update()
    {
        base.Update();

        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void ScreenShake(Vector3 shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(shakePower.x * PlayerManager.GetPlayer().dir, shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject obj = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            obj.GetComponent<AfterImageFx>().setup(sr.sprite, colorLooseRate);
        }
    }

    public void PlayDustFx()
    {
        dustFx.Play();
    }
}
