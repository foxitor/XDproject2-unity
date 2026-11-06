using UnityEngine;
using XD.Games;

public class ChertoletTaggedObject : MainTaggedObject {
    public CLObjTypes Type;
    public string objectSubtype;
    bool isObsticale => Type == CLObjTypes.rock || Type == CLObjTypes.egg || Type == CLObjTypes.liqud;
    bool isSpining => Type == CLObjTypes.rock || Type == CLObjTypes.egg;
    public float obsticaleSpeed = 7.0f;

    void Update() {
        if (isObsticale) {
            transform.position += Vector3.left * obsticaleSpeed * Time.deltaTime;
            if (isSpining) transform.Rotate(0, 0, (obsticaleSpeed * 10) * Time.deltaTime);
            if (transform.position.x < -20f) Destroy(gameObject);
        }
    }
}
