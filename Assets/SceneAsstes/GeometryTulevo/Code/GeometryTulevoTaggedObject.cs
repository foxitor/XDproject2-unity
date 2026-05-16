#region Import Libs
using System.Collections;
using UnityEngine; using UnityEngine.UI; using UnityEditor; using UnityEngine.SceneManagement;
using XD.UI; using XD.Prefs; using XD.Games;
#endregion
#region Class
public class GeometryTulevoTaggedObject : MainTaggedObject {
    public GTObjTypes Type;
    public Transform teleportLink;
    public float rotationSpeed;
    public bool layerChanger;
    public string objectSubtype;

    GeometryTulevoAttributes attributes;

    [Header("-loot-")]
    bool isLooted;
    public int lootChance;
    public GameObject[] posibleLoots;

    void OnEnable() {
        if (Type == GTObjTypes.wallpaintedTip) {
            GeometryTulevoAttributes.NewLayer += OnLayer;
        }
    }
    void OnDisable() {
        if (Type == GTObjTypes.wallpaintedTip) {
            GeometryTulevoAttributes.NewLayer -= OnLayer;
        }
    }
    void Start() {
        attributes = GameObject.Find("Main Camera").GetComponent<GeometryTulevoAttributes>();
        OnLayer();
    }
    void Update() {
        if (Type == GTObjTypes.saw) {
            transform.Rotate(0, 0, -180 * rotationSpeed * Time.deltaTime);
        }
    } 
    void OnLayer() {
        if (Type == GTObjTypes.wallpaintedTip) {
            if (objectSubtype == "dash-unlocked") {
                if (attributes.layersSearched == attributes.dashGoal) {
                    if (TryGetComponent<Renderer>(out var r)) { r.enabled = true; }
                } else { 
                    if (TryGetComponent<Renderer>(out var r)) { r.enabled = false; }
                }
            } else if (objectSubtype == "lootChance") {
                if (attributes.layersSearched == lootChance) {
                    if (TryGetComponent<Renderer>(out var r)) { r.enabled = true; }
                } else { 
                    if (TryGetComponent<Renderer>(out var r)) { r.enabled = false; }
                }
            }
        }
    }
    public void OpenBox() {
        isLooted = true;
        transform.GetChild(0).gameObject.SetActive(false);
        SpawnLoot();
    }
    public void RestoreBox() {
        isLooted = false;
        transform.GetChild(0).gameObject.SetActive(true);
        if (transform.childCount > 1) {
            Destroy(transform.GetChild(1).gameObject);
        }
    }
    public void SpawnLoot() {
        if (Random.Range(0, lootChance) == lootChance-1 && !isLooted) {
            Vector3 defPos = transform.position;
            Vector3 spawnPos = new Vector3(defPos.x + Random.Range(-0.25f, 0.25f), defPos.y + Random.Range(-0.25f, 0.25f), defPos.z);
            Instantiate(posibleLoots[Random.Range(0, posibleLoots.Length)], transform.position, Quaternion.identity, transform);
        }
    }
}
#endregion