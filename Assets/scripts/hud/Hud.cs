﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hud : MonoBehaviour {
	public RectTransform reservedTransform;

	public ClassicInfo classicInfoPrototype;
	public CompetitiveInfo competitiveInfoPrototype;
	public BoomInfo boomInfoPrototype;

	public RectTransform statusBar;
	public Text statusName;
	public RectTransform statusDuration;

	public RectTransform gameOverDialog;
	public Text gameOverResult;
	public Text backToLobbyButtonText;

	public Info info;

	// Use this for initialization
	void Start()
	{
		gameOverDialog.gameObject.SetActive(false);
		Manager.instance.resultStringHandler = (string result) =>
		{
			ShowGameOverDialog(result);
		};
		Info prototype;
		switch (Manager.instance.GetGameMode())
		{
			case GameMode.Classic:
				prototype = classicInfoPrototype;
				break;

			case GameMode.Competitive:
				prototype = competitiveInfoPrototype;
				break;

			case GameMode.Boom:
				prototype = boomInfoPrototype;
				break;

			default:
				prototype = null;
				break;

		}
		if (prototype != null)
		{
			info = Instantiate(prototype);
			info.reservedTransform.SetParent(reservedTransform, false);
        }
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateStatusBar();
    }

	private void UpdateStatusBar()
	{
		bool statusBarActivated = false;
		if (Manager.instance.IsGaming())
		{
			if (Manager.instance.IsOperating())
			{
				if (Manager.instance.ship.IsInvincible())
				{
					statusBarActivated = true;
					statusName.text = "无敌";
					UpdateDurationScale(Manager.instance.ship.InvincibleTime / Manager.instance.ship.invincibleTimeBase);
				}
			}
			else if (Manager.instance.WaitTime < 3.0f)
			{
				statusBarActivated = true;
				if (Manager.instance.IsPaddlingLeft())
				{
					statusName.text = "左侧准备";
				}
				else if (Manager.instance.IsPaddlingRight())
				{
					statusName.text = "右侧准备";
				}
				else if (Manager.instance.localControlMode == ShipControlMode.FireOnly)
                {
					statusName.text = "准备开火";
                }
				else
				{
					statusName.text = "准备出发";
				}
				UpdateDurationScale(Manager.instance.WaitTime / 3.0f);
			}
		}
		statusBar.gameObject.SetActive(statusBarActivated);
	}

	private void UpdateDurationScale(float duration)
	{
		statusDuration.localScale = new Vector3(duration, statusDuration.localScale.y, statusDuration.localScale.z);
	}

	private void ShowGameOverDialog(string result)
	{
		gameOverResult.text = result;
		if (NetHub.instance.isServer && Configuration.NumberOfPlayers > 1)
		{
			backToLobbyButtonText.text = "返回房间";
		}
		else
		{
			backToLobbyButtonText.text = "退出房间";
		}
		gameOverDialog.gameObject.SetActive(true);
	}

	public void BackToLobby()
	{
		uint reason;
		if (NetHub.instance.isServer && Configuration.NumberOfPlayers == 1)
		{
			reason = LobbyManager.SINGLE_GAME_OVER;
        }
		else
		{
			reason = LobbyManager.DOUBLE_GAME_OVER;
		}
		LobbyManager.instance.ChangeToLobbyScene(reason);
	}

}
