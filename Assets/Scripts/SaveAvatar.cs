using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class SaveAvatar : MonoBehaviour
{
    [SerializeField] private TMP_Text _idText;
    public void GenerateStringId(GameObject avatar)
    {
        string id = avatar.GetComponent<Avatar>().GetGenderId() +
                    avatar.GetComponent<Avatar>().GetHeadId() +
                    avatar.GetComponent<Avatar>().GetBodyId() +
                    avatar.GetComponent<Avatar>().GetLegsId() +
                    avatar.GetComponent<Avatar>().GetSkinId();
        
        // aggiungere System.GUID all'ID
        string guid = Guid.NewGuid().ToString();
        
        // stampa sul Canvas l'ID per copiare
        this._idText.text = id;
        
        // salvo id e guid in un file json
        string path = "C:/Users/j.derosa/Documents/TEST";
        SaveJson(id, guid, path);
        
        // DEBUG
        Debug.Log("ID SALVATO: " + id);
    }
    
    
    // Metodo per aggiungere un nuovo avatar al JSON
    public static void SaveJson(string setId, string setGuid, string folderPath)
    {
        string filePath = "C:/Users/j.derosa/Documents/TEST/data.json";

        // Lista degli avatar esistenti
        AvatarList avatarList = new AvatarList();
        
        // Se il file esiste, leggilo e deserializzalo
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingJson)) // Evita di deserializzare un file vuoto
            {
                avatarList = JsonUtility.FromJson<AvatarList>(existingJson) ?? new AvatarList();
            }
        }

        // Aggiungi il nuovo avatar alla lista
        avatarList.avatars.Add(new DataModel { id = setId, guid = setGuid });
        Debug.Log( "avatarList: "+ avatarList.ToString());


        // Serializza l'intera lista in JSON
        string jsonString = JsonUtility.ToJson(avatarList, true);
        Debug.Log("pezzo demmerda01: "+jsonString);
        
        // Verifica se la cartella esiste, altrimenti la crea
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // Scrivi il JSON nel file
        File.WriteAllText(filePath, jsonString);
        
        Debug.Log($"Nuovo avatar salvato in: {filePath}");
    }
    
    /*
    // Metodo per serializzare e salvare il JSON in un percorso specifico
    private static void SaveJson(string setId, string setGuid, string folderPath)
    {
        // Crea l'oggetto da serializzare
        DataModel data = new DataModel
        {
            id = setId,
            guid = setGuid
        };

        // Serializza l'oggetto in formato JSON usando JsonUtility
        string jsonString = JsonUtility.ToJson(data, true); // true = JSON formattato (pretty print)

        // Verifica se la cartella esiste, altrimenti la crea
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Definisce il percorso completo del file JSON
        string filePath = Path.Combine(folderPath, "data.json");

        // Salva il file JSON nel percorso specificato
        File.WriteAllText(filePath, jsonString);

        Debug.Log($"Dati JSON salvati con successo in: {filePath}");
    }
    */
} 

[System.Serializable]
public class DataModel
{
    public string id;
    public string guid;
}

[System.Serializable]
public class AvatarList
{
    public List<DataModel> avatars = new List<DataModel>();
}


