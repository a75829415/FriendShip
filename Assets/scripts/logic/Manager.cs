using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public abstract class Manager : MonoBehaviour
{
	public static Manager instance;

	public Boomer boomerPrototype;

	public delegate void ResultStringHandler(string result);

	public ResultStringHandler resultStringHandler = DefaultResultStringHandler;

	public ShipControlMode localControlMode = ShipControlMode.Unknown;

	public Ship ship;
	public float backgroundScale;

	public List<MoveableObject> toMoveList;

	public Ship shipPrototype;
	public Transform obstaclePrototype;
	public float waitTimeBase;

	private float pieceScale;
	private float waitTime;

	public float WaitTime { get { return waitTime; } set { waitTime = value; } }

	private float gameTime = 0.0f;

	public float GameTime { get { return gameTime; } set { gameTime = value; } }
	public static string TimeToString(float time) {
		return string.Format("{0:F3}", time);
	}

	public delegate void CrashHandler(ShipCollider shipCollider, Collider obstacleCollider);
	public CrashHandler crashHandler = DefaultCrashHandler;

	public abstract bool IsGaming();

	public bool IsOperating()
	{
		return waitTime <= 0.0f;
	}

	public bool IsPaddlingLeft()
	{
		return localControlMode == ShipControlMode.LeftPaddleOnly;
	}

	public bool IsPaddlingRight()
	{
		return localControlMode == ShipControlMode.RightPaddleOnly;
	}

	public abstract bool IsObstacle(Collider collider);

	public void ResetWaitTime()
	{
		waitTime = waitTimeBase;
	}

	public virtual GameMode GetGameMode()
	{
		return GameMode.Unset;
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
		Time.timeScale = 1.0f;
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
			if (IsGaming() && IsOperating())
			{
				gameTime += Time.deltaTime;
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

	public void RegisterToMove(MoveableObject toRegister)
	{
		toMoveList.Add(toRegister);
	}

	public void UnregisterToMove(MoveableObject toUnregister)
	{
		toMoveList.Remove(toUnregister);
    }

	public void RegisterShip(Ship shipToRegister)
	{
		ship = shipToRegister;
		RegisterToMove(ship);
		InitializeShipCollider();
    }

	public abstract void InitializeShipCollider();

	public virtual void UpdateClient()
	{
		if (NetHub.instance.isServer)
		{
			NetHub.instance.RpcUpdateWaitTime(WaitTime);
			if (ship != null)
			{
				NetHub.instance.RpcUpdateInvincibleTime(ship.InvincibleTime);
				NetHub.instance.RpcUpdateShip(ship.CurrentStatus);
				NetHub.instance.RpcUpdateBoomer(Boomer.GetStatuses());
            }
			NetHub.instance.RpcUpdateGameTime(GameTime);
		}
    }

	public virtual void NotifyControlMode(ShipControlMode controlMode)
	{
		localControlMode = controlMode;
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
		foreach (MoveableObject currentToMove in toMoveList)
		{
			currentToMove.MoveVertically(PieceScale(), 0);
		}
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
		foreach (MoveableObject currentToMove in toMoveList)
		{
			currentToMove.MoveVertically(0, -PieceScale());
		}
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
		foreach (MoveableObject currentToMove in toMoveList) {
			currentToMove.MoveVertically(-PieceScale(), 0);
		}
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
		foreach (MoveableObject currentToMove in toMoveList)
		{
			currentToMove.MoveVertically(0, PieceScale());
		}
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

	public virtual void NotifyCrash(ShipCollider shipCollider, Collider obstacleCollider)
	{
		crashHandler(shipCollider, obstacleCollider);
	}

	public static void DefaultCrashHandler(ShipCollider shipCollider, Collider obstacleCollider)
	{
	}

	public static void DefaultResultStringHandler(string result)
	{
		Debug.Log(result);
	}

}
