using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Slider slider;
    public float fillTime = 3f;

    public void StartProgress(float duration, System.Action onFinish)
    {
        gameObject.SetActive(true);
        StartCoroutine(Fill(duration, onFinish));
    }

    private IEnumerator Fill(float duration, System.Action onFinish)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            slider.value = timer / duration;
            yield return null;
        }

        onFinish?.Invoke();
    }
}
