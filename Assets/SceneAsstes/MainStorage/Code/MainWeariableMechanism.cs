#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class MainWeariableMechanism : MonoBehaviour {
    public string savePath;
    public string customData;
    public int order;
    public WearableStates state;
    public MainGameAttributes attributes;

    Button myButton;
    string LastUsedPath = "";

    public static event System.Action OnWearablePick;

    void OnEnable() { OnWearablePick += Reset; MainGameAttributes.OnGameStart += UseOrDeny; }
    void OnDisable() { OnWearablePick -= Reset; MainGameAttributes.OnGameStart -= UseOrDeny; }

    void Start() {
        LastUsedPath = $"{savePath} Picked";
        if (!PlayerPrefs.HasKey(savePath)) {
            int result = 0;
            if (customData == "default") {
                result = 1;
            } else { result = 0; }
            PlayerPrefs.SetInt(savePath, result);

            if (PlayerPrefs.GetInt(savePath) == 1) {
                state = WearableStates.unlocked;
            } else {
                state = WearableStates.locked;
            }
        } else {
            if (!PlayerPrefs.HasKey(LastUsedPath)) {
                if (customData == "default" && !PlayerPrefs.HasKey(savePath)) {
                    PlayerPrefs.SetInt(LastUsedPath, 1);
                    state = WearableStates.selected;
                } else {
                    PlayerPrefs.SetInt(LastUsedPath, 0);
                }
            }
        }
        if (PlayerPrefs.GetInt(savePath) == 1) {
                state = WearableStates.unlocked;
                if (PlayerPrefs.HasKey(LastUsedPath)) {
                    if (PlayerPrefs.GetInt(LastUsedPath) == 1) {
                        state = WearableStates.selected;
                    }
                }
            } else {
                state = WearableStates.locked;
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
        if (state == WearableStates.locked || state == WearableStates.unlocked) { UpdateDisplay(); return; }
        state = WearableStates.unlocked;
        PlayerPrefs.SetInt(LastUsedPath, 0);
        UpdateDisplay();
    }
    public void OnPress() {
        if (state == WearableStates.locked || state == WearableStates.selected) { UpdateDisplay(); return; }
        OnWearablePick?.Invoke();
        state = WearableStates.selected;
        PlayerPrefs.SetInt(LastUsedPath, 1);
        UpdateDisplay();
    }
    void UseOrDeny() {
        if (state == WearableStates.selected) {
            attributes.AssignWearable(order);
        }
    }
}
#endregion