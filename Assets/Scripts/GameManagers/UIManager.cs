using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Inputs;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
	public GameObject BattleControlImagePrefab;
	public List<ActionSpritePair> inputList;

	public GameObject GetComboButton(InputAction inputAction, ControllerType controllerType)
	{
		Sprite sprite = inputList
						.FirstOrDefault(i => i.inputAction == inputAction && i.controllerType == controllerType)
						.sprite;
		return GameObject.Instantiate(BattleControlImagePrefab);
	}
}

[Serializable]
public struct ActionSpritePair
{
	public InputAction inputAction;
	public ControllerType controllerType;
	public Sprite sprite;


}
