using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(ClassicManager manager, float time);

	public RectTransform gameOverPanel;
	public Text resultText;
	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	private uint currentHealth;

	void Awake()
	{
		base.AwakeWorkaround();
		gameOverPanel.gameObject.SetActive(false);
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
	}

	void OnGUI()
	{
		if (ship != null && Time.timeScale != 0.0f)
		{
			GUI.skin.label.fontSize = 50;
			GUI.Label(new Rect(10, 10, 130, 60), "Time: ");
			GUI.Label(new Rect(180, 10, 130, 60), GameTime.ToString());
			GUI.Label(new Rect(10, 70, 190, 60), "Health: ");
			GUI.Label(new Rect(180, 70, 30, 60), currentHealth.ToString());
		}
	}

	public override void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
		if (--currentHealth == 0)
		{
			Time.timeScale = 0.0f;
			gameOverHandler(this, GameTime);
		}
	}

	public static void DefaultGameOverHandler(ClassicManager manager, float time)
	{
		manager.gameOverPanel.gameObject.SetActive(true);
		manager.resultText.text = "Time: " + manager.GameTime;
    }

}
