using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BoomInfo : Info {
	public Text ScoreValue;
	public Text healthValue;

	public override bool IsValid()
	{
		return base.IsValid() && ScoreValue != null && healthValue != null;
	}

	void Awake()
	{
		base.AwakeWorkaround();
	}

	// Use this for initialization
	void Start()
	{
		base.StartWorkaround();
	}

	// Update is called once per frame
	void Update()
	{
		base.UpdateWorkaround();
		if (IsValid())
		{
			if (Manager.instance.IsGaming())
			{
				Show();
				ScoreValue.text = ((BoomManager)(Manager.instance)).score.ToString();
				healthValue.text = ((BoomManager)(Manager.instance)).currentHealth + "/" + ((BoomManager)(Manager.instance)).health;
			}
			else
			{
				Hide();
			}
		}
	}

}
