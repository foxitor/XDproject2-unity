using UnityEngine;
using XD.Prefs;

public class XDAdvancement : MonoBehaviour {
    public GameObject Checkmark;

    public string AchivementPref;
    public string AdvancmentType = "Default";
        public string[] AllinPrefs;
        public string RewardPref;
    
    void OnEnable() { AdvancedPPfs.OnDataReset += CheckPref; }
    void OnDisable() { AdvancedPPfs.OnDataReset -= CheckPref; }

    void Start() { CheckPref(); }

    void CheckPref() {
        switch (AdvancmentType) {
            case "Default" : Checkmark.SetActive(!AdvancedPPfs.Core.isNull(AchivementPref)); 
            break;
            case "AllIn" : 
                bool Complited = true;
                for (int a = 0; a < AllinPrefs.Length-1; a++) {
                    Complited = AdvancedPPfs.Core.isNull(AllinPrefs[a]) ? false : Complited;
                }
                Checkmark.SetActive(Complited);
                if (Complited) { PlayerPrefs.SetInt(AchivementPref, 1); }
            break;
            case "Reward" : 
                Checkmark.SetActive(!AdvancedPPfs.Core.isNull(AchivementPref)); 
                if (!AdvancedPPfs.Core.isNull(AchivementPref)) {
                    PlayerPrefs.SetInt(RewardPref, 1);
                }
            break;
        }
    }
}
