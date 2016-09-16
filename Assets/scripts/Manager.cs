using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour
{
	public Ship ship;
	public Transform[] backgroundPieces;
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
		if (ship.reservedTransform.position.x < -PieceBound())
		{
			MoveTowardEast();
		} else if (ship.reservedTransform.position.x > PieceBound())
		{
			MoveTowardWest();
		}
		if (ship.reservedTransform.position.z < -PieceBound())
		{
			MoveTowardNorth();
		}
		else if (ship.reservedTransform.position.z > PieceBound())
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
		ship.Move(pieceScale, 0);
		foreach (Transform currentPiece in backgroundPieces)
		{
			if (currentPiece.position.x > pieceScale / 2)
			{
				currentPiece.Translate(-2 * pieceScale, 0, 0);
			}
			else
			{
				currentPiece.Translate(pieceScale, 0, 0);
			}
		}
	}

	public void MoveTowardSouth()
	{
		ship.Move(0, -pieceScale);
		foreach (Transform currentPiece in backgroundPieces)
		{
			if (currentPiece.position.z < -pieceScale / 2)
			{
				currentPiece.Translate(0, 2 * pieceScale, 0);
			}
			else
			{
				currentPiece.Translate(0, -pieceScale, 0);
			}
		}
	}

	public void MoveTowardWest()
	{
		ship.Move(0, -pieceScale);
		foreach (Transform currentPiece in backgroundPieces)
		{
			if (currentPiece.position.x < -pieceScale / 2)
			{
				currentPiece.Translate(2 * pieceScale, 0, 0);
			}
			else
			{
				currentPiece.Translate(-pieceScale, 0, 0);
			}
		}
	}

	public void MoveTowardNorth()
	{
		ship.Move(0, -pieceScale);
		foreach (Transform currentPiece in backgroundPieces)
		{
			if (currentPiece.position.z > pieceScale / 2)
			{
				currentPiece.Translate(0, -2 * pieceScale, 0);
			}
			else
			{
				currentPiece.Translate(0, pieceScale, 0);
			}
		}
	}

}
