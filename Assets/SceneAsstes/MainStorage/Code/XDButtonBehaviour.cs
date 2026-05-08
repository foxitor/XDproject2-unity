#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement; using UnityEngine.Audio;
using XD.UI; using XD.Prefs;
#endregion
#region CustomLibrary
namespace XD.UI {
    public enum UI_types {
        button,
        toggleButton,
        switchButton,
        slider,
        dropdown
    }
    public enum UI_target_types {
        gameObjectsActivity,
        RandomPicker,
        TriggerAnimation,
        LoadScene,
        StoreIntData,
        StoreFloatData
    }
    [System.Serializable]
    public class PiledObjects {
        public GameObject[] Positive = new GameObject[0];
        public GameObject[] Negative = new GameObject[0];
    }
    [System.Serializable]
    public class RandomPullEntry {
        public Animator TargetAnimator;
        public AudioClip Clip;
    }
}
#endregion
#region Class
public class XDButtonBehaviour : MonoBehaviour {
    //General 
    public UI_types type;
    public UI_target_types targetMode;

    //Strats
    public bool flipFlopStrat;
    public int switcherVariants;
    public bool switcherDisplayNegatives;

    //StratData
    bool flipFlopState;
    int switcherState = 1;

    //Targets
    public PiledObjects DefaultButtonPile;
    public PiledObjects[] SwitcherButtonPiles = new PiledObjects[0];
    public RandomPullEntry[] RandomPickerEntries;
    public Animator SingleAnimator;
    public string ActionName;
    public string[] Strings = new string[0];
    public AudioMixer Mixer;
    public bool InteractWithMixer;

    //ClassificationVars
    Button myButton;
    Slider mySlider;
    Dropdown myDropdown;
    //Utility
    AudioSource myAudio;

    //CompileTypes
    void Awake() {
        switch (type) {
            //Assign Button
            case UI_types.button : 
                case UI_types.toggleButton :
                    case UI_types.switchButton : myButton = gameObject.GetComponent<Button>(); break;
            //Assign Dropdown
            case UI_types.dropdown : myDropdown = gameObject.GetComponent<Dropdown>(); break;
            //Assign Slider
            case UI_types.slider : mySlider = gameObject.GetComponent<Slider>(); break;
            //Assign Error
            default:
                Debug.Log(type + " Doesn't Work within awake assign");
                break;
        }
    }
    void OnEnable() => AdvancedPPfs.OnDataReset += RestoreDatas;
    void OnDisable() => AdvancedPPfs.OnDataReset -= RestoreDatas;
    //PlugAdditionss
    void Start() {
        //Button
        if (myButton != null) { myButton.onClick.AddListener(() => {ButtonPressed();}); }
        //Dropdown
        if (myDropdown != null) { myDropdown.onValueChanged.AddListener(delegate {DropdownChanged();}); }
        //Slider
        if (mySlider != null) { mySlider.onValueChanged.AddListener(delegate {SliderValueTweaked();}); }
        //Audio
        if (targetMode == UI_target_types.RandomPicker) { myAudio = gameObject.GetComponent<AudioSource>(); }

        RestoreDatas();
    }
    public void ButtonPressed() {
        if (myButton == null) return;
        //DefaultButton
        if (type == UI_types.button) {
            //RandomPicker (SmexuatinaSounds)
            if (targetMode == UI_target_types.RandomPicker) {
                //Pick
                int RandomRoll = Random.Range(0, RandomPickerEntries.Length);
                //PlaySound
                myAudio.Stop(); myAudio.PlayOneShot(RandomPickerEntries[RandomRoll].Clip);
                //Animate Glow
                RandomPickerEntries[RandomRoll].TargetAnimator.SetTrigger("Glow");
            }
            //SetTrigger on Animator
            else if (targetMode == UI_target_types.TriggerAnimation) { SingleAnimator.SetTrigger(ActionName); }
            //Scene manage
            else if (targetMode == UI_target_types.LoadScene) {
                //'RunScene' or 'ForceQuit'
                if (ActionName == "ForceQuit") { 
                    Application.Quit();
                } else if (ActionName == "DeletePrefs") {
                    AdvancedPPfs.Core.Erase();
                } else { StartCoroutine(LoadSceneAsync(ActionName));  }
            }
        } 
        //ToggleButton
        else if (type == UI_types.toggleButton) {
            //ToggleObjects
            if (targetMode == UI_target_types.gameObjectsActivity) {
                flipFlopState = !flipFlopState;
                foreach (GameObject GutObj in DefaultButtonPile.Positive) { GutObj.SetActive(flipFlopState); }
                foreach (GameObject BadObj in DefaultButtonPile.Negative) { BadObj.SetActive(!flipFlopState); }
            }
        } 
        //SwitcherButton
        else if (type == UI_types.switchButton) {
            //AddProfile
            if (switcherState > switcherVariants) { switcherState = 1; } else { switcherState += 1; }
            //SwitchMomentally (Might be bad for the performance if includes alotof objects)
            foreach(PiledObjects piled in SwitcherButtonPiles) {
                for (int i = 0; i < piled.Positive.Length; i++) { piled.Positive[i].SetActive(false); }
                for (int i = 0; i < piled.Negative.Length; i++) { piled.Negative[i].SetActive(true); }
            }
            foreach(GameObject GutObj in SwitcherButtonPiles[switcherState-1].Positive) { GutObj.SetActive(true); }
            foreach(GameObject BadObj in SwitcherButtonPiles[switcherState-1].Negative) { BadObj.SetActive(false); }
        }
    }
    public void DropdownChanged() {
        if (myDropdown == null) return;
        //Storing Ints
        if (targetMode == UI_target_types.StoreIntData) {
            foreach (string ppf in Strings) {
                AdvancedPPfs.Core.ProtectedStore(ppf, myDropdown.value);
                //Debug.Log($"I as a computer programm now indeed store the data, that can be in fact be translated to {ppf}.\nyou may be asking yourself now, whats the value that the {ppf} obtained? so the answer is simple my friend, it is equal to {myDropdown.value}");
            }
        }
    }
    public void SliderValueTweaked() {
        if (mySlider == null) return;
        //StoreFloat
        if (targetMode == UI_target_types.StoreFloatData) {
            foreach (string ppf in Strings) {
                AdvancedPPfs.Core.ProtectedStore(ppf, mySlider.value);
                //Debug.Log($"I as a computer programm now indeed store the data, that can be in fact be translated to {ppf}.\nyou may be asking yourself now, whats the value that the {ppf} obtained? so the answer is simple my friend, it is equal to {mySlider.value}");
            }
            if (InteractWithMixer) {
                switch (Strings[0]) {
                    case "music_volume" :
                        Mixer.SetFloat("MusicVolume", Mathf.Log10(mySlider.value)*20);
                        break;
                    case "sfx_volume" :
                        Mixer.SetFloat("SfxVolume", Mathf.Log10(mySlider.value)*20);
                        break;
                }
            }
        }
    }
    void RestoreDatas() {
        //Debug.Log($"RestoreData Was Runned by {gameObject.name}");
        //Restore Drops
        if (myDropdown != null) {
            //RestoreSeasons
            if (PlayerPrefs.HasKey(Strings[0])) { 
                myDropdown.value = PlayerPrefs.GetInt(Strings[0]); 
                //Debug.Log($"RestoreData Was Finished, result : found the ppf");
                } 
            else { foreach (string ppf in Strings) { 
                AdvancedPPfs.Core.ProtectedStore(ppf, myDropdown.value); 
                } Debug.Log($"RestoreData Was Finished, result : found the protective data");
            }
        }
        if (mySlider != null) {
            //RestoreSeasons
            if (PlayerPrefs.HasKey(Strings[0])) { 
                mySlider.value = PlayerPrefs.GetFloat(Strings[0]); 
                //Debug.Log($"RestoreData Was Finished, result : found the ppf");
                } 
            else { foreach (string ppf in Strings) { 
                AdvancedPPfs.Core.ProtectedStore(ppf, mySlider.value); 
                } Debug.Log($"RestoreData Was Finished, result : found the protective data");
            }
        }
    }
    IEnumerator LoadSceneAsync(string Name) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Name);
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
#endregion
#region CustomEditor
//###   WARNING   ###
//###    Mess     ###
//###             ###
#if UNITY_EDITOR
[CustomEditor(typeof(XDButtonBehaviour))]
public class XDButtonBehaviour_Editor : Editor {
    public override void OnInspectorGUI() {
        DrawText("XDproject's UI Editor"); DrawText("SniffersInc. Technology");
        //Root
        DrawText("-=~=-");
        XDButtonBehaviour Elem = (XDButtonBehaviour)target;
        Elem.type = (UI_types)EditorGUILayout.EnumPopup("Type", Elem.type);
        Elem.targetMode = (UI_target_types)EditorGUILayout.EnumPopup("Target Type", Elem.targetMode);
        //Strats
        DrawText("-=~=-");
        switch (Elem.type) {
            case UI_types.switchButton:
                Elem.switcherVariants = EditorGUILayout.IntField("Max Switcher Variants", Elem.switcherVariants);
                Elem.switcherDisplayNegatives = EditorGUILayout.Toggle("Display Negatives", Elem.switcherDisplayNegatives);
                break;
            default:
                DrawText("No Editor Setting for type");
                break;
        }
        //Targets
        DrawText("-=~=-");
        switch (Elem.targetMode) {
            //GameObjs Active
            case UI_target_types.gameObjectsActivity:
                //SinglePile
                if (Elem.type == UI_types.button) {
                    ResizeObjectArray(ref Elem.DefaultButtonPile.Positive, "  Positive");
                    EditObjectArray(Elem.DefaultButtonPile.Positive, "    +");
                    ResizeObjectArray(ref Elem.DefaultButtonPile.Negative, "  Negative");
                    EditObjectArray(Elem.DefaultButtonPile.Negative, "    -");
                } 
                //Multy-pile
                else if (Elem.type == UI_types.switchButton) {
                    System.Array.Resize(ref Elem.SwitcherButtonPiles, Elem.switcherVariants);
                    DrawText("Toggles");
                    foreach(var pile in Elem.SwitcherButtonPiles) {
                        DrawText("  [#=#=#]");
                        ResizeObjectArray(ref pile.Positive, "  Positives");
                        EditObjectArray(pile.Positive, "    +");
                        if (Elem.switcherDisplayNegatives) {
                            DrawText("    -=-=-");
                            ResizeObjectArray(ref pile.Negative, "  Negatives");
                            EditObjectArray(pile.Negative, "    -");
                        }
                    }
                }
                break;
            //Pulll
            case UI_target_types.RandomPicker :
                ResizeRandomPullArray(ref Elem.RandomPickerEntries, "  Pull Entries");
                EditRandomPullArray(Elem.RandomPickerEntries, "    {");
                break;
            //Animation Trigger
            case UI_target_types.TriggerAnimation :
                Elem.SingleAnimator = (Animator)EditorGUILayout.ObjectField(
                        $"Target Animator", Elem.SingleAnimator, typeof(Animator), true
                    );
                Elem.ActionName = EditorGUILayout.TextField("Action Name", Elem.ActionName);
                break;
            //RunScene
            case UI_target_types.LoadScene :
                Elem.ActionName = EditorGUILayout.TextField("Scene Name", Elem.ActionName);
                break;
            //Store
            case UI_target_types.StoreFloatData:
                Elem.Mixer = (AudioMixer)EditorGUILayout.ObjectField($" Target Mixer", Elem.Mixer, typeof(AudioMixer), true);
                if (Elem.type == UI_types.slider) {
                    Elem.InteractWithMixer = EditorGUILayout.Toggle("Apply to AudioMixer", Elem.InteractWithMixer);
                }
                ResizeStringArray(ref Elem.Strings, "   Paths");
                EditStringArray(Elem.Strings, "   Path");
                break;
            case UI_target_types.StoreIntData :
                ResizeStringArray(ref Elem.Strings, "   Paths");
                EditStringArray(Elem.Strings, "   Path");
                break;
                
            //Null
            default :
            DrawText("No Editor Starts for this Target-type");
                break;
        }
        if (GUI.changed) { EditorUtility.SetDirty(Elem); }
    }
    void DrawText(string text) {
        EditorGUILayout.LabelField(text);
    }

    //Obj
    void ResizeObjectArray(ref GameObject[] array, string label) {
        int newSize = EditorGUILayout.IntField($"{label} Size", array.Length, GUILayout.Height(20));
        if (newSize != array.Length) System.Array.Resize(ref array, newSize);
    }
    void EditObjectArray(GameObject[] array, string label) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = (GameObject)EditorGUILayout.ObjectField($"{label} Element {i}", array[i], typeof(GameObject), true, GUILayout.Height(15));
        }
    }
    //Strings
    void ResizeStringArray(ref string[] array, string label) {
        int newSize = EditorGUILayout.IntField($"{label} Size", array.Length, GUILayout.Height(20));
        if (newSize != array.Length) System.Array.Resize(ref array, newSize);
    }
    void EditStringArray(string[] array, string label) {
        for (int i = 0; i < array.Length; i++) {
            array[i] = EditorGUILayout.TextField($"{label} Element {i}", array[i], GUILayout.Height(15));
        }
    }
    //Pull
    void ResizeRandomPullArray(ref RandomPullEntry[] array, string label) {
        int newSize = EditorGUILayout.IntField($"{label} Size", array.Length, GUILayout.Height(20));
        if (newSize != array.Length) System.Array.Resize(ref array, newSize);
    }
    void EditRandomPullArray(RandomPullEntry[] array, string label) {
        for (int i = 0; i < array.Length; i++) {
            DrawText("   [#=#]");
            array[i].TargetAnimator = (Animator)EditorGUILayout.ObjectField(
                $"{label} Element {i}", array[i].TargetAnimator, typeof(Animator), true, GUILayout.Height(15)
            );
            array[i].Clip = (AudioClip)EditorGUILayout.ObjectField(
                $"{label} Element {i}", array[i].Clip, typeof(AudioClip), true, GUILayout.Height(15)
            );
        }
    }
}
#endif
#endregion