#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class MainWeariableMechanism : MonoBehaviour {
    public string savePath;
    public string customData;
    public WearableStates state;
    public MainGameAttributes attributes;

    Button myButton;

    public static event System.Action OnWearablePick;

    void OnEnable() => OnWearablePick += Reset;
    void OnDisable() => OnWearablePick -= Reset;

    void Start() {
        if (!PlayerPrefs.HasKey(savePath)) {
            int result = 0;
            if (customData == "default") {
                result = 1;
            } else { result = 0; }
            PlayerPrefs.SetInt(savePath, result);
        } else {
            if (PlayerPrefs.GetInt(savePath) == 1) {
                state = WearableStates.unlocked;
            } else {
                state = WearableStates.locked;
            }
            string LastUsedPath = $"{savePath} Picked";
            if (PlayerPrefs.HasKey(LastUsedPath)) {
                if (PlayerPrefs.GetInt(LastUsedPath) == 1) {
                    state = WearableStates.selected;
                }
            } else {
                if (customData == "default") {
                    PlayerPrefs.SetInt(LastUsedPath, 1);
                    state = WearableStates.selected;
                }
                else {
                    PlayerPrefs.SetInt(LastUsedPath, 0);
                }
            }
        }
        UpdateDisplay();
        myButton = gameObject.GetComponent<Button>();
        if (myButton != null) { myButton.onClick.AddListener(() => {OnPress();}); }
    }
    void UpdateDisplay() {
        transform.GetChild(1).gameObject.SetActive(state == WearableStates.locked);
        transform.GetChild(0).gameObject.SetActive(state == WearableStates.selected);
    }
    public void Reset() {
        string LastUsedPath = $"{savePath} Picked";
        if (state == WearableStates.locked || state == WearableStates.unlocked) { UpdateDisplay(); return; }
        state = WearableStates.unlocked;
        PlayerPrefs.SetInt(LastUsedPath, 0);
        UpdateDisplay();
    }
    public void OnPress() {
        string LastUsedPath = $"{savePath} Picked";
        if (state == WearableStates.locked || state == WearableStates.selected) { UpdateDisplay(); return; }
        OnWearablePick?.Invoke();
        state = WearableStates.selected;
        PlayerPrefs.SetInt(LastUsedPath, 1);
        UpdateDisplay();
    }
}
#endregion