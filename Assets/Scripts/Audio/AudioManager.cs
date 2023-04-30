using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class AudioManager : UnitySingleton<AudioManager>
{
	[SerializeField] private AudioMixerGroup MixerGroup;
	[SerializeField] private AudioSource _uiAudioSource;
	[SerializeField] private AudioSource _musicAudioSource;
	[SerializeField] private AudioSource _characterAudioSource;

	[SerializeField] private List<AudioStruct> _uiSounds;
	[SerializeField] private List<AudioStruct> _musicSounds;
	[SerializeField] private List<CharacterAudio> _characterSounds;

	public Action MusicEnd;
	
	public void PlayUISound(string name)
	{
		Play(name, _uiSounds, _uiAudioSource, false);
	}

	public void PlayMusicSound(string name, bool loop)
	{
		Play(name, _musicSounds, _musicAudioSource, loop);
		StartCoroutine(MusicEndCoroutine(_musicAudioSource.clip.length));
	}

	public void PlayCharacterSound(int characterIndex, string name)
	{
		Play($"{characterIndex}{name}", _characterSounds, _characterAudioSource, false);
	}

	private void Play(string name, List<AudioStruct> audioCollection, AudioSource source, bool loop)
	{
		AudioStruct clipToPlay = audioCollection.FirstOrDefault(s => s.Name.ToUpper().Contains(name.ToUpper()));
		if (!string.IsNullOrEmpty(clipToPlay.Name) && clipToPlay.Clip != null)
		{
			if (source.clip != clipToPlay.Clip)
			{
				source.clip = clipToPlay.Clip;
			}
			source.loop = loop;
			source.Play();
		}
	}

	private void Play(string name, List<CharacterAudio> audioCollection, AudioSource source, bool loop)
	{
		var characterAudioCollection = audioCollection.FirstOrDefault(a => a.Name.ToUpper().Contains(name.ToUpper()));
		AudioClip clipToPlay = characterAudioCollection.Clips[Random.Range(0, characterAudioCollection.Clips.Count)];
		if (clipToPlay != null)
		{
			if (source.clip != clipToPlay)
			{
				source.clip = clipToPlay;
			}
			source.loop = loop;
			source.Play();
		}
	}

	private IEnumerator MusicEndCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		MusicEnd?.Invoke();
	}
}

[Serializable]
public struct AudioStruct
{
	public string Name;
	public AudioClip Clip;
}

[Serializable]
public struct CharacterAudio
{
	public string Name;
	public List<AudioClip> Clips;
}