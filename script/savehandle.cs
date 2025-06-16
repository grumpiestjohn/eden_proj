using Godot;
using System;
using System.Collections.Generic;

public partial class SaveHandle : Node
{
    public static Dictionary<string, int> saveddata = new Dictionary<string, int>
    {
        { "nightcompleted", 0 },
        { "customcompleted", 0 },
        { "musicvolume", 50 },
        { "sfxvolume", 50 },
        { "visualeffects", 1 },
        { "localization", 1 }
    };
    private const string path = "res://data.dat";
    public override void _Ready()
    {
        FileAccess file = null;
        //CREATING A FILE IF IT'S MISSING
        if (!FileAccess.FileExists(path))
        {
            //GD.Print($"Save file not found at [{path}], creating a new one.");
            file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.Close();
        }
        //else GD.Print($"Save file found at [{path}], loading existing data.");
        file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string data = file.GetAsText();
        //FILLING THE FILE WITH DEFAULT VALUES IF EMPTY
        if (data == "")
        {
            foreach (KeyValuePair<string, int> line in saveddata)
            {
                SaveNewData(line.Key, line.Value);
            }
        }
        //STORING EXISTING DATA IN THE DICTIONARY
        string[] entries = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (string entry in entries)
        {
            string[] keyValue = entry.Split('=');
            if (int.TryParse(keyValue[1], out int value))
            {
                if (keyValue[0] == "nightcompleted" || keyValue[0] == "customcompleted" || keyValue[0] == "visualeffects" || keyValue[0] == "localization" || keyValue[0] == "musicvolume" || keyValue[0] == "sfxvolume")
                {
                    //GD.Print(keyValue[0] + ": " + value + " successfully added to game data.");
                    saveddata[keyValue[0]] = value;
                }
                //else GD.Print(entry + " does not follow format criteria, skipping.");
            }
            //else GD.Print(entry + " does not follow format criteria, skipping.");
        }
        //APPLYING SAVED SETTINGS
        SoundHandle.ChangeMusVolume((double)saveddata["musicvolume"] / 100);
        SoundHandle.ChangeSFXVolume((double)saveddata["sfxvolume"] / 100);
        switch (saveddata["localization"])
        {
            case 0:
                TranslationServer.SetLocale("en");
                break;
            case 1:
                TranslationServer.SetLocale("uk");
                break;
        }
        file.Close();
    }
    public static void SaveNewData(string valuename, int value)
    {
        saveddata[valuename] = value;
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        foreach (KeyValuePair<string, int> line in saveddata)
        {
            file.StoreLine(line.Key + "=" + line.Value);
        }
        file.Close();
    }
}