using UnityEngine;

public class PrefabTag : MonoBehaviour
{
    [SerializeField] private AvatarCategories _headerId;
    [SerializeField] private string _trailerId;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}

public enum AvatarCategories
{
    Gender = 01,
    Head = 02,
    Body = 03,
    Legs = 04,
    Skin = 05
}
