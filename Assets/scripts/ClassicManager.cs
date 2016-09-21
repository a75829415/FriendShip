using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(ClassicManager manager, float time);

	public Canvas uiCanvas;

	public RectTransform Hud;
	public Text hudTime;
	public Text hudHealth;

	public RectTransform gameOverPanel;
	public Text resultText;

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	private uint currentHealth;

	void Awake()
	{
		base.AwakeWorkaround();
		gameOverPanel.gameObject.SetActive(false);
		Hud.gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start()
	{
		base.StartWorkaround();
		currentHealth = health;
    }
	
	// Update is called once per frame
	void Update()
	{
		base.UpdateWorkaround();
		if (!Hud.gameObject.activeSelf && currentHealth > 0 && ship != null)
		{
			uiCanvas.worldCamera = playerCamera;
			Hud.gameObject.SetActive(true);
		}
		hudTime.text = GameTimeInString;
		hudHealth.text = currentHealth + "/" + health;
    }

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (--currentHealth == 0)
		{
			Time.timeScale = 0.0f;
			Hud.gameObject.SetActive(false);
			gameOverHandler(this, GameTime);
		}
	}

	public static void DefaultGameOverHandler(ClassicManager manager, float time)
	{
		manager.gameOverPanel.gameObject.SetActive(true);
		manager.resultText.text = "Time: " + manager.GameTimeInString;
    }

}
