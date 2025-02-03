using UnityEngine;

public class Avatar : MonoBehaviour
{
    [SerializeField] private bool _gender;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _body;
    [SerializeField] private Mesh _skin;

    public SetGender(bool gender)
    {
        _gender = gender;
    }

    public SetHead (GameObject head)
    {
        _head = head;
    }

    public SetBody (GameObject body)
    {
        _body = body;
    }

    public SetSkinColor ()
    {
        
    }
}
