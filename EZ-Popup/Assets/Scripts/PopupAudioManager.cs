using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAudioManager : MonoBehaviour {
  public PopupManager popup_manager_ref_;

  [SerializeField]
  AudioSource popup_sounds_source_;

  [SerializeField]
  AudioSource popup_music_source_;

  [SerializeField]
  AudioSource popup_text_source_;

  List<AudioSource> other_asources_;
  List<float> other_asources_backup_;

  void Awake() {
    other_asources_ = new List<AudioSource>();
    other_asources_backup_ = new List<float>();
  }

  public void ConfigureAudioSources(PopupSkin setting) {
    popup_music_source_.clip = setting.popup_background_music_;
    popup_text_source_.clip = setting.popup_text_sound_;

    popup_music_source_.volume = setting.popup_music_volume_;
    popup_sounds_source_.volume = setting.popup_sfx_volume_;

    if (setting.stop_external_audio_sources_) { 
      FindOtherAudioSources();
      SaveOtherAudioSources();
    }
  }

  public void StartMusic() {
    if (popup_music_source_.clip != null) popup_music_source_.Play();
  }

  public void StopMusic() {
    if (popup_music_source_.clip != null) popup_music_source_.Stop();
  }

  public void PlaySFX(AudioClip sfx_clip = null) {
    if (sfx_clip != null) {
      popup_sounds_source_.clip = sfx_clip;
      popup_sounds_source_.Play();
    }
  }

  public void PlayTextSFX(AudioClip sfx_clip = null) {
    if (sfx_clip != null) {
      popup_text_source_.clip = sfx_clip;
      popup_text_source_.pitch = Random.Range(popup_manager_ref_.current_canvas_skin_.text_min_pitch_variation_,
                                              popup_manager_ref_.current_canvas_skin_.text_max_pitch_variation_);
      
      popup_text_source_.Play();
    }
  }



  void SaveOtherAudioSources() {
    for (int i = 0; i < other_asources_.Count; i++) {
      AudioSource asource = other_asources_[i];
      if (asource != null) {
        if (asource.isPlaying) other_asources_backup_[i] = asource.volume;
        other_asources_[i].volume = 0.0f;
      }
    }
  }

  public void RestoreOtherAudioSources() {
    for (int i = 0; i < other_asources_.Count; i++) {
      AudioSource asource = other_asources_[i];
      if (asource != null) {
        other_asources_[i].volume = other_asources_backup_[i];
      }
    }
  }

  void FindOtherAudioSources() {
    AudioSource[] other_asources = FindObjectsOfType<AudioSource>();
    other_asources_.Clear();
    other_asources_backup_.Clear();

    foreach (AudioSource a in other_asources) {
      if (a != popup_text_source_ && a != popup_music_source_ && a != popup_sounds_source_) {
        other_asources_.Add(a);
        other_asources_backup_.Add(a.volume);
      }
    }
  }
 }
