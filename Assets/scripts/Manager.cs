using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour
{
	public static Manager instance;

	public Ship ship;
	public BackgroundPiece[] backgroundPieces;
	public float backgroundScale;

	public Transform obstacleProtocal;
	public float waitTimeBase;

	private float pieceScale;
	private float waitTime;

	public float WaitTime { get { return waitTime; } }

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
		instance = this;
	}

	// Use this for initialization
	void Start()
	{
		pieceScale = GameObject.FindGameObjectWithTag("Background").transform.localScale.x;
		ResetWaitTime();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateWaitTime();
		if (ship.Position.x < -PieceBound())
		{
			MoveTowardEast();
		} else if (ship.Position.x > PieceBound())
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
		foreach (BackgroundPiece currentPiece in backgroundPieces)
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
		foreach (BackgroundPiece currentPiece in backgroundPieces)
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
		foreach (BackgroundPiece currentPiece in backgroundPieces)
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
		foreach (BackgroundPiece currentPiece in backgroundPieces)
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

}
