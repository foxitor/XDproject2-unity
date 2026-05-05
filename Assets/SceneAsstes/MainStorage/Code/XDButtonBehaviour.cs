using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI;
namespace XD.UI {
    public enum UI_types {
        button,
        switchButton,
        slider
    }
    public enum UI_target_types {
        gameObjectsActivity,
        RandomPicker,
        TriggerAnimation,
        LoadScene
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

    //ClassificationVars
    Button myButton;
    Slider mySlider;

    AudioSource myAudio;

    void Awake() {
        switch (type) {
            case UI_types.button : myButton = gameObject.GetComponent<Button>(); break;
            case UI_types.switchButton : myButton = gameObject.GetComponent<Button>(); break;
        }
    }
    void Start() {
        if (myButton != null) { myButton.onClick.AddListener(() => {ButtonPressed();}); }
        if (targetMode == UI_target_types.RandomPicker) {
            myAudio = gameObject.AddComponent<AudioSource>();
        }
    }
    public void ButtonPressed() {
        if (myButton == null) return;
        if (type == UI_types.button) {
            if (flipFlopStrat) {
                if (targetMode == UI_target_types.gameObjectsActivity) {
                    flipFlopState = !flipFlopState;
                    foreach (GameObject Obj in DefaultButtonPile.Positive) {
                        Obj.SetActive(flipFlopState);
                    }
                    foreach (GameObject Obj in DefaultButtonPile.Negative) {
                        Obj.SetActive(!flipFlopState);
                    }
                }
            }
            else if (targetMode == UI_target_types.RandomPicker) {
                int RandomRoll = Random.Range(0, RandomPickerEntries.Length);
                myAudio.Stop();
                myAudio.PlayOneShot(RandomPickerEntries[RandomRoll].Clip);
                RandomPickerEntries[RandomRoll].TargetAnimator.SetTrigger("Glow");
            } else if (targetMode == UI_target_types.TriggerAnimation) {
                SingleAnimator.SetTrigger(ActionName);
            } else if (targetMode == UI_target_types.LoadScene) {
                StartCoroutine(LoadSceneAsync(ActionName));
            }
        } else {
            switcherState += 1;
            if (switcherState > switcherVariants) {
                switcherState = 1;
            }
            foreach(PiledObjects piled in SwitcherButtonPiles) {
                for (int i = 0; i < piled.Positive.Length; i++) {
                    piled.Positive[i].SetActive(false);
                }
                for (int i = 0; i < piled.Negative.Length; i++) {
                    piled.Negative[i].SetActive(true);
                }
                
            }
            foreach(GameObject obj in SwitcherButtonPiles[switcherState-1].Positive) {
                obj.SetActive(true);
            }
            foreach(GameObject obj in SwitcherButtonPiles[switcherState-1].Negative) {
                obj.SetActive(false);
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
//###   WARNING   ###
//###  GOVNOCODE! ###
//###             ###
[CustomEditor(typeof(XDButtonBehaviour))]
public class XDButtonBehaviour_Editor : Editor {
    public override void OnInspectorGUI() {
        XDButtonBehaviour Elem = (XDButtonBehaviour)target;

        Elem.type = (UI_types)EditorGUILayout.EnumPopup("Type", Elem.type);
        Elem.targetMode = (UI_target_types)EditorGUILayout.EnumPopup("Target Type", Elem.targetMode);

        switch (Elem.type) {
            case UI_types.button:
                EditorGUILayout.LabelField("-=~=-");
                Elem.flipFlopStrat = EditorGUILayout.Toggle("Toggle Strat", Elem.flipFlopStrat);
                break;
            case UI_types.switchButton:
                EditorGUILayout.LabelField("-=~=-");
                Elem.switcherVariants = EditorGUILayout.IntField("Max Switcher Variants", Elem.switcherVariants);
                Elem.switcherDisplayNegatives = EditorGUILayout.Toggle("Display Negatives", Elem.switcherDisplayNegatives);
                break;
            default:
                EditorGUILayout.LabelField("-=~=-");
                EditorGUILayout.LabelField("No Editor Setting for type");
                break;
        }
        EditorGUILayout.LabelField("-=~=-");
        switch (Elem.targetMode) {
            case UI_target_types.gameObjectsActivity:
                if (Elem.type == UI_types.button) {
                    int posNewSize = EditorGUILayout.IntField("Positive Objects Size", Elem.DefaultButtonPile.Positive.Length, GUILayout.Height(20));
                    if (posNewSize != Elem.DefaultButtonPile.Positive.Length) {
                        System.Array.Resize(ref Elem.DefaultButtonPile.Positive, posNewSize);
                    }
                    for (int i = 0; i < Elem.DefaultButtonPile.Positive.Length; i++) {
                        Elem.DefaultButtonPile.Positive[i] = (GameObject)EditorGUILayout.ObjectField(
                            $"Element {i}", 
                            Elem.DefaultButtonPile.Positive[i], 
                            typeof(GameObject), 
                            true,
                            GUILayout.Height(15)
                        );
                    }
                    int negNewSize = EditorGUILayout.IntField("Negative Objects Size", Elem.DefaultButtonPile.Negative.Length, GUILayout.Height(20));
                    if (negNewSize != Elem.DefaultButtonPile.Negative.Length) {
                        System.Array.Resize(ref Elem.DefaultButtonPile.Negative, negNewSize);
                    }
                    for (int i = 0; i < Elem.DefaultButtonPile.Negative.Length; i++) {
                        Elem.DefaultButtonPile.Negative[i] = (GameObject)EditorGUILayout.ObjectField(
                            $"Element {i}", 
                            Elem.DefaultButtonPile.Negative[i], 
                            typeof(GameObject), 
                            true,
                            GUILayout.Height(15)
                        );
                    }
                } else if (Elem.type == UI_types.switchButton) {
                    System.Array.Resize(ref Elem.SwitcherButtonPiles, Elem.switcherVariants);
                    foreach(PiledObjects piled in Elem.SwitcherButtonPiles) {
                        EditorGUILayout.LabelField("Toggles");
                        EditorGUILayout.LabelField("=-=-=");
                        EditorGUILayout.LabelField("Positive");

                        int posNewSize = EditorGUILayout.IntField("  Positive Objects Size", piled.Positive.Length, GUILayout.Height(20));
                        if (posNewSize != piled.Positive.Length) {
                            System.Array.Resize(ref piled.Positive, posNewSize);
                        }

                        for (int i = 0; i < piled.Positive.Length; i++) {
                            piled.Positive[i] = (GameObject)EditorGUILayout.ObjectField(
                                $"    Element {i}", 
                                piled.Positive[i], 
                                typeof(GameObject), 
                                true,
                                GUILayout.Height(15)
                            );
                        }
                        if (Elem.switcherDisplayNegatives) {
                            EditorGUILayout.LabelField("-=-");
                            EditorGUILayout.LabelField("Negative");

                            int negNewSize = EditorGUILayout.IntField("  Negative Objects Size", piled.Negative.Length, GUILayout.Height(20));
                            if (negNewSize != piled.Negative.Length) {
                                System.Array.Resize(ref piled.Negative, negNewSize);
                            }
                            for (int i = 0; i < piled.Negative.Length; i++) {
                                piled.Negative[i] = (GameObject)EditorGUILayout.ObjectField(
                                    $"    Element {i}", 
                                    piled.Negative[i], 
                                    typeof(GameObject), 
                                    true,
                                    GUILayout.Height(15)
                                );
                            }
                        }
                    }
                }
                break;
            case UI_target_types.RandomPicker :
                int pullsNewSize = EditorGUILayout.IntField("Pull Entries Count", Elem.RandomPickerEntries.Length, GUILayout.Height(20));
                if (pullsNewSize != Elem.RandomPickerEntries.Length) {
                    System.Array.Resize(ref Elem.RandomPickerEntries, pullsNewSize);
                }
                for (int i = 0; i < Elem.RandomPickerEntries.Length; i++) {
                    EditorGUILayout.LabelField("   [#=#]");
                    //Animators
                    Elem.RandomPickerEntries[i].TargetAnimator = (Animator)EditorGUILayout.ObjectField(
                        $"   Element {i}", 
                        Elem.RandomPickerEntries[i].TargetAnimator, 
                        typeof(Animator), 
                        true,
                        GUILayout.Height(15)
                    );
                    //Sounds
                    Elem.RandomPickerEntries[i].Clip = (AudioClip)EditorGUILayout.ObjectField(
                        $"   Element {i}", 
                        Elem.RandomPickerEntries[i].Clip, 
                        typeof(AudioClip), 
                        true,
                        GUILayout.Height(15)
                    );
                }
                break;
            case UI_target_types.TriggerAnimation :
                Elem.SingleAnimator = (Animator)EditorGUILayout.ObjectField(
                        $"Target Animator", 
                        Elem.SingleAnimator, 
                        typeof(Animator), 
                        true
                    );
                Elem.ActionName = EditorGUILayout.TextField("Action Name", Elem.ActionName);
                break;
            case UI_target_types.LoadScene :
                Elem.ActionName = EditorGUILayout.TextField("Scene Name", Elem.ActionName);
                break;
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(Elem);
        }
    }
}
