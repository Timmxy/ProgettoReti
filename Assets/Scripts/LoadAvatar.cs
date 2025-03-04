using UnityEngine;
using UnityEditor;

public class LoadAvatar : MonoBehaviour
{ 
    private Avatar _avatar;     // riferimento all'avatar in scena (script)
    private string _avatarId;   // codice avatar


    private void OnEnable()
    {
        _avatar = GameObject.FindGameObjectWithTag("Avatar").GetComponent<Avatar>();
    }


    public void LoadAvatarSettings()
    {
        Debug.Log(_avatarId);
        // suddivide id in parti uguali (4 per ogni categoria) e applico cambiamenti per ciascuna categoria
        ApplyChangesToAvatar(_avatarId.Substring(0, 4));
        ApplyChangesToAvatar(_avatarId.Substring(4, 4));
        ApplyChangesToAvatar(_avatarId.Substring(8, 4));
        ApplyChangesToAvatar(_avatarId.Substring(12, 4));
        //ApplyChangesToAvatar(_avatarId.Substring(16, 4));

        Debug.Log("Avatar caricato correttamente!");
    }

    // applica gli asset letti nell'id all'avatar
    private void ApplyChangesToAvatar(string code)
    {
        // suddivide stringa in 2 sottostringhe, la prima indica la categoria e la seconda il tag del prefab
        string category = code.Substring(0,2);
        string subCategory = code.Substring(2, 2);
        
        switch (category)
        {
            case "01" :
                _avatar.SetGender(subCategory);
                break;
            case "02":
                // assegno il prefab trovato alla sezione giusta dell'avatar
                _avatar.SetHead(GetCorrectPrefab("Prefabs/Head", subCategory));
                break;
            case "03":
                _avatar.SetBody(GetCorrectPrefab("Prefabs/Body", subCategory));
                break;
            //case "04":
                //_avatar.SetLegs(GetCorrectPrefab("Assets/Prefabs/Legs", subCategory));
              //  break;
            case "05":
                _avatar.SetSkinMaterial(GetCorrectPrefab("Prefabs/Skin", subCategory));
                break;
        }
    }

    // dentro alla cartella passata cerca un prefab con id corrispondente e lo ritorna 
    private GameObject GetCorrectPrefab(string folderPath, string subCategory)
    {
        GameObject[] objectsArray = Resources.LoadAll<GameObject>(folderPath);

        foreach (GameObject prefab in objectsArray)
        {
            if (prefab.GetComponent<PrefabTag>().GetTrailerId() == subCategory)
            {
                Debug.Log($"Trovato Prefab: {prefab.name}");
                return prefab;
            }
        }
        Debug.Log("Se sono qui vuol dire che qualcosa e' andato storto.");
        return null;
    }

    public void SetAvatarId(string id)
    {
        this._avatarId = id;
    }
}
