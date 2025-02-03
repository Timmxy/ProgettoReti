using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [SerializeField] private bool _gender;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _body;
    [SerializeField] private MeshRenderer _skin;

    public void SetGender(bool gender)
    {
        _gender = gender;
    }

    public void SetHead (GameObject head)
    {
        _head = head;
    }

    public void SetBody (GameObject body)
    {
        _body = body;
    }

    public void SetSkinColor (List<Color> color)
    {
        _skin.GetComponent<Mesh>().SetColors(color);
    }
}
