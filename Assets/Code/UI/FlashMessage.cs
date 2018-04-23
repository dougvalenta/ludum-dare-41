using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashMessage : MonoBehaviour {

    public static FlashMessage main { get; protected set; }

    public Image image;
    public Text text;

    public float imageAlpha;
    public float textAlpha;

    public float fadeInSpeed = 1f;
    public float fadeOutSpeed = 5f;

    bool fadingIn;
    bool fadeInDelayed;

	private void Awake()
	{
        main = this;
        gameObject.SetActive(false);
	}

    public void FadeIn(string message, float delay) {
        gameObject.SetActive(true);
        StartCoroutine(DoFadeIn(message, delay));
    }

    public void FadeOut() {
        fadingIn = false;
        fadeInDelayed = false;
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DoFadeOut());
        }
    }

    IEnumerator DoFadeIn(string message, float delay) {
        if (delay > 0f) {
            fadeInDelayed = true;
            yield return new WaitForSeconds(delay);
            if (!fadeInDelayed) {
                yield break;
            }
        }
        fadingIn = true;
        text.text = message;
        Color imageColor = image.color;
        Color textColor = text.color;
        Color targetImageColor = imageColor;
        Color targetTextColor = textColor;
        targetImageColor.a = imageAlpha;
        targetTextColor.a = textAlpha;
        float t = Time.deltaTime * fadeInSpeed;
        while (t < 1f && fadingIn) {
            image.color = Color.Lerp(imageColor, targetImageColor, t);
            text.color = Color.Lerp(textColor, targetTextColor, t);
            yield return null;
            t += Time.deltaTime * fadeInSpeed;
        }
        if (fadingIn) {
            image.color = targetImageColor;
            text.color = targetTextColor;
            fadingIn = false;
        }
    }

    IEnumerator DoFadeOut() {
        Color imageColor = image.color;
        Color textColor = text.color;
        Color targetImageColor = image.color;
        Color targetTextColor = text.color;
        targetImageColor.a = 0f;
        targetTextColor.a = 0f;
        float t = Time.deltaTime * fadeOutSpeed;
        while (!fadingIn) {
            image.color = Color.Lerp(imageColor, targetImageColor, t);
            text.color = Color.Lerp(textColor, targetTextColor, t);
            yield return null;
            t += Time.deltaTime * fadeOutSpeed;
        }
        if (!fadingIn) {
            image.color = targetImageColor;
            text.color = targetTextColor;
            gameObject.SetActive(false);
        }
    }

}
