using Godot;
using System;

public partial class StorySelect : Control
{
    private int selectednight = SaveHandle.saveddata["nightcompleted"];
    public override void _Ready()
    {
        SceneHandle.Instance.UpdateLocalizedSprites();
        GetNode<RichTextLabel>("nightname").Text = "[center]" + Tr($"NIGHT_{selectednight}") + "[/center]";
    }
    private void OnRightPressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        if (selectednight < 7 && selectednight <= SaveHandle.saveddata["nightcompleted"])
        {
            selectednight++;
            GetNode<RichTextLabel>("nightname").Text = "[center]" + Tr($"NIGHT_{selectednight}") + "[/center]";
        }
    }
    private void OnLeftPressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        if (selectednight > 1)
        {
            selectednight--;
            GetNode<RichTextLabel>("nightname").Text = "[center]" + Tr($"NIGHT_{selectednight}") + "[/center]";
        }
    }
    private void OnStartPressed()
    {
        if (GlobalOverlay.Instance.TweenWorking) return;
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        GameProcessHandle.curr_night = selectednight;
        SceneHandle.Instance.ChangeScene("cutscene");
        SoundHandle.FadeMusicOut();
    }
    private void OnBackPressed()
    {
        if (GlobalOverlay.Instance.TweenWorking) return;
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        SceneHandle.Instance.ChangeScene("modeselect");
    }
}
