using UnityEngine;

public class CanvasMouseParallax : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float maxOffset = 10f;      // asla geçmez
    public float smoothSpeed = 3f;     // ne kadar aðýr davransýn
    public float sensitivity = 0.4f;   // mouse etkisi (NEFES GÝBÝ)

    [Header("Dead Zone")]
    public float deadZone = 0.15f;     // merkeze yakýnken hiç oynamaz

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.width) - 0.5f;

        // dead zone
        if (Mathf.Abs(mouseX) < deadZone)
            mouseX = 0f;

        // easing (küçük hareket = çok küçük tepki)
        float easedX = Mathf.Sign(mouseX) * Mathf.Pow(Mathf.Abs(mouseX), 2);

        float offsetX = Mathf.Clamp(
            easedX * maxOffset * sensitivity,
            -maxOffset,
            maxOffset
        );

        Vector2 targetPos = startPos + new Vector2(offsetX, 0);

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPos,
            Time.deltaTime * smoothSpeed
        );
    }
}
