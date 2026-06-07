using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Slider")]
    public Slider musicSlider;
    public Slider soundSlider;

    private void Start()
    {
        // Load volume yang tersimpan
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);

        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;

        bgmSource.volume = musicVolume;
        sfxSource.volume = soundVolume;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
    }

    public void SetMusicVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSoundVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }
}