using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassicManager : Manager
{
	public delegate void GameOverHandler(ClassicManager manager, float time);

	public Canvas uiCanvas;

	public RectTransform hud;
	public Text hudTime;
	public Text hudHealth;

	public RectTransform status;
	public Text statusName;
	public RectTransform statusTime;

	public RectTransform gameOverPanel;
	public Text resultText;

	public GameOverHandler gameOverHandler = DefaultGameOverHandler;

	public uint health;

	private uint currentHealth;

	void Awake()
	{
		base.AwakeWorkaround();
		gameOverPanel.gameObject.SetActive(false);
		hud.gameObject.SetActive(false);
		status.gameObject.SetActive(false);
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
		if (!hud.gameObject.activeSelf && currentHealth > 0 && ship != null)
		{
			uiCanvas.worldCamera = playerCamera;
			hud.gameObject.SetActive(true);
		}
		if (ship != null && Time.timeScale != 0)
		{
			if (ship.IsInvincible())
			{
				if (!status.gameObject.activeSelf)
				{
					status.gameObject.SetActive(true);
				}
				statusName.text = "Invincible";
				statusTime.localScale = new Vector3(ship.InvincibleTime / ship.invincibleTimeBase, 1, 1);
			}
			else if (status.gameObject.activeSelf)
			{
				status.gameObject.SetActive(false);
			}
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
			hud.gameObject.SetActive(false);
			gameOverHandler(this, GameTime);
		}
	}

	public static void DefaultGameOverHandler(ClassicManager manager, float time)
	{
		manager.gameOverPanel.gameObject.SetActive(true);
		manager.resultText.text = "Time: " + manager.GameTimeInString;
    }

}
