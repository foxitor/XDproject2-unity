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
    [Header("-loot-")]
    public int lootChance;
    public GameObject[] posibleLoots;
    void Update() {
        if (Type == GTObjTypes.saw) {
            transform.Rotate(0, 0, -180 * rotationSpeed * Time.deltaTime);
        }
    } 
    public void OpenBox() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        SpawnLoot();
    }
    public void RestoreBox() {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        if (transform.childCount > 1) {
            Destroy(transform.GetChild(1).gameObject);
        }
    }
    public void SpawnLoot() {
        if (Random.Range(0, lootChance) == lootChance) {
            Instantiate(posibleLoots[Random.Range(0, posibleLoots.Length)], transform.position, Quaternion.identity, transform);
        }
    }
}
#endregion