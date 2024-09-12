using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharGrade
{
    Epic,
    Rare,
    Common
}

[System.Serializable]
public class GachaCharacterInfo
{
    public string charName;
    public GameObject charImage;
    public CharGrade charGrade;
    public float weight;

    public GachaCharacterInfo(GachaCharacterInfo Info)
    {
        this.charName = Info.charName;
        this.charImage = Info.charImage;
        this.charGrade = Info.charGrade;
        this.weight = Info.weight;
    }
}
