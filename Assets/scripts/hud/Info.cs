using UnityEngine;
using System.Collections;

public class Info : MonoBehaviour {
	public RectTransform reservedTransform;

	public virtual bool IsValid()
	{
		return reservedTransform != null;
    }

	void Awake()
	{
		AwakeWorkaround();
    }

	public void AwakeWorkaround()
	{
	}

	// Use this for initialization
	void Start() {
		StartWorkaround();
	}

	public void StartWorkaround()
	{
    }
	
	// Update is called once per frame
	void Update() {
		UpdateWorkaround();
    }

	public void UpdateWorkaround()
	{
	}

}
