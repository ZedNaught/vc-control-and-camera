using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public Unit activeUnit;

//    Transform activeUnit;
//
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        activeUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
    }
}
