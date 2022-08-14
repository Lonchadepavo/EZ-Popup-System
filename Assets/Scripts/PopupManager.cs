using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PopupManager : MonoBehaviour {
  //PUBLIC TEMPORAL FOR TESTING
  public PopupScriptable current_popup_;
  GameObject canvas_parent_;
  List<Canvas> other_canvas_;
  List<bool> other_canvas_backup_;
  
  public PopupSkin current_canvas_skin_;

  PopupAudioManager audio_manager_ref_;

  PlayerInput popup_input_;
  List<PlayerInput> other_inputs_;
  List<bool> other_inputs_backup_;

  [SerializeField]
  GameObject left_image_;

  [SerializeField]
  GameObject right_image_;

  [SerializeField]
  GameObject animated_image_;

  [SerializeField]
  GameObject popup_background_image_;

  [SerializeField]
  AnimationClip default_anim_;

  [SerializeField]
  TextMeshProUGUI popup_text_;

  //Text animation variables
  string additive_text_ = "";

  float current_time_scale_;
  float saved_time_scale_;

  public PopupScriptable debug_popup_;

  void Awake() {
    audio_manager_ref_ = GetComponent<PopupAudioManager>();
    audio_manager_ref_.popup_manager_ref_ = this;

    canvas_parent_ = transform.GetChild(0).gameObject;
    canvas_parent_.SetActive(false);

    other_canvas_ = new List<Canvas>();
    other_canvas_backup_ = new List<bool>();

    other_inputs_ = new List<PlayerInput>();
    other_inputs_backup_ = new List<bool>();

    popup_input_ = GetComponent<PlayerInput>();
  }

  void Start() {
    //DEBUG
    OpenPopup(debug_popup_);
  }

  void ReloadPopupCanvas() {
    Image popup_background = popup_background_image_.GetComponent<Image>();
    popup_background.sprite = current_canvas_skin_.popup_background_image_;
    popup_background.color = popup_background.color + current_canvas_skin_.poup_background_color_;

    popup_text_.color = current_popup_.popup_text_color_;
    popup_text_.font = current_popup_.popup_text_font_;
  }

  // Adds a new popup to execute it.
  public void OpenPopup(PopupScriptable popup) {
    if (current_popup_ == null) { 
      current_popup_ = popup;

      ReloadPopupCanvas();

      current_time_scale_ = current_canvas_skin_.popup_time_scale_ > 0.0f ? current_canvas_skin_.popup_time_scale_ : 0.00001f;

      audio_manager_ref_.ConfigureAudioSources(current_canvas_skin_);

      //Configure popup canvas with the current popup data. (could add some dotween here)
      ConfigureSpriteInfo(current_popup_.left_animation_, current_popup_.left_sprite_, left_image_);
      ConfigureSpriteInfo(current_popup_.right_animation_, current_popup_.right_sprite_, right_image_);
      ConfigureSpriteInfo(current_popup_.central_animation_, current_popup_.central_sprite_, animated_image_);

      popup_text_.text = "";
      StartCoroutine(ShowText());

      //Prepare the popup screen (if there is any canvas opened, it stores its state and closes it, after the popup is closed it restores the previous canvas)
      if (current_popup_.on_popup_open_event_ != null) StartCoroutine(PopupEventDelay(current_popup_.on_popup_open_event_));

      if (!canvas_parent_.activeSelf) {
        if (current_canvas_skin_.close_external_canvas_) { 
          FindOtherCanvas();
          SaveOtherCanvas();
        }

        FindOtherInputs();
        SaveOtherInputs();

        saved_time_scale_ = Time.timeScale;
        Time.timeScale = current_time_scale_;

        //Open popup sound
        audio_manager_ref_.PlaySFX(current_canvas_skin_.popup_open_sound_);

        //Start popup music
        audio_manager_ref_.StartMusic();

        canvas_parent_.GetComponent<Animator>().SetBool("PopupIn", true);
      }

    } else {
      Debug.Log("A popup is already open.");
    }
  }

  public void NextPopup() {
    if (current_popup_.on_popup_close_event_ != null) StartCoroutine(PopupEventDelay(current_popup_.on_popup_close_event_));

    if (current_popup_.next_popup_ != null) {
      PopupScriptable next_popup = current_popup_.next_popup_;

      current_popup_ = null;
      OpenPopup(next_popup);

      // Next popup sound
      audio_manager_ref_.PlaySFX(current_canvas_skin_.popup_next_sound_);

    } else {
      ClosePopup();

      // Close popup sound
      audio_manager_ref_.PlaySFX(current_canvas_skin_.popup_close_sound_);
    }
  }

  public void ClosePopup() {
    //Close popup code (could add some dotween here)
    if (canvas_parent_.activeSelf) canvas_parent_.SetActive(false);
    
    current_popup_ = null;

    //Need a time stop here
    canvas_parent_.GetComponent<Animator>().SetBool("PopupIn", false);

    RestoreOtherCanvas();
    RestoreOtherInputs();
    audio_manager_ref_.RestoreOtherAudioSources();

    Time.timeScale = saved_time_scale_;
    saved_time_scale_ = 0.0f;

    //Stop popup music
    audio_manager_ref_.StopMusic();
  }

  public void PopupControls(InputAction.CallbackContext context) {
    if (context.phase == InputActionPhase.Started) {
      //If the text is not fully shown, it skips the text animation.
      //If the text animation ended, it skips to the next popup.

      if (popup_text_.text.Length != current_popup_.popup_text_.Length) { 
        popup_text_.text = current_popup_.popup_text_;

      } else {
        NextPopup();

      }
      
    }
  }

  // Simple event call without parameters
  void CallSimplePopupEvent(UnityEvent e) {
    e.Invoke();
  }

  public void SetPopupSkin(PopupSkin skin) {
    current_canvas_skin_ = skin;

    ReloadPopupCanvas();
  }

  void ConfigureSpriteInfo(AnimationClip clip, Sprite sprite, GameObject target) {
    if (sprite) {
      Image target_img = target.GetComponent<Image>();

      target_img.sprite = sprite;
    }

    if (!clip) clip = default_anim_;
    Animator target_anim = target.GetComponent<Animator>();

    AnimatorOverrideController animator_override = new AnimatorOverrideController(target_anim.runtimeAnimatorController);
    var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();

    foreach (var a in animator_override.animationClips) { 
      anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, clip));
      }

    animator_override.ApplyOverrides(anims);
    target_anim.runtimeAnimatorController = animator_override;
    target_anim.speed = current_canvas_skin_.popup_animation_speed_ / current_time_scale_;

      //target_anim.runtimeAnimatorController = anim.runtimeAnimatorController;
  }

  IEnumerator ShowText() {
    int number_characters = current_popup_.popup_text_.Length;

    //Should use an event when the animation finishes
    for (int i = 0; i <= number_characters && number_characters != popup_text_.text.Length; i++) {
      additive_text_ = current_popup_.popup_text_.Substring(0, i);
      popup_text_.text = additive_text_;

      //Text sound
      audio_manager_ref_.PlayTextSFX(current_popup_.popup_text_sound_);

      yield return new WaitForSecondsRealtime(current_popup_.popup_text_speed_);
    }

    yield return null;
  }

  IEnumerator PopupEventDelay(UnityEvent e) {
    yield return new WaitForSeconds(current_popup_.popup_delay_time_);
    CallSimplePopupEvent(e);
  }

  void SaveOtherCanvas() {
    for (int i = 0; i < other_canvas_.Count; i++) {
      Canvas canvas = other_canvas_[i];
      if (canvas != null) {
        other_canvas_backup_[i] = canvas.enabled;
        other_canvas_[i].enabled = false;
      }
    }

    canvas_parent_.SetActive(true);
  }

  void RestoreOtherCanvas() {
    for (int i = 0; i < other_canvas_.Count; i++) {
      Canvas canvas = other_canvas_[i];
      if (canvas != null) {
        other_canvas_[i].enabled = other_canvas_backup_[i];
      }
    }

    canvas_parent_.SetActive(false);
  }

  //Save the state of the other inputs
  void SaveOtherInputs() {
    for (int i = 0; i < other_inputs_.Count; i++) {
      PlayerInput input = other_inputs_[i];
      if (input != null) {
        other_inputs_backup_[i] = input.enabled;
        other_inputs_[i].enabled = false;
      }
    }

    //Activate popup input
    popup_input_.enabled = true;
  }

  //Restore the state of the other inputs
  void RestoreOtherInputs() {
    for (int i = 0; i < other_inputs_.Count; i++) {
      PlayerInput input = other_inputs_[i];
      if (input != null) {
        other_inputs_[i].enabled = other_inputs_backup_[i];
      }
    }

    //Deactivate popup input
    popup_input_.enabled = false;
  }

  void FindOtherInputs() {
    PlayerInput[] other_inputs = FindObjectsOfType<PlayerInput>();
    other_inputs_.Clear();
    other_inputs_backup_.Clear();

    foreach (PlayerInput input in other_inputs) {
      if (input != popup_input_) {
        other_inputs_.Add(input);
        other_inputs_backup_.Add(input.enabled);
      }
    }
  }

  void FindOtherCanvas() {
    Canvas[] other_canvas = FindObjectsOfType<Canvas>();
    other_canvas_.Clear();
    other_canvas_backup_.Clear();

    foreach (Canvas c in other_canvas) {
      if (c != canvas_parent_.GetComponentInParent<Canvas>()) {
        other_canvas_.Add(c);
        other_canvas_backup_.Add(c.enabled);
      }
    }
  }

}
