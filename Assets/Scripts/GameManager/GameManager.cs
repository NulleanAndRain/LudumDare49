using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _respawnTime;
    [SerializeField] private float _EffectTickInterval;

    public static GameManager Instance { get; private set; }
    public event Action onEffectTick = delegate { };

    public static float RespawnTime => Instance._respawnTime;
    public static float EffectTickInterval => Instance._EffectTickInterval;

    private void Awake () {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Application.targetFrameRate = 100; //Эти две строки ограничивают количество кадров в главном меню
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        StartCoroutine(effectTickCoroutine());
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private IEnumerator effectTickCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_EffectTickInterval);
            onEffectTick();
        }
    }

}
