using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;
using UnityEditor.Rendering;

public class LoadAvatar : MonoBehaviour
{
    //[SerializeField] private InputField _input;
    
    public void LoadJson()
    {
        string filePath = "C:/Users/j.derosa/Documents/TEST/data.json";
        // Verifica se il file esiste
        if (File.Exists(filePath))
        {
            try
            {
                // Legge il contenuto del file JSON
                string jsonString = File.ReadAllText(filePath);

                // Deserializza il JSON in un oggetto DataModel
                DataModel data = JsonUtility.FromJson<DataModel>(jsonString);

                Debug.Log("Dati JSON caricati con successo!");

                string id = data.id;

                /*if (id.Length != 10)
                {
                    Debug.LogError("ID non valido. Deve contenere esattamente 10 cifre.");
                    return;
                }*/

                // Estrazione delle cifre (2 per ogni caratteristica)
                string genderCode = id.Substring(0, 4);
                string headCode = id.Substring(4, 4);
                string bodyCode = id.Substring(8, 4);
                string legsCode = id.Substring(12, 4);
                string skinCode = id.Substring(16, 4);
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
    
    public GameObject FindAssetsInFolder(string code)
    {
        string category = code.Substring(0,2);
        string subCategory = code.Substring(2, 2);

        string folderPath = null; // Cartella da cercare
        switch (category)
        {
            case "02":
                folderPath = "Assets/Prefabs/Head";
                break;
            case "03":
                folderPath = "Assets/Prefabs/Body";
                break;
            case "04":
                folderPath = "Assets/Prefabs/Legs";
                break;
            case "05":
                folderPath = "Assets/Prefabs/Skin";
                break;
        }
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab.GetComponent<PrefabTag>().GetTrailerId() == subCategory)
            {
                //assegnare questo prefab trovato all'avatar
            }
            Debug.Log($"Trovato Prefab: {prefab.name} in {assetPath}");
        }

        return null;
    }
}
