using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Inputs;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : UnitySingleton<UIManager>
{
	public GameObject BattleControlImagePrefab;
	public List<ActionSpritePair> inputList;

	public GameObject GetComboButton(InputAction inputAction, ControllerType controllerType)
	{
		Sprite sprite = inputList
						.FirstOrDefault(i => i.inputAction == inputAction && i.controllerType == controllerType)
						.sprite;
		GameObject go = GameObject.Instantiate(BattleControlImagePrefab);
		go.GetComponent<Image>().sprite = sprite;
		return go;
	}
}

[Serializable]
public struct ActionSpritePair
{
	public InputAction inputAction;
	public ControllerType controllerType;
	public Sprite sprite;


}