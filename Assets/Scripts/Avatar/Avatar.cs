using System;
using UnityEngine;

public class Avatar : MonoBehaviour
{
    [Header("Avatar Settings")]
    [SerializeField] private string _gender;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _legs;
    [SerializeField] private GameObject _skin;
    
    [SerializeField] private MeshRenderer[] _bodyPartsMesh;

    [SerializeField] private GameObject _instantiateParent;
    
    [Header("Default Man Preset")]
    [SerializeField] private GameObject _defaultHeadM;
    [SerializeField] private GameObject _defaultBodyM;
    [SerializeField] private GameObject _defaultLegsM;
    [SerializeField] private Material _defaultSkinMaterialM;
    
    
    [Header("Default Woman Preset")]
    [SerializeField] private GameObject _defaultHeadW;
    [SerializeField] private GameObject _defaultBodyW;
    [SerializeField] private GameObject _defaultLegsW;
    [SerializeField] private Material _defaultSkinMaterialW;

    private void Start()
    {
        SetGender("01");
    }
    
    public void SetGender(string gender)
    {
        _gender = gender;

        if (_gender.Equals("01"))
        {
            SetHead(_defaultHeadM);
            SetBody(_defaultBodyM);
            SetLegs(_defaultLegsM);

            foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
            {
                bodyPartMesh.material = _defaultSkinMaterialM;
            }
        }
        else if (_gender.Equals("02"))
        {
            SetHead(_defaultHeadW);
            SetBody(_defaultBodyW);
            SetLegs(_defaultLegsW);
            
            foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
            {
                bodyPartMesh.material = _defaultSkinMaterialW;
            }
        }
    }

    public void SetHead (GameObject head)
    {
        Destroy(_head);
        _head = Instantiate(head, gameObject.transform);
    }

    public void SetBody (GameObject body)
    {
        Destroy(_body);
        _body = Instantiate(body, gameObject.transform);
    }
    
    public void SetLegs (GameObject legs)
    {
        Destroy(_legs);
        _legs = Instantiate(legs, gameObject.transform);
    }

    public void SetSkinMaterial (GameObject skin)
    {
        Destroy(_skin);
        this._skin = Instantiate(skin, this._instantiateParent.transform);
        foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
        {
            bodyPartMesh.material = _skin.GetComponent<MeshRenderer>().material;
        }
    }

    public string GetGenderId()
    {
        int genderHeader = (int)AvatarCategories.Gender;
        return genderHeader.ToString("D2") + this._gender.ToString();
    }

    public string GetHeadId()
    {
        return this._head.GetComponent<PrefabTag>().GetId();
    }

    public string GetBodyId()
    {
        return this._body.GetComponent<PrefabTag>().GetId();
    }

    public string GetLegsId()
    {
        return this._legs.GetComponent<PrefabTag>().GetId();
    }

    public string GetSkinId()
    {
        return this._skin.GetComponent<PrefabTag>().GetId();
    }
}
