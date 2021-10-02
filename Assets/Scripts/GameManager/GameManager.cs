using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	public float respawnTime;

	private static GameManager _instance;

	public static float RespawnTime { get => _instance.respawnTime; }

	private void Start () {
		if (_instance != null) {
			Destroy(gameObject);
			return;
		}
		_instance = this;

		Application.targetFrameRate = 100; //Эти две строки ограничивают количество кадров в главном меню
		QualitySettings.vSyncCount = 0;
	}

}
