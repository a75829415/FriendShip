using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Manager : MonoBehaviour
{
	public static Manager instance;

	public Ship ship;
	public float backgroundScale;

	public Camera playerCamera;

	public Ship shipProtocal;
	public Transform obstacleProtocal;
	public float waitTimeBase;

	private float pieceScale;
	private float waitTime;

	public float WaitTime { get { return waitTime; } set { waitTime = value; } }

	private float gameTime = 0.0f;

	public float GameTime { get { return gameTime; } set { gameTime = value; } }
	public string GameTimeInString { get { return string.Format("{0:F3}", GameTime); } }

	public RectTransform status;
	public Text statusName;
	public RectTransform statusTime;

	public delegate void CrashHandler(Collider shipCollider, Collider obstacleCollider);
	public CrashHandler crashHandler = DefaultCrashHandler;

	public bool IsOperating()
	{
		return waitTime < 0;
	}

	public void ResetWaitTime()
	{
		waitTime = waitTimeBase;
	}

	private void UpdateWaitTime()
	{
		waitTime -= Time.deltaTime;
	}

	void Awake()
	{
		AwakeWorkaround();
    }

	

	public void AwakeWorkaround()
	{
		instance = this;
		ResetWaitTime();
		pieceScale = 512;
		int seed = (int)(Random.value * int.MaxValue);
		Random.InitState(seed);
		NetHub.instance.RpcUpdateRandomSeed(seed);
        foreach (BackgroundPiece currentPiece in BackgroundPiece.backgroundPieces)
		{
			currentPiece.Regenerate();
		}
	}

	// Use this for initialization
	void Start()
	{
		StartWorkaround();
    }

	public void StartWorkaround()
	{
	}

	// Update is called once per frame
	void Update()
	{
		UpdateWorkaround();
    }

	public void UpdateWorkaround()
	{
		if (ship != null)
		{
			if (NetHub.instance.isServer)
			{
				UpdateWaitTime();
				if (ship.Position.x < -PieceBound())
				{
					MoveTowardEast();
				}
				else if (ship.Position.x > PieceBound())
				{
					MoveTowardWest();
				}
				if (ship.Position.z < -PieceBound())
				{
					MoveTowardNorth();
				}
				else if (ship.Position.z > PieceBound())
				{
					MoveTowardSouth();
				}
				UpdateClient();
			}
        }
		if (ship != null && Time.timeScale != 0)
		{
			if (ship.IsInvincible())
			{
				if (!status.gameObject.activeSelf)
				{
					status.gameObject.SetActive(true);
				}
				statusName.text = "无敌";
				statusTime.localScale = new Vector3(ship.InvincibleTime / ship.invincibleTimeBase, 1, 1);
			}
			else
			{
				if (WaitTime <= 0.0f)
				{
					gameTime += Time.deltaTime;
				}
				if (status.gameObject.activeSelf)
				{
					status.gameObject.SetActive(false);
				}
			}
		}
	}

	public virtual void UpdateClient()
	{
		NetHub.instance.RpcUpdateWaitTime(WaitTime);
		NetHub.instance.RpcUpdateInvincibleTime(ship.InvincibleTime);
		NetHub.instance.RpcUpdateGameTime(GameTime);
		NetHub.instance.RpcUpdateShip(ship.reservedTransform.position, ship.reservedTransform.eulerAngles, ship.reservedRigidbody.velocity,
			ship.reservedRigidbody.angularVelocity, ship.reservedBackgroundForce.relativeForce);
    }

	public float PieceScale()
	{
		return pieceScale;
	}

	public float PieceBound()
	{
		return PieceScale() / 2;
	}

	public void MoveTowardEast()
	{
		NetHub.instance.RpcMoveTowardEast();
		ship.MoveVertically(PieceScale(), 0);
		foreach (BackgroundPiece currentPiece in BackgroundPiece.backgroundPieces)
		{
			if (currentPiece.Position.x > PieceBound())
			{
				currentPiece.MoveVertically(-2 * PieceScale(), 0);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(PieceScale(), 0);
			}
		}
	}

	public void MoveTowardSouth()
	{
		NetHub.instance.RpcMoveTowardSouth();
		ship.MoveVertically(0, -PieceScale());
		foreach (BackgroundPiece currentPiece in BackgroundPiece.backgroundPieces)
		{
			if (currentPiece.Position.z < -PieceBound())
			{
				currentPiece.MoveVertically(0, 2 * PieceScale());
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(0, -PieceScale());
			}
		}
	}

	public void MoveTowardWest()
	{
		NetHub.instance.RpcMoveTowardWest();
		ship.MoveVertically(-PieceScale(), 0);
		foreach (BackgroundPiece currentPiece in BackgroundPiece.backgroundPieces)
		{
			if (currentPiece.Position.x < -PieceBound())
			{
				currentPiece.MoveVertically(2 * PieceScale(), 0);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(-PieceScale(), 0);
			}
		}
	}

	public void MoveTowardNorth()
	{
		NetHub.instance.RpcMoveTowardNorth();
		ship.MoveVertically(0, PieceScale());
		foreach (BackgroundPiece currentPiece in BackgroundPiece.backgroundPieces)
		{
			if (currentPiece.Position.z > PieceBound())
			{
				currentPiece.MoveVertically(0, -2 * PieceScale());
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(0, PieceScale());
			}
		}
	}

	public virtual void NotifyCrash(Collider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
	}

	public static void DefaultCrashHandler(Collider shipCollider, Collider obstacleCollider)
	{
	}

}
