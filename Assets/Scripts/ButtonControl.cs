using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonControl : MonoBehaviour
{
	private Animation _anim;
	[SerializeField] private UnityEvent _clickEvent;
	[SerializeField] private string AudioName;

	public void HoverButton()
	{
		PlayAnimation("ButtonHover");
		AudioManager.Instance.PlayUISound(AudioName);
	}

	public void DehoverButton()
	{
		PlayAnimation("ButtonDehover");
	}

	public void ClickButton()
	{
		_clickEvent?.Invoke();
	}

	private void StopAnimation()
	{
		if (_anim == null)
			_anim = GetComponent<Animation>();
		_anim.Stop();
	}

	private void PlayAnimation(string name)
	{
		if(_anim == null)
			_anim = GetComponent<Animation>();
		_anim.Play(name);
	}
}
