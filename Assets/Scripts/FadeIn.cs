using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class FadeIn : MonoBehaviour {
    public Volume volume;
    private float targetExposure = 0f; // Normal exposure
    public float fadeDuration = 10f;

    void Start() {
        if (volume.profile.TryGet(out ColorAdjustments colorAdjustments)) {
            StartCoroutine(FadeInFromBlack(colorAdjustments));
        }
    }

    IEnumerator FadeInFromBlack(ColorAdjustments colorAdjustments) {
        float elapsedTime = 0f;
        float initialExposure = colorAdjustments.postExposure.value;
        while (elapsedTime < fadeDuration) {
            colorAdjustments.postExposure.value = Mathf.Lerp(initialExposure, targetExposure, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        colorAdjustments.postExposure.value = targetExposure;
    }
}