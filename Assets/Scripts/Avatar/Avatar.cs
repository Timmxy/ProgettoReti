using UnityEngine;

public class Avatar : MonoBehaviour
{
    [Header("Avatar Settings")]
    [SerializeField] private bool _gender;
    [SerializeField] private GameObject _head;
    [SerializeField] private GameObject _body;
    [SerializeField] private MeshRenderer[] _bodyPartsMesh;
    
    [Header("Default Man Preset")]
    [SerializeField] private GameObject _defaultHeadM;
    [SerializeField] private GameObject _defaultBodyM;
    [SerializeField] private Material _defaultSkinMaterialM;
    
    [Header("Default Woman Preset")]
    [SerializeField] private GameObject _defaultHeadW;
    [SerializeField] private GameObject _defaultBodyW;
    [SerializeField] private Material _defaultSkinMaterialW;

    private void Start()
    {
        SetGender(true);
    }
    
    public void SetGender(bool gender)
    {
        _gender = gender;

        if (_gender)
        {
            _head = _defaultHeadM;
            _body = _defaultBodyM;

            foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
            {
                bodyPartMesh.material = _defaultSkinMaterialM;
            }
        }
        else
        {
            _head = _defaultHeadW;
            _body = _defaultBodyW;

            foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
            {
                bodyPartMesh.material = _defaultSkinMaterialW;
            }
        }
    }

    public void SetHead (GameObject head)
    {
        _head = head;
    }

    public void SetBody (GameObject body)
    {
        _body = body;
    }

    public void SetSkinMaterial (Material skinMaterial)
    {
        foreach (MeshRenderer bodyPartMesh in _bodyPartsMesh)
        {
            bodyPartMesh.material = skinMaterial;
        }
    }
}
