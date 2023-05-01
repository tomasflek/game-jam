using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrefab
{
	int PrefabInt { get; set; }
	int PlayerIndex { get; set; }
	bool PickedUp { get; set; }
}
