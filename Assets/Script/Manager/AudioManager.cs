using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager ins;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    private int curBgmIndex = 0;

    public bool playBgm;
    public float sfxMinimumDistance;

    private void Awake()
    {
        if (ins != null)
        {
            Destroy(this);
            return;
        }
        ins = this;
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBgm();
        else if (!bgm[curBgmIndex].isPlaying)
            PlayRandomBGM();
    }

    public void PlaySFX(int index, Vector2? _source = null)
    {
        if (index < sfx.Length)
        {
            if (_source != null && Vector2.Distance(PlayerManager.playerPos, _source.Value) > sfxMinimumDistance)
                return;

            sfx[index].pitch = Random.Range(.8f, 1.2f);
            sfx[index].Play();
        }
    }

    public void StopSFX(int index)
    {
        if (index < sfx.Length)
        {
            sfx[index].Stop();
        }
    }

    public void PlaySFXFadeIn(int index)
    {
        if (index < sfx.Length)
        {
            StartCoroutine(IncreaseVolume(sfx[index]));
        }
    }

    public void StopSFXFadeOut(int index)
    {
        if (index < sfx.Length)
        {
            StartCoroutine(DecreaseVolume(sfx[index]));
        }
    }

    IEnumerator IncreaseVolume(AudioSource audio)
    {
        audio.Play();
        audio.volume = 0f;
        while (audio.volume < 1f)
        {
            audio.volume += .2f;
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator DecreaseVolume(AudioSource audio)
    {
        float defaultVolume = audio.volume;

        while (audio.volume > .1f)
        {
            audio.volume -= .2f;
            yield return new WaitForSeconds(.05f);

            if (audio.volume <= .1f)
            {
                audio.Stop();
                audio.volume = defaultVolume;
                break;
            }
        }
    }

    public void PlayBGM(int index, bool stopLastBgm = true)
    {
        if (index < bgm.Length)
        {
            if (stopLastBgm)
                bgm[curBgmIndex].Stop();

            bgm[index].Play();
            curBgmIndex = index;
        }
    }

    public void PlayRandomBGM()
    {
        bgm[curBgmIndex].Stop();
        curBgmIndex = Random.Range(0, bgm.Length);
        bgm[curBgmIndex].Play();
    }

    public void StopAllBgm()
    {
        for (int i = 0; i <  bgm.Length; i++)
            bgm[i].Stop();
    }
}
