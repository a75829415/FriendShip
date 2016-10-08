using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompetitiveInfo : Info {
	public Text leftHealth;
	public Text leftHealthValue;
	public Text rightHealth;
	public Text rightHealthValue;

	public override bool IsValid()
	{
		return base.IsValid() && leftHealth != null && leftHealthValue != null && rightHealth != null && rightHealthValue != null;
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
				reservedTransform.gameObject.SetActive(true);
				if (Manager.instance.IsPaddlingLeft())
				{
					leftHealth.color = Color.red;
					rightHealth.color = Color.black;
				}
				else if (Manager.instance.IsPaddlingRight())
				{
					leftHealth.color = Color.black;
					rightHealth.color = Color.red;
				}
				leftHealthValue.text = ((CompetitiveManager)(Manager.instance)).currentLeftHealth + "/" + ((CompetitiveManager)(Manager.instance)).leftHealth;
				rightHealthValue.text = ((CompetitiveManager)(Manager.instance)).currentRightHealth + "/" + ((CompetitiveManager)(Manager.instance)).rightHealth;
			}
			else
			{
				reservedTransform.gameObject.SetActive(false);
			}
		}
	}

}
