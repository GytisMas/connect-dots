using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataSingleton : MonoBehaviour
{
    public static LevelDataSingleton instance;
    public Level level;
    private void Awake() {
        if (instance != this) {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }
    }
}
