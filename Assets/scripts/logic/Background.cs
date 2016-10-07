using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {
	public static Background instance;

	public BackgroundPiece[] backgroundPieces;

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start () {
		foreach (BackgroundPiece currentPiece in backgroundPieces)
		{
			currentPiece.Regenerate();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
