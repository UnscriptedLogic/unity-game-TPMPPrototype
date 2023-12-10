using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance singleton { get; private set; } = null;

    private void Awake()
    {
        if (singleton != null)
        {
            Debug.Log($"Found duplicate GameInstance on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        singleton = this;

        DontDestroyOnLoad(gameObject);
    }
}
