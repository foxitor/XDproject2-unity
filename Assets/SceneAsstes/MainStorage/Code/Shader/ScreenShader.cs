using UnityEngine; using UnityEngine.UI;

public class ScreenShader : MonoBehaviour {
    public RectTransform ScreenLines;
    private Vector3 DefaultPosition;
    public float resetThresholdY = -30f;
    public float Speed;

    void Start() {
        DefaultPosition = ScreenLines.anchoredPosition;
    }

    void Update() {
        Vector3 currentPosition = ScreenLines.anchoredPosition;
        currentPosition.y -= Speed * Time.deltaTime;

        if (ScreenLines.anchoredPosition.y < resetThresholdY) { currentPosition = DefaultPosition; }
        ScreenLines.anchoredPosition = currentPosition;
    }
}