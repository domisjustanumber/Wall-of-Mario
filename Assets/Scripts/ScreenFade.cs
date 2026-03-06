using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    private static ScreenFade _instance;
    public static ScreenFade Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ScreenFade");
                _instance = go.AddComponent<ScreenFade>();
            }
            return _instance;
        }
    }

    private Canvas _canvas;
    private Image _image;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        CreateOverlay();
    }

    private void CreateOverlay()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 9999;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();

        var imageGo = new GameObject("FadeImage");
        imageGo.transform.SetParent(transform, false);
        _image = imageGo.AddComponent<Image>();
        _image.color = new Color(0f, 0f, 0f, 0f);
        _image.raycastTarget = false;
        var rect = _image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public IEnumerator FadeToBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(elapsed / duration);
            _image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        _image.color = new Color(0f, 0f, 0f, 1f);
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(elapsed / duration);
            _image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        _image.color = new Color(0f, 0f, 0f, 0f);
    }
}
