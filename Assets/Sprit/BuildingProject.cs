using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameBase;

///<summary>
///½¨Öþ¹æ»®
///</summary>
public class BuildingProject : Block
{
	private Block projecting;
	private BoxCollider2D coll;
	public void SetProject(GameObject prefab)
	{
		projecting = prefab.GetComponent<Block>();
		if (!projecting) return;
		coll.size = prefab.GetComponent<BoxCollider2D>().size;

		Debug.Log(coll.size);
	}

	private void Start()
	{
		coll = GetComponent<BoxCollider2D>();
		if (!coll) coll = gameObject.AddComponent<BoxCollider2D>();
	}

	private void Update()
	{

	}

}

