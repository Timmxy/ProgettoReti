using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering.Universal.Internal;

public class LoadAvatar : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField _input;   // textfield da dove prendo id da caricare
    [SerializeField] private Avatar _avatar;    // riferimento all'avatar in scena
    [SerializeField] private GameObject _testSprite;
    
    private const int NumCategories = 5;    // numero di categorie customizzabili
    
    
    public void LoadJson()
    {
        string filePath = "C:/Users/j.derosa/Documents/TEST/data.json"; // path del json
        string id = null;   // id asset dell'avatar
        
        
        // verifica se il file esiste
        if (File.Exists(filePath))
        {
            try
            {
                // legge il contenuto del file JSON
                string jsonString = File.ReadAllText(filePath);

                // deserializza il JSON in un array di oggetti DataModel
                AvatarList data = JsonUtility.FromJson<AvatarList>(jsonString);

                // DEBUG
                Debug.Log("Dati JSON caricati con successo!");
                

                // estrae id dal json deserializzato
                // TODO: aggiungere anche GUID
                foreach (DataModel avatarInfo in data.avatars)
                {
                    // se è l'id che cerco
                    if (avatarInfo.id.Equals(_input.text))
                    {
                       // carico gli asset corrispondenti
                       id = avatarInfo.id;
                       _testSprite.GetComponent<Image>().sprite = Base64ToSprite(avatarInfo.image);
                    }
                }

                // controllo lunghezza stringa id
                /*if (id.Length != 10)
                {
                    Debug.LogError("ID non valido. Deve contenere esattamente 10 cifre.");
                    return;
                }*/

                // suddivide id in parti uguali (4 per ogni categoria) e applico cambiamenti per ciascuna categoria
                ApplyChangesToAvatar(id.Substring(0, 4));
                ApplyChangesToAvatar(id.Substring(4, 4));
                ApplyChangesToAvatar(id.Substring(8, 4));
                ApplyChangesToAvatar(id.Substring(12, 4));
                ApplyChangesToAvatar(id.Substring(16, 4));
                
                Debug.Log("Avatar caricato correttamente!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Errore durante la lettura del JSON: {ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"File JSON non trovato nel percorso: {filePath}");
        }

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
                _avatar.SetHead(GetCorrectPrefab("Assets/Prefabs/Head", subCategory));
                break;
            case "03":
                _avatar.SetBody(GetCorrectPrefab("Assets/Prefabs/Body", subCategory));
                break;
            case "04":
                _avatar.SetLegs(GetCorrectPrefab("Assets/Prefabs/Legs", subCategory));
                break;
            case "05":
                _avatar.SetSkinMaterial(GetCorrectPrefab("Assets/Prefabs/Skin", subCategory));
                break;
        }
    }

    // dentro alla cartella passata cerca un prefab con id corrispondente e lo ritorna 
    private GameObject GetCorrectPrefab(string folderPath, string subCategory)
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab.GetComponent<PrefabTag>().GetTrailerId() == subCategory)
            {
                Debug.Log($"Trovato Prefab: {prefab.name} in {assetPath}");
                return prefab;
            }
        }
        Debug.Log("Se sono qui vuol dire che qualcosa e' andato storto.");
        return null;
    }
    
    public Sprite Base64ToSprite(string base64)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(imageBytes))
        {
            return TextureToSprite(texture);
        }
        return null;
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
