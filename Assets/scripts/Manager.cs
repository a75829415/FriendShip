using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour
{
	public static Manager instance;

	public Ship ship;
	public float backgroundScale;

	public Ship shipProtocal;
	public Transform obstacleProtocal;
	public float waitTimeBase;

	private float pieceScale;
	private float waitTime;

	public float WaitTime { get { return waitTime; } }

	private float beginTime = 0.0f;

	public float BeginTime { get { return beginTime; } }
	public float GameTime { get { return IsOperating() ? Time.time - beginTime : 0.0f; } }

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
		if (waitTime <= 0.0f && beginTime == 0.0f)
		{
			beginTime = Time.time;
		}
	}

	void Awake()
	{
		AwakeWorkaround();
    }

	public void AwakeWorkaround()
	{
		instance = this;
		ResetWaitTime();
	}

	// Use this for initialization
	void Start()
	{
		StartWorkaround();
    }

	public void StartWorkaround()
	{
		pieceScale = 256;
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
		}
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
