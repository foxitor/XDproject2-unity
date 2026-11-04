using System.Collections; using System.Collections.Generic;
using UnityEngine; using UnityEngine.InputSystem;
using XD.Games;

public class ChertoletController : MonoBehaviour {
    ChertoletAttributes Attributes;
    Rigidbody2D Phy;
    bool canMove = true;

    GameObject visual; SpriteRenderer visualRenderer;
    AudioSource mySource;

    public float FlapStrength = 3.5f;
    float currentRotation;
    public float flapRotationLimit = 30f;

    public GameObject feedMessengeGroup;
    public GameObject cameraMessengeGroup;

    public LayerMask obsticaleLayer;

    void OnEnable() { 
        MainGameAttributes.OnLateGameStart += OnGameEnable;
    } 
    void OnDisable() { 
        MainGameAttributes.OnLateGameStart -= OnGameEnable;
    }

    void Start() {
        Attributes = GameObject.Find("Main Camera").GetComponent<ChertoletAttributes>();
        mySource = gameObject.GetComponent<AudioSource>();
        visual = transform.GetChild(0).gameObject;
        visualRenderer = visual.GetComponent<SpriteRenderer>();
        Phy = this.gameObject.GetComponent<Rigidbody2D>();
        obsticaleLayer = LayerMask.GetMask("Obsticale");
    }
    public void OnGameEnable() {
        StartCoroutine(CameraMessenge(0, 2.5f));
    }
    void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) { 
            Flap(); 
        }

        float targetRotation = Mathf.Lerp(-flapRotationLimit, flapRotationLimit, (Phy.velocity.y + 5) / 10);
        targetRotation = Mathf.Clamp(targetRotation, -flapRotationLimit, flapRotationLimit);
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, 2 * Time.deltaTime);

        visual.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        if (!canMove) { 
            Phy.simulated = false; 
        } else { 
            Phy.simulated = true; 
        }
        if (Attributes.CurrentLap >= Attributes.MaxLaps) {
            cameraMessengeGroup.transform.GetChild(2).gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return)) {
                Attributes.EndGame();
            }
        }
    } 
    void Flap() {
        if (canMove) {
            Phy.velocity = Vector2.up * FlapStrength;
        }
    }
    IEnumerator FeedMessenge(int order, float liveTime) {
        feedMessengeGroup.transform.GetChild(order).gameObject.SetActive(true);
        //VibrateController(0.25f, 0.25f, 0.15f);
        yield return new WaitForSeconds(liveTime);
        feedMessengeGroup.transform.GetChild(order).gameObject.SetActive(false);
    }
    IEnumerator CameraMessenge(int order, float liveTime) {
        cameraMessengeGroup.transform.GetChild(order).gameObject.SetActive(true);
        yield return new WaitForSeconds(liveTime);
        cameraMessengeGroup.transform.GetChild(order).gameObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collision) {
        if ((obsticaleLayer.value & (1 << collision.gameObject.layer)) > 0) {
            Debug.Log("dead");
        }
    }
}
