using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; set; }

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Destroy(gameObject);
            return;
        }
    }
}
