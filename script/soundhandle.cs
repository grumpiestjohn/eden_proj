using Godot;
using System;

public partial class soundhandle : Node
{
    AudioStreamPlayer musicinstance = new AudioStreamPlayer();
    AudioStreamPlayer sfxinstance = new AudioStreamPlayer();
    private AudioStream musicname;
    private AudioStream sfxname;
    public static float musvolume;
    public static float sfxvolume;
    public static float mastervolume;
    public static soundhandle Instance { get; private set; }
    //BOOT
    public override void _Ready()
    {
        //INSTANCE ACTIVATION
        if (Instance == null) Instance = this;
        //MUSICINSTANCE ACTIVATION
        musicinstance.Bus = "music";
        AddChild(musicinstance);
        //SFXINSTANCE ACTIVATION
        sfxinstance.Bus = "sfx";
        AddChild(sfxinstance);
        //INITIAL MUSIC
        PlayMusic("res://mus/testing.ogg");
    }
    //MAIN
    public void PlayMusic(string newmusicpath)
    {
        AudioStream musicname = GD.Load<AudioStream>(newmusicpath);
        musicinstance.Stream = musicname;
        musicinstance.Play();
    }
    public void PlaySFX(string newsfxpath)
    {
        AudioStream sfxname = GD.Load<AudioStream>(newsfxpath);
        sfxinstance.Stream = sfxname;
        sfxinstance.Play();
    }
    //CHANGE VOLUME FUNCTIONS
    public static void ChangeMusVolume(double value)
    {
        soundhandle.musvolume = (float)value;
        GD.Print("Music volume to " + value);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("music"), Mathf.LinearToDb(soundhandle.musvolume));
    }
    public static void ChangeSFXVolume(double value)
    {
        soundhandle.sfxvolume = (float)value;
        GD.Print("SFX volume to " + value);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("sfx"), Mathf.LinearToDb(soundhandle.sfxvolume));
    }
    public static void ChangeMasterVolume(double value)
    {
        soundhandle.mastervolume = (float)value;
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), Mathf.LinearToDb(soundhandle.mastervolume));
    }
}