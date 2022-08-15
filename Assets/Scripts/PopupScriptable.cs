using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "Popup", menuName = "PopupSystem/Popup", order = 0)]
public class PopupScriptable : ScriptableObject {
  
  public int popup_id_;
  public string popup_name_;

  public string popup_text_;

  // Gameobject with sprite and animation
  public Sprite left_sprite_;
  public AnimationClip left_animation_;

  public Sprite right_sprite_;
  public AnimationClip right_animation_;

  public Sprite central_sprite_;
  public AnimationClip central_animation_;

  public UnityEvent on_popup_open_event_;
  public UnityEvent on_popup_close_event_;

  public float popup_event_delay_time_;
  [Tooltip("Time limit for the popup, this time will be start after the text finished showing. -1 for unlimited time.")]
  public float popup_time_limit_ = -1.0f;

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

  public PopupScriptable next_popup_;

  // popups should include a path to i'ts translation folder and read the text from there
}
