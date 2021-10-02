using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PauseControl : MonoBehaviour {
    public static bool isPaused => Time.timeScale == 0;
    public static event Action onPause = delegate { };
    public static event Action onUnpause = delegate { };

    public static void Pause() {
        Time.timeScale = 0f;
        onPause();
    }

    public static void Unpause() {
        Time.timeScale = 1f;
        onUnpause();
    }

    public static void TogglePause () {
        if (isPaused) {
            Unpause();
        } else {
            Pause();
        }
    }
}
