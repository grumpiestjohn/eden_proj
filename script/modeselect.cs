using Godot;
using System;

public partial class ModeSelect : Node
{
    public override void _Ready()
    {
        SceneHandle.Instance.UpdateLocalizedSprites();
    }
    public override void _Process(double delta)
    {
        if (GetNode<Button>("story_mode").IsHovered())
        {
            GetNode<AnimatedSprite2D>("story1").Visible = true;
            GetNode<AnimatedSprite2D>("story2").Visible = true;
        }
        else
        {
            GetNode<AnimatedSprite2D>("story1").Visible = false;
            GetNode<AnimatedSprite2D>("story2").Visible = false;
        }
        if (GetNode<Button>("custom_night").IsHovered())
        {
            GetNode<AnimatedSprite2D>("phone1").Visible = true;
            GetNode<AnimatedSprite2D>("phone2").Visible = true;
        }
        else
        {
            GetNode<AnimatedSprite2D>("phone1").Visible = false;
            GetNode<AnimatedSprite2D>("phone2").Visible = false;
        }
        if (GetNode<Button>("endless_night").IsHovered())
        {
            GetNode<AnimatedSprite2D>("inf1").Visible = true;
            GetNode<AnimatedSprite2D>("inf2").Visible = true;
        }
        else
        {
            GetNode<AnimatedSprite2D>("inf1").Visible = false;
            GetNode<AnimatedSprite2D>("inf2").Visible = false;
        }
    }
    private void OnExitPressed()
    {
        if (GlobalOverlay.Instance.TweenWorking) return;
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        SceneHandle.Instance.ChangeScene("menu");
    }
    private void OnStoryModePressed()
    {
        if (GlobalOverlay.Instance.TweenWorking) return;
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        if (SaveHandle.saveddata["nightcompleted"] == 0)
        {
            GameProcessHandle.curr_night = 1;
            SceneHandle.Instance.ChangeScene("cutscene");
            SoundHandle.FadeMusicOut();
        }
        else
        {
            SceneHandle.Instance.ChangeScene("storyselect");
        }
    }
}
