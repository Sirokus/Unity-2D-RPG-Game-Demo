using System.Collections;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    protected SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material HitMat;
    private Material OriginalMat;

    [Header("Ailment colors")]
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment Particles")]
    [SerializeField] public ParticleSystem igniteFx;
    [SerializeField] public ParticleSystem chillFx;
    [SerializeField] public ParticleSystem shockFx;

    [Header("UI")]
    public GameObject healthBarUI;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        OriginalMat = sr.material;
    }

    protected virtual void Update()
    {
    }

    private IEnumerator FlashFX()
    {
        sr.material = HitMat;

        yield return new WaitForSeconds(.2f);

        sr.material = OriginalMat;
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }

    public void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;

        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    public void IgniteFXFor(float duration)
    {
        igniteFx.Play();
        TimerManager.addTimer(duration, false, () => igniteFx.Stop());
        InvokeRepeating("IgniteColorFX", 0, .3f);
        Invoke("CancelColorChange", duration);
    }

    private void IgniteColorFX()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    public void ShockFXFor(float duration)
    {
        shockFx.Play();
        TimerManager.addTimer(duration, false, () => shockFx.Stop());
        InvokeRepeating("ShockColorFX", 0, .1f);
        Invoke("CancelColorChange", duration);
    }
    private void ShockColorFX()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void ChillFXFor(float duration)
    {
        chillFx.Play();
        TimerManager.addTimer(duration, false, () => chillFx.Stop());

        sr.color = chillColor;
        Invoke("CancelColorChange", duration);
    }

    public void setVisibilityEffect(bool visibility)
    {
        healthBarUI.SetActive(visibility);
        igniteFx.gameObject.SetActive(visibility);
        chillFx.gameObject.SetActive(visibility);
        shockFx.gameObject.SetActive(visibility);
    }

    public void playParticleFX(bool ignite, bool chill, bool shock)
    {
        if (ignite)
            igniteFx.Play();
        if (chill)
            chillFx.Play();
        if (shock)
            shockFx.Play();
    }
}
