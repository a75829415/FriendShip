using System.Collections;

public static class Configuration {
	public static readonly int[] playerNumberSet = { 1, 2, 4 };
	public static readonly float[] invincibleTimeSet = { 3.0f, 5.0f, 10.0f };
	public static readonly float[] miniViewSizeSet = { 0.20f, 0.25f, 0.30f, 0.35f };
    public static readonly string[] miniViewSizeTextSet = { "最小", "较小", "较大", "最大" };

    public static uint health = 3;

	public static int indexOfPlayers = 0;
	public static int indexOfInvincibleTime = 0;

	public static GameMode mode = GameMode.Classic;

	public static bool enableMiniView = true;
	public static int indexOfMiniViewSize = 2;

    public static bool swapViews = false;

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

	public static float MiniViewSize
	{
		get
		{
			if (indexOfMiniViewSize < 0 || indexOfMiniViewSize >= miniViewSizeSet.Length)
			{
				indexOfMiniViewSize = 2;
			}
			return miniViewSizeSet[indexOfMiniViewSize];
		}
	}

    public static string MiniViewSizeText
    {
        get
        {
            if (indexOfMiniViewSize < 0 || indexOfMiniViewSize >= miniViewSizeSet.Length)
            {
                indexOfMiniViewSize = 2;
            }
            return miniViewSizeTextSet[indexOfMiniViewSize];
        }
    }
}