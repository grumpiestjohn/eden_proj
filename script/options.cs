using Godot;
using System;

public partial class options : Node
{

    private HSlider musicslider, sfxslider;
    private CheckBox visualeffects;
    public static options Instance { get; private set; }
    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        musicslider = GetNode<HSlider>("musicslider");
        sfxslider = GetNode<HSlider>("sfxslider");
        visualeffects = GetNode<CheckBox>("visualeffects");
        musicslider.ValueChanged += MusicSliderChanged;
        sfxslider.ValueChanged += SFXSliderChanged;
        visualeffects.Pressed += CheckBoxPressed;
        musicslider.Value = (float)savehandle.saveddata["musicvolume"] / 100;
        sfxslider.Value = (float)savehandle.saveddata["sfxvolume"] / 100;
        if (savehandle.saveddata["visualeffects"] == 1) visualeffects.ButtonPressed = true;
        else visualeffects.ButtonPressed = false;
    }
    //SLIDERS
    private static void MusicSliderChanged(double value)
    {
        savehandle.SaveNewData("musicvolume", (int)(Math.Round(value, 2) * 100));
        soundhandle.ChangeMusVolume(value);
    }
    private static void SFXSliderChanged(double value)
    {
        savehandle.SaveNewData("sfxvolume", (int)(Math.Round(value, 2) * 100));
        soundhandle.ChangeSFXVolume(value);
    }
    private static void SFXSliderStop()
    {
        soundhandle.Instance.PlaySFX("res://mus/button.ogg");
    }

    //BUTTONS
    private static void OnExitPressed()
    {
        scenehandle.Instance.ChangeScene("res://tscn/menu.tscn");
    }
    private void CheckBoxPressed()
    {
        if (visualeffects.ButtonPressed == true) savehandle.SaveNewData("visualeffects", 1);
        else savehandle.SaveNewData("visualeffects", 0);
    }
}