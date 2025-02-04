using System;
using UnityEngine;
//using System.Text.Json;

public class SaveAvatar : MonoBehaviour
{
    private string _id;

    public void GenerateStringId(GameObject avatar)
    {
        this._id = avatar.GetComponent<Avatar>().GetGenderId() +
                   avatar.GetComponent<Avatar>().GetHeadId() +
                   avatar.GetComponent<Avatar>().GetBodyId() +
                   avatar.GetComponent<Avatar>().GetLegsId() +
                   avatar.GetComponent<Avatar>().GetSkinId();
        // aggiungere System.GUID all'ID
        
        // stampa sul Canvas l'ID per copiare
        
        // salvo id e guid in un file json
        
        
        // DEBUG
        Debug.Log("ID SALVATO: "+this._id);
    }
    /*
    // Metodo per serializzare e salvare il JSON in un percorso specifico
    public static void SaveJson(string id, string guid, string folderPath)
    {
        // Crea l'oggetto da serializzare
        DataModel data = new DataModel
        {
            Id = id,
            Guid = guid
        };

        // Opzioni per una formattazione leggibile
        var options = new JsonSerializerOptions { WriteIndented = true };

        // Serializza l'oggetto in formato JSON
        string jsonString = JsonSerializer.Serialize(data, options);

        // Verifica se la cartella esiste, altrimenti la crea
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Definisce il percorso completo del file JSON
        string filePath = Path.Combine(folderPath, "data.json");

        // Salva il file JSON nel percorso specificato
        File.WriteAllText(filePath, jsonString);

        Console.WriteLine($"Dati JSON salvati con successo in: {filePath}");
    }*/
} 

public class DataModel
{
    public string Id { get; set; }
    public string Guid { get; set; }
}


