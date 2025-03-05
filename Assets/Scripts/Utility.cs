using System.Collections.Generic;
using System;
using UnityEngine;

public class Utility
{
    [System.Serializable]
    public class DataModel
    {
        public string IdAvatar;
        public string GUID;
        public string ImagePath;
    }

    [System.Serializable]
    public class AvatarList
    {
        public List<DataModel> avatars = new List<DataModel>();
    }

    [System.Serializable]
    public class ImageJson
    {
        public string ImageBase64;
    }


    public class ExistingIdException : Exception
    {
        public ExistingIdException() : base("ERRORE: questo ID e' gia' presente nel DATABASE")
        {
        }
    }

    [Serializable]
    public class AvatarListRPM
    {
        public List<DataModelRPM> avatarsRPM = new List<DataModelRPM>();
    }

    [Serializable]
    public class DataModelRPM
    {
        public string id_museo;
        public int id_totem;
        public string url_glb;
    }
}
