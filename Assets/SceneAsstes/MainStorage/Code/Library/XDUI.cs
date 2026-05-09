using UnityEngine;

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