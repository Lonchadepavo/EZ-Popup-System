using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "PopupSkin", menuName = "PopupSystem/Popup_skin", order = 0)]
public class PopupSkin : ScriptableObject {

  //-------POPUP GENERAL SETTINGS-------//
  [Header("Popup general settings")]
  public Sprite popup_background_image_;
  public Color poup_background_color_;

  public float popup_animation_speed_;

  public float popup_time_scale_;
  public bool close_external_canvas_;
  public bool stop_external_audio_sources_;
  //--------------------------------//

  //-------POPUP TEXT SETTINGS-------//
  [Header("Popup text settings")]
  public TMP_FontAsset popup_text_font_;

  public Color popup_text_color_;
  [Tooltip("Time between characters.")]
  public float popup_text_speed_;

  public AudioClip popup_text_sound_;

  [Range(0.0f, 2.0f)]
  public float text_min_pitch_variation_ = 1.0f;
  [Range(0.0f, 2.0f)]
  public float text_max_pitch_variation_ = 1.0f;
  //--------------------------------//

  //-------POPUP SOUND SETTINGS-------//
  [Header("Popup sound configuration")]
  public AudioClip popup_open_sound_;
  public AudioClip popup_close_sound_;
  public AudioClip popup_next_sound_;
  public AudioClip popup_background_music_;

  [Header("Volume configuration")]
  public float popup_sfx_volume_;
  public float popup_music_volume_;
  //--------------------------------//
}
