using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundPiece : MoveableObject {
	public int bounds;
	public float minScale;
	public float maxScale;

	private Transform[,] obstacles;

	// Use this for initialization
	void Awake() {
		base.AwakeWorkaround();
		obstacles = new Transform[bounds, bounds];
		for (int i = 0; i < bounds; ++i)
		{
			for (int j = 0; j < bounds; ++j)
			{
				obstacles[i, j] = Instantiate(Manager.instance.obstaclePrototype);
				obstacles[i, j].SetParent(reservedTransform);
			}
		}
	}

	public void Regenerate()
	{
		for (int i = 0; i < bounds; ++i)
		{
			for (int j = 0; j < bounds; ++j)
			{
				RegenerateObstacle(i, j);
            }
		}
	}

	private void RegenerateObstacle(int i, int j)
	{
		obstacles[i, j].gameObject.SetActive(true);
        obstacles[i, j].position = new Vector3(GenerateObstacleX(i), obstacles[i, j].position.y, GenerateObstacleZ(j));
		float scale = Random.Range(minScale, maxScale);
        float xScale = scale / reservedTransform.lossyScale.x;
		float yScale = scale / reservedTransform.lossyScale.z / 8.0f;
		float zScale = scale / reservedTransform.lossyScale.y;
		obstacles[i, j].localScale = new Vector3(xScale, yScale, zScale);
    }

	private float ObstacleBoundRange()
	{
		return Manager.instance.PieceScale() / bounds;
	}

	private float GenerateObstacleX(int i)
	{
		float min = Position.x - Manager.instance.PieceBound() + i * ObstacleBoundRange();
		return Random.Range(min, min + ObstacleBoundRange());
	}

	private float GenerateObstacleZ(int j)
	{
		float min = Position.z - Manager.instance.PieceBound() + j * ObstacleBoundRange();
		return Random.Range(min, min + ObstacleBoundRange());
	}

}
