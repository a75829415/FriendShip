using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour
{
	public Ship ship;
	public BackgroundPiece[] backgroundPieces;
	public float backgroundScale;

	private float pieceScale;

	// Use this for initialization
	void Start()
	{
		pieceScale = GameObject.FindGameObjectWithTag("Background").transform.localScale.x;
	}

	// Update is called once per frame
	void Update()
	{
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

	public float PieceBound()
	{
		return pieceScale / 2;
	}

	public void MoveTowardEast()
	{
		ship.MoveVertically(pieceScale, 0);
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			if (currentPiece.Position.x > PieceBound())
			{
				currentPiece.MoveVertically(-2 * pieceScale, 0);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(pieceScale, 0);
			}
		}
	}

	public void MoveTowardSouth()
	{
		ship.MoveVertically(0, -pieceScale);
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			if (currentPiece.Position.z < -PieceBound())
			{
				currentPiece.MoveVertically(0, 2 * pieceScale);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(0, -pieceScale);
			}
		}
	}

	public void MoveTowardWest()
	{
		ship.MoveVertically(-pieceScale, 0);
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			if (currentPiece.Position.x < -PieceBound())
			{
				currentPiece.MoveVertically(2 * pieceScale, 0);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(-pieceScale, 0);
			}
		}
	}

	public void MoveTowardNorth()
	{
		ship.MoveVertically(0, pieceScale);
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			if (currentPiece.Position.z > PieceBound())
			{
				currentPiece.MoveVertically(0, -2 * pieceScale);
				currentPiece.Regenerate();
			}
			else
			{
				currentPiece.MoveVertically(0, pieceScale);
			}
		}
	}

}
