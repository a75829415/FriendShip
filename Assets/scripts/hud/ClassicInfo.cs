using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClassicInfo : Info {
	public Text timeValue;
	public Text healthValue;

	public override bool IsValid()
	{
		return base.IsValid() && timeValue != null && healthValue != null;
	}

	void Awake()
	{
		base.AwakeWorkaround();
	}

	// Use this for initialization
	void Start() {
		base.StartWorkaround();
	}
	
	// Update is called once per frame
	void Update() {
		base.UpdateWorkaround();
		if (IsValid())
		{
			if (Manager.instance.IsGaming())
			{
				Show();
				timeValue.text = Manager.TimeToString(Manager.instance.GameTime);
				healthValue.text = ((ClassicManager)(Manager.instance)).currentHealth + "/" + ((ClassicManager)(Manager.instance)).health;
			}
			else
			{
				Hide();
			}
		}
	}

}
