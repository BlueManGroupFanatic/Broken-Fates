﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class QuickLife : Mechanics
{

    public static QuickLife control;

    public float x_coord;
    public float y_coord;
    public int saves_used;
    public const int MAXSAVES = 3;

    // Use this for initialization
    void Awake()
    {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        saves_used = 0;
    }

    private void OnGUI()
    {
        x_coord = gameObject.transform.position.x;
        y_coord = gameObject.transform.position.y;
        string s = String.Format("Coords: {0:F2}, {1:F2} \n Saves: {2} of {3}", x_coord, y_coord, saves_used, MAXSAVES);
        GUI.Label(new Rect(10, 10, 150, 50), s);
    }

    public void Save()
    {
        saves_used += 1;
        if (saves_used > MAXSAVES)
        {
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (!File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
        {
            file = File.Create(Application.persistentDataPath + "/playerinfo.dat");
        } else
        {
            file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
        }

        PlayerData data = new PlayerData();
        data.x_coord = gameObject.transform.position.x;
        data.y_coord = gameObject.transform.position.y;
        data.saves_used = saves_used;
        data.stamina = 100;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (saves_used == 0)
        {
            return;
        }
        try
        {
            if (File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open);
                PlayerData data = (PlayerData)bf.Deserialize(file);

                saves_used = data.saves_used - 1;
                x_coord = data.x_coord;
                y_coord = data.y_coord;

                gameObject.transform.SetPositionAndRotation(new Vector2(x_coord, y_coord), new Quaternion(0,0,0,0));
            }
        } catch (IOException e)
        {
            Console.WriteLine("file does not exist: " + e);
        }
    }
}

[Serializable]
class PlayerData
{
    public float x_coord;
    public float y_coord;
    public int saves_used;
    public int stamina;
}