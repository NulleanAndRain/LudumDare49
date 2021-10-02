using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PauseControl : MonoBehaviour {
    // added
    public static bool isPaused { get; private set; } = false;
    // если надо сделать поле, которое надо получать извне, но нужно менять только внутри класса, пишешь такую хуиту

    public static void Pause() { // renamed
        Time.timeScale = 0; //Мировое время приравневаем к нулю, т.е. останавливаем
        isPaused = true;
    }

    public static void Unpause() { // renamed
        Time.timeScale = 1; //Возвращаем прежнее значение мирового времени. 1 - нормальное течение
        isPaused = false;
    }

    // added
    public static void togglePause () {
        if (isPaused) {
            Unpause();
        } else {
            Pause();
        }
    }
}
