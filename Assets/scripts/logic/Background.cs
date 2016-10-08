using UnityEngine;
using System.Collections;

public class Background : MoveableObject
{
	public static Background instance;

	public BackgroundPiece[] backgroundPieces;

	void Awake()
	{
		base.AwakeWorkaround();
		instance = this;
	}

	// Use this for initialization
	void Start () {
		base.AwakeWorkaround();
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			currentPiece.Regenerate();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
