using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class VolumeContoller : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicMasterSlider;
    public Slider musicBGMSlider;
    public Slider musicSFXSlider;
	 
    private void Awake()
    {
        musicMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicBGMSlider.onValueChanged.AddListener(SetMusicVolume);
        musicSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }
	 
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }
	 
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
	 
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }  
}