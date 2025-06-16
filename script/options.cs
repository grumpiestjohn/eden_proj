using Godot;
using System;

public partial class Options : Node
{

    private HSlider musicslider, sfxslider;
    private CheckBox visualeffects;
    public static Options Instance { get; private set; }
    public override void _Ready()
    {
        SceneHandle.Instance.UpdateLocalizedSprites();
        if (Instance == null) Instance = this;
        musicslider = GetNode<HSlider>("musicslider");
        sfxslider = GetNode<HSlider>("sfxslider");
        visualeffects = GetNode<CheckBox>("visualeffects");
        musicslider.Value = (float)SaveHandle.saveddata["musicvolume"] / 100;
        sfxslider.Value = (float)SaveHandle.saveddata["sfxvolume"] / 100;
        if (SaveHandle.saveddata["visualeffects"] == 1) visualeffects.ButtonPressed = true;
        else visualeffects.ButtonPressed = false;
    }
    //SLIDERS
    private static void MusicSliderChanged(double value)
    {
        SaveHandle.SaveNewData("musicvolume", (int)(Math.Round(value, 2) * 100));
        SoundHandle.ChangeMusVolume(value);
    }
    private static void SFXSliderChanged(double value)
    {
        SaveHandle.SaveNewData("sfxvolume", (int)(Math.Round(value, 2) * 100));
        SoundHandle.ChangeSFXVolume(value);
    }
    private static void SFXSliderStop()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
    }

    //BUTTONS
    private static void OnBackPressed()
    {
        if (GlobalOverlay.Instance.TweenWorking) return;
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        SceneHandle.Instance.ChangeScene("menu");
    }
    private static void OnLocalePressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        if (SaveHandle.saveddata["localization"] == 1)
        {
            SaveHandle.SaveNewData("localization", 0);
            TranslationServer.SetLocale("en");
            SceneHandle.Instance.UpdateLocalizedSprites();
        }
        else if (SaveHandle.saveddata["localization"] == 0)
        {
            SaveHandle.SaveNewData("localization", 1);
            TranslationServer.SetLocale("uk");
            SceneHandle.Instance.UpdateLocalizedSprites();
        }
    }
    private void CheckBoxPressed()
    {
        SoundHandle.Instance.PlaySFX("res://sfx/button.ogg");
        if (visualeffects.ButtonPressed == true) SaveHandle.SaveNewData("visualeffects", 1);
        else SaveHandle.SaveNewData("visualeffects", 0);
    }
}