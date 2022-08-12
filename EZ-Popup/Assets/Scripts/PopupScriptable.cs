using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Popup", menuName = "PopupSystem/Popup", order = 0)]
public class PopupScriptable : ScriptableObject {
  
  public int popup_id_;
  public string popup_name_;

  public string popup_text_;

  // Gameobject with sprite and animation
  public GameObject left_image_;
  public GameObject right_image_;
  public GameObject animated_sprite_;

  public UnityEvent on_popup_open_event_;
  public UnityEvent on_popup_close_event_;

  public float popup_delay_time_;

  public PopupScriptable next_popup_;

  // popups should include a path to i'ts translation folder and read the text from there
}
