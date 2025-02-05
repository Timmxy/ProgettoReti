using System;
using UnityEngine;

public class PrefabTag : MonoBehaviour
{
    [SerializeField] private AvatarCategories _headerId;
    [SerializeField] private string _trailerId;

    public string GetId()
    {
        int headerInt = (int)this._headerId;
        return headerInt.ToString("D2") + this._trailerId;
    }

    public string GetTrailerId()
    {
        return this._trailerId;
    }
}

public enum AvatarCategories
{
    Gender = 1,
    Head = 2,
    Body = 3,
    Legs = 4,
    Skin = 5
}
