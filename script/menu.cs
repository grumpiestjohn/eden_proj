using Godot;
using System;

public partial class Menu : Node
{
    private AnimatedSprite2D candle;
    private bool candleprogress = false;
    public override void _Ready()
    {
        candle = GetNode<AnimatedSprite2D>("candle");
        candle.Play("burn");
        SceneHandle.Instance.UpdateLocalizedSprites();
        GlobalOverlay.Instance.FadeInOut(false, 1.5);
        SoundHandle.NewBgMusic();
    }
    //BUTTONS
    private void OnStartPressed()
    {
        if (candleprogress || GlobalOverlay.Instance.TweenWorking)return;
        candleprogress = true;
        candle.AnimationFinished += OnPutOut;
        SoundHandle.Instance.PlaySFX("res://sfx/blowout.ogg");
        candle.Play("putout1");
    }
    private void OnPutOut()
    {
        candle.AnimationFinished -= OnPutOut;
        candle.Play("putout2");
        SceneHandle.Instance.ChangeScene("modeselect");
    }
    private void OnOptionsPressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        SceneHandle.Instance.ChangeScene("options");
    }

    private void OnExitPressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        GetTree().Quit();
    }
}