using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SplashScreenManager : MonoBehaviour
{
    [Header("═══════════════════════════════════════")]
    [Header("         SPLASH SCREEN SETTINGS        ")]
    [Header("═══════════════════════════════════════")]

    [Space(10)]
    [Header("⏱️ Time Settings")]
    [Range(0.1f, 3f)] public float fadeInDuration = 1f;
    [Range(0f, 5f)] public float holdDuration = 1.5f;
    [Range(0.1f, 3f)] public float fadeOutDuration = 0.5f;

    [Space(10)]
    [Header("🎬 Animation Settings")]
    public AnimType enterAnimation = AnimType.FadeIn;
    public AnimType exitAnimation = AnimType.FadeOut;

    [Space(10)]
    [Header("🖼️ Visual Settings")]
    public Sprite backgroundSprite;
    public Color backgroundColor = Color.black;
    public bool useBackgroundImage = false;

    [Space(5)]
    public Sprite logoSprite;
    public Vector2 logoSize = new Vector2(400, 400);
    public Color logoColor = Color.white;
    public bool showLogo = true;

    [Space(10)]
    [Header("📊 Loading Bar Settings")]
    [Tooltip("Tampilkan loading bar?")]
    public bool showLoadingBar = true;

    [Tooltip("Warna bar yang terisi")]
    public Color barFillColor = new Color(0.2f, 0.8f, 0.4f, 1f);

    [Tooltip("Warna latar belakang bar")]
    public Color barBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Space(10)]
    [Header("🚀 Target Scene Settings")]
#if UNITY_EDITOR
    public SceneAsset targetSceneAsset;
#endif
    public string nextScene = "MainMenu";

    // ============================================
    // ENUM
    // ============================================
    public enum AnimType
    {
        None, FadeIn, FadeOut, ScaleUp, ScaleDown,
        SlideFromBottom, SlideFromTop, SlideFromLeft, SlideFromRight,
        Bounce, RotateIn, ZoomBounce
    }

    // ============================================
    // PRIVATE VARIABLES
    // ============================================
    private Image bgImage;
    private Image logoImg;
    private CanvasGroup canvasGroup;
    private Slider loadingSlider;
    private Image sliderFillImg;
    private Image sliderBgImg;

    // Posisi awal untuk animasi slide
    private Vector2 slideStartPos;

    // ============================================
    // UNITY LIFECYCLE
    // ============================================
    void Awake()
    {
        InitComponents();
        ApplyVisuals();
    }

    void Start()
    {
        if (!IsSceneInBuildSettings(nextScene))
        {
            Debug.LogError($"[SplashScreen] Scene '{nextScene}' TIDAK ada di Build Settings!");
            return;
        }
        StartCoroutine(PlaySequence());
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (targetSceneAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(targetSceneAsset);
            nextScene = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        if (!Application.isPlaying)
        {
            InitComponents();
            ApplyVisuals();
        }
    }
#endif

    // ============================================
    // INITIALIZATION
    // ============================================
    private void InitComponents()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Transform bgT = transform.Find("Background");
        if (bgT != null) bgImage = bgT.GetComponent<Image>();

        Transform logoT = transform.Find("Logo");
        if (logoT != null) logoImg = logoT.GetComponent<Image>();

        // Cari Loading Bar
        Transform barT = transform.Find("LoadingBar");
        if (barT != null)
        {
            loadingSlider = barT.GetComponent<Slider>();
            Transform fillT = barT.Find("Fill Area/Fill");
            if (fillT != null) sliderFillImg = fillT.GetComponent<Image>();

            Transform bgBarT = barT.Find("Background");
            if (bgBarT != null) sliderBgImg = bgBarT.GetComponent<Image>();
        }
    }

    private void ApplyVisuals()
    {
        // Background
        if (bgImage != null)
        {
            bgImage.sprite = useBackgroundImage ? backgroundSprite : null;
            bgImage.color = useBackgroundImage && backgroundSprite != null ? Color.white : backgroundColor;
        }

        // Logo
        if (logoImg != null)
        {
            logoImg.gameObject.SetActive(showLogo);
            if (showLogo)
            {
                logoImg.sprite = logoSprite;
                logoImg.color = logoColor;
                logoImg.rectTransform.sizeDelta = logoSize;
            }
        }

        // Loading Bar
        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(showLoadingBar);
            loadingSlider.value = 0f; // Reset saat di inspector

            if (showLoadingBar)
            {
                if (sliderFillImg != null) sliderFillImg.color = barFillColor;
                if (sliderBgImg != null) sliderBgImg.color = barBackgroundColor;
            }
        }
    }

    // ============================================
    // MAIN SEQUENCE (SYNCED WITH TIME)
    // ============================================
    private IEnumerator PlaySequence()
    {
        float totalDuration = fadeInDuration + holdDuration + fadeOutDuration;
        float elapsed = 0f;

        // Tentukan posisi awal slide berdasarkan tipe animasi masuk
        SetSlideStartPosition();

        while (elapsed < totalDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            // Hitung progress keseluruhan (0.0 sampai 1.0)
            float totalProgress = Mathf.Clamp01(elapsed / totalDuration);

            // UPDATE LOADING BAR (Selaras dengan waktu)
            if (loadingSlider != null && showLoadingBar)
            {
                loadingSlider.value = totalProgress;
            }

            // UPDATE ANIMASI BERDASARKAN FASE WAKTU
            if (elapsed <= fadeInDuration)
            {
                // FASE 1: Fade In
                float enterProgress = Mathf.Clamp01(elapsed / fadeInDuration);
                UpdateAnimation(enterAnimation, enterProgress, isEnter: true);
            }
            else if (elapsed <= fadeInDuration + holdDuration)
            {
                // FASE 2: Hold (Pastikan animasi masuk tetap di posisi akhir 100%)
                UpdateAnimation(enterAnimation, 1f, isEnter: true);
            }
            else
            {
                // FASE 3: Fade Out
                float exitProgress = Mathf.Clamp01((elapsed - fadeInDuration - holdDuration) / fadeOutDuration);
                UpdateAnimation(exitAnimation, exitProgress, isEnter: false);
            }

            yield return null;
        }

        // Pastikan semuanya di kondisi akhir
        if (loadingSlider != null) loadingSlider.value = 1f;
        UpdateAnimation(exitAnimation, 1f, isEnter: false);

        // Pindah Scene
        SceneManager.LoadScene(nextScene);
    }

    // ============================================
    // ANIMATION UPDATE (Tanpa Coroutine/Loop)
    // ============================================
    private void SetSlideStartPosition()
    {
        if (logoImg == null || !showLogo) return;
        slideStartPos = Vector2.zero;

        if (enterAnimation == AnimType.SlideFromBottom) slideStartPos = new Vector2(0, -Screen.height);
        else if (enterAnimation == AnimType.SlideFromTop) slideStartPos = new Vector2(0, Screen.height);
        else if (enterAnimation == AnimType.SlideFromLeft) slideStartPos = new Vector2(-Screen.width, 0);
        else if (enterAnimation == AnimType.SlideFromRight) slideStartPos = new Vector2(Screen.width, 0);

        logoImg.rectTransform.anchoredPosition = slideStartPos;
    }

    private void UpdateAnimation(AnimType type, float progress, bool isEnter)
    {
        float p = Mathf.Clamp01(progress);

        switch (type)
        {
            case AnimType.FadeIn:
                if (isEnter) canvasGroup.alpha = p;
                break;

            case AnimType.FadeOut:
                if (!isEnter) canvasGroup.alpha = 1f - p;
                break;

            case AnimType.ScaleUp:
                if (isEnter && logoImg != null) logoImg.rectTransform.localScale = Vector3.one * EaseOutBack(p);
                break;

            case AnimType.ScaleDown:
                if (!isEnter && logoImg != null) logoImg.rectTransform.localScale = Vector3.one * (1f - EaseInBack(p));
                break;

            case AnimType.SlideFromBottom:
            case AnimType.SlideFromTop:
            case AnimType.SlideFromLeft:
            case AnimType.SlideFromRight:
                if (isEnter && logoImg != null) logoImg.rectTransform.anchoredPosition = Vector2.Lerp(slideStartPos, Vector2.zero, EaseOutCubic(p));
                break;

            case AnimType.Bounce:
                if (isEnter && logoImg != null) logoImg.rectTransform.localScale = Vector3.one * Bounce(p);
                break;

            case AnimType.RotateIn:
                if (isEnter && logoImg != null)
                {
                    logoImg.rectTransform.localScale = Vector3.one * EaseOutCubic(p);
                    logoImg.rectTransform.localRotation = Quaternion.Euler(0, 0, 360 * (1 - p));
                }
                break;

            case AnimType.ZoomBounce:
                if (isEnter && logoImg != null) logoImg.rectTransform.localScale = Vector3.one * ZoomBounce(p);
                break;

            case AnimType.None:
            default:
                canvasGroup.alpha = isEnter ? 1f : 0f;
                break;
        }
    }

    // ============================================
    // SCENE VALIDATION
    // ============================================
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == sceneName) return true;
        }
        return false;
    }

    // ============================================
    // EASING FUNCTIONS
    // ============================================
    private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f; float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private float EaseInBack(float t)
    {
        float c1 = 1.70158f; float c3 = c1 + 1f;
        return c3 * t * t * t - c1 * t * t;
    }

    private float Bounce(float t)
    {
        if (t < 0.3636f) return 7.5625f * t * t;
        if (t < 0.7272f) return 7.5625f * (t -= 0.5454f) * t + 0.75f;
        if (t < 0.9090f) return 7.5625f * (t -= 0.8181f) * t + 0.9375f;
        return 7.5625f * (t -= 0.9545f) * t + 0.984375f;
    }

    private float ZoomBounce(float t)
    {
        if (t < 0.6f) return EaseOutCubic(t / 0.6f) * 1.2f;
        return 1.2f - 0.2f * EaseOutCubic((t - 0.6f) / 0.4f);
    }
}
