#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class MainTaggedObject : MonoBehaviour {
    public DetalizationLevels DrawOn;
    MainGameAttributes attributes;
    bool isDrawn;

    void OnEnable() { 
        attributes = Camera.main.gameObject.GetComponent<MainGameAttributes>();
    
        MainGameAttributes.OnLowGraphDraw += HandleLow;
        MainGameAttributes.OnMediumGraphDraw += HandleMedium;
        MainGameAttributes.OnHighGraphDraw += HandleHigh;
    }

    void OnDisable() { 
        MainGameAttributes.OnLowGraphDraw -= HandleLow;
        MainGameAttributes.OnMediumGraphDraw -= HandleMedium;
        MainGameAttributes.OnHighGraphDraw -= HandleHigh;
    }

    private void HandleLow() => DrawMaching(0);
    private void HandleMedium() => DrawMaching(1);
    private void HandleHigh() => DrawMaching(2);

    void DrawMaching(int level) {
        if (this == null) return;
        switch (level) {
            case 0 : 
                if (DrawOn == DetalizationLevels.low && !isDrawn) { isDrawn = true; } 
                break;
            case 1 : 
                if (DrawOn == DetalizationLevels.medium && !isDrawn) { isDrawn = true; } 
                break;
            case 2 : 
                if (DrawOn == DetalizationLevels.high && !isDrawn) { isDrawn = true; } 
                break;
        }
        if (TryGetComponent<Renderer>(out var r)) {
            r.enabled = isDrawn;
        }
    }
}
#endregion