using System.Collections;

public static class Configuration {
	public static readonly int[] playerNumberSet = { 1, 2, 4 };
	public static readonly float[] invincibleTimeSet = { 3.0f, 5.0f, 10.0f };

	public static uint health = 3;

	public static int indexOfPlayers = 0;
	public static int indexOfInvincibleTime = 0;

	public static GameMode mode = GameMode.Classic;

	public static int NumberOfPlayers
	{
		get
		{
			if (indexOfPlayers < 0 || indexOfPlayers >= playerNumberSet.Length)
			{
				indexOfPlayers = 0;
			}
			return playerNumberSet[indexOfPlayers];
		}
	}

	public static float InvincibleTime
	{
		get
		{
			if (indexOfInvincibleTime < 0 || indexOfInvincibleTime >= invincibleTimeSet.Length)
			{
				indexOfInvincibleTime = 0;
			}
			return invincibleTimeSet[indexOfInvincibleTime];
		}
	}

}