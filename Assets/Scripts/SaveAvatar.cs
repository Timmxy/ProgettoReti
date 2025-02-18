using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class SaveAvatar : MonoBehaviour
{
    [SerializeField] private Camera _screenShotCamera;  // telecamera che salva immagine dell'avatar
    [SerializeField] private string url;
    
    private int _resolutionWidth = 64;
    private int _resolutionHeight = 64;
    private string _imageBase64;

    
    public void GenerateStringId(GameObject avatar)
    {
        // esegue screen dell'avatar da salvare
        TakeScreenshot();

        Avatar avatarComponent = avatar.GetComponent<Avatar>();
        // genera stringa riconoscitiva dell'avatar
        string id = avatarComponent.GetGenderId() + 
                    avatarComponent.GetHeadId() +
                    avatarComponent.GetBodyId() +
                    avatarComponent.GetLegsId() +
                    avatarComponent.GetSkinId();

        // aggiungere System.GUID all'ID
        string guid = Guid.NewGuid().ToString();

        // salvo id e guid in un file json, percorso generico del folder path
        string path = Application.persistentDataPath;
        Debug.Log("Path: " + path);
        SaveJson(id, guid, _imageBase64, path);
    }
    
    
    // Metodo per aggiungere un nuovo avatar al JSON
    public void SaveJson(string setId, string setGuid, string setImage, string folderPath)
    {
        //nome del file json
        string fileName = "playerData.json";

        //percorso generico combinato al nome del file
        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log("filePath (in SavaJson): " + filePath);

        // Lista degli avatar esistenti
        AvatarList avatarList = new AvatarList();
        // Lista che contiene SOLO l'avatar da inviare al DB
        AvatarList avatarListDatabase = new AvatarList();
        
        // Se il file esiste, leggilo e deserializzalo
        if (File.Exists(filePath))
        {  
            string existingJson = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingJson)) // Evita di deserializzare un file vuoto
            {
                avatarList = JsonUtility.FromJson<AvatarList>(existingJson) ?? new AvatarList();
            }
            // controllo se il file e' in sola lettura
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }
        }

        // controlla se esiste gia' nel file un id uguale al corrente
        try
        {
            foreach (var avatar in avatarList.avatars)
            {
                if (avatar.IdAvatar == setId)
                {
                    throw new ExistingIdException();
                }
            }

            // Aggiungi il nuovo avatar alla lista
            avatarList.avatars.Add(new DataModel { IdAvatar = setId, GUID = setGuid, ImagePath = _imageBase64 });
            avatarListDatabase.avatars.Add(new DataModel { IdAvatar = setId, GUID = setGuid, ImagePath = _imageBase64 });

            // Serializza il JSON con Newtonsoft.Json (serve quando hai molti caratteri, con JsonUtility troncava)
            string jsonString = JsonConvert.SerializeObject(avatarList, Formatting.Indented);
            string jsonAvatarString = JsonConvert.SerializeObject(avatarListDatabase, Formatting.Indented);

            // Scrivi il JSON nel file
            File.WriteAllText(filePath, jsonString);
            //sW.Write(jsonString);

            Debug.Log($"Nuovo avatar salvato in: {filePath}");

            // faccio partire la coroutine per salvare json nel DB del server
            StartCoroutine(UpdateDatabase(jsonAvatarString));
        }
        catch (Exception)
        {
            Debug.Log("ERRORE: questo avatar è già stato salvato!!!");
        }
    }


    // Coroutine per salvare contenuti json nel database del server
    private IEnumerator UpdateDatabase(string jsonData)
    {
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dati inviati con successo al DB! Risposta: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Errore nell'invio al DB: " + request.error);
            }
        }
    }

    public void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshot());
    }

    private IEnumerator CaptureScreenshot()
    {
        // Crea una RenderTexture temporanea
        RenderTexture renderTexture = new RenderTexture(_resolutionWidth, _resolutionHeight, 24);
        
        // Assegna la RenderTexture alla camera
        _screenShotCamera.targetTexture = renderTexture;
        _screenShotCamera.Render();

        // Cattura l'immagine
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(_resolutionWidth, _resolutionHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, _resolutionWidth, _resolutionHeight), 0, 0);
        screenshot.Apply();

        // Ripristina lo stato della camera
        _screenShotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Converti l'immagine in byte e poi in stringa, per salvare nel json
        byte[] imageBytes = screenshot.EncodeToPNG();
        _imageBase64 = Convert.ToBase64String(imageBytes);

        yield return null;
    }
} 



[System.Serializable]
public class DataModel
{
    public string IdAvatar;
    public string GUID;
    public string ImagePath;
}

[System.Serializable]
public class AvatarList
{
    public List<DataModel> avatars = new List<DataModel>();
}

public class ExistingIdException : Exception
{

}

