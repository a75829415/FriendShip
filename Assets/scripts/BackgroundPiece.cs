using UnityEngine;
using System.Collections;

public class BackgroundPiece : MoveableObject {
	public int bounds;
	public float minScale;
	public float maxScale;
	private Transform[,] obstacles;

	// Use this for initialization
	void Start () {
		base.StartWorkaround();
        obstacles = new Transform[bounds, bounds];
		for (int i = 0; i < bounds; ++i)
		{
			for (int j = 0; j < bounds; ++j)
			{
				obstacles[i, j] = Instantiate(Manager.instance.obstacleProtocal);
				obstacles[i, j].SetParent(reservedTransform);
			}
		}
		Regenerate();
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
		obstacles[i, j].position = new Vector3(GenerateObstacleX(i), obstacles[i, j].position.y, GenerateObstacleZ(j));
		float scale = Random.Range(minScale, maxScale) / reservedTransform.lossyScale.x;
		obstacles[i, j].localScale = new Vector3(scale, 1, scale);
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
