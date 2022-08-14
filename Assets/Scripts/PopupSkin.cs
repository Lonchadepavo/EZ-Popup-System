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
