using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

public class MainBase : LogistCentral
{
	private void Start()
	{
		MapManager.SetBuild(transform.position, size, this);
	}
}
