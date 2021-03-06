﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUI : SingletonBehaviour<DebugUI> {

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    bool isDebug =
#if DEBUG
        true
#else
        false
#endif
        ;

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.F3))
        {
            isDebug = !isDebug;
        }
	}

    private void OnGUI()
    {
        if(isDebug)
        {
            switch(SceneManager.GetActiveScene().name)
            {
                case "PlayingScene":
                {
                    GUILayout.Label("Replay Events Length: " + PlayingSceneManager.Instance.replay.events.Count);
                    if (GUILayout.Button("End Map")) PlayingSceneManager.Instance.EndMap();
                } break;
            }
        }
    }
}
