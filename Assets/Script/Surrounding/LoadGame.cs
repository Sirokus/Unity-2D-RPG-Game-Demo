using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    public AsyncOperation ao;
    public Slider slider;

    public void Setup(string levelName, int slotIndex)
    {
        StartCoroutine(LoadAsync(levelName, slotIndex));
    }

    IEnumerator LoadAsync(string levelName, int slotIndex)
    {
        slider.value = 0;

        ao = SceneManager.LoadSceneAsync(levelName);
        ao.allowSceneActivation = false;

        while (ao.progress <= 0.85)
        {
            yield return new WaitForEndOfFrame();
            slider.value += 0.1f;
        }

        slider.value = 1;
        yield return new WaitForSeconds(0.3f);
        ao.allowSceneActivation = true;

        Debug.Log("¼ÓÔØÍê³É");
    }
}
