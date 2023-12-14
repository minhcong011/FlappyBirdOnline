using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PlayerData
{

    public PlayerData(string id, string name)
    {
        this.id = id;
        this.name = name;
        isDelete = false;
    }
    public PlayerData()
    {
        name = "No Name";
    }
    public float x, y, rotationZ;
    public string name, id;
    public bool isDelete;
    public int highScore;
}
