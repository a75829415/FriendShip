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
	public static string TimeToString(float time) {
		return string.Format("{0:F3}", time);
	}

	public RectTransform status;
	public Text statusName;
	public RectTransform statusTime;

	public delegate void CrashHandler(Collider shipCollider, Collider obstacleCollider);
	public CrashHandler crashHandler = DefaultCrashHandler;

	public bool IsOperating()
	{
		return waitTime <= 0.0f;
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
			if (Time.timeScale != 0)
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
					if (status.gameObject.activeSelf)
					{
						status.gameObject.SetActive(false);
					}
				}
				if (IsOperating())
				{
					gameTime += Time.deltaTime;
				}
			}
			if (NetHub.instance.isServer)
			{
				UpdateWaitTime();
				if (ship.Position.x < -PieceBound())
				{
					NetHub.instance.RpcMoveTowardEast();
				}
				else if (ship.Position.x > PieceBound())
				{
					NetHub.instance.RpcMoveTowardWest();
				}
				else if (ship.Position.z < -PieceBound())
				{
					NetHub.instance.RpcMoveTowardNorth();
				}
				else if (ship.Position.z > PieceBound())
				{
					NetHub.instance.RpcMoveTowardSouth();
				}
				else
				{
					UpdateClient();
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
		ship.MoveVertically(PieceScale(), 0);
		foreach (BackgroundPiece currentPiece in Background.instance.backgroundPieces)
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
		ship.MoveVertically(0, -PieceScale());
		foreach (BackgroundPiece currentPiece in Background.instance.backgroundPieces)
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
		ship.MoveVertically(-PieceScale(), 0);
		foreach (BackgroundPiece currentPiece in Background.instance.backgroundPieces)
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
		ship.MoveVertically(0, PieceScale());
		foreach (BackgroundPiece currentPiece in Background.instance.backgroundPieces)
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
