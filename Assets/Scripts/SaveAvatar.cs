using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Utility;

public class SaveAvatar : MonoBehaviour
{
    [SerializeField] private Camera _screenShotCamera;  // telecamera che salva immagine dell'avatar
    [SerializeField] private string _url;
    
    private int _resolutionWidth = 64;
    private int _resolutionHeight = 64;
    private string _imageBase64;
    private string _imagePath;
    private bool _waitUploadImage;

    
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
        StartCoroutine(SaveJson(id, guid, _imageBase64, path));
    }
    
    
    // Metodo per aggiungere un nuovo avatar al JSON
    public IEnumerator SaveJson(string setId, string setGuid, string setImage, string folderPath)
    {
        ImageJson imageJson = new ImageJson { ImageBase64 = _imageBase64 };
        string jsonString = JsonConvert.SerializeObject(imageJson, Formatting.Indented);
        Debug.Log(jsonString);
        StartCoroutine(UpdateImage(jsonString));

        yield return new WaitUntil (() => _waitUploadImage);

        _waitUploadImage = false;
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
            avatarListDatabase.avatars.Add(new DataModel { IdAvatar = setId, GUID = setGuid, ImagePath = _imagePath });

            // Serializza il JSON con Newtonsoft.Json (serve quando hai molti caratteri, con JsonUtility troncava)
            jsonString = JsonConvert.SerializeObject(avatarList, Formatting.Indented);
            string jsonAvatarString = JsonConvert.SerializeObject(avatarListDatabase, Formatting.Indented);

            // Scrivi il JSON nel file
            File.WriteAllText(filePath, jsonString);
            //sW.Write(jsonString);

            Debug.Log($"Nuovo avatar salvato in: {filePath}");

            // faccio partire la coroutine per salvare json nel DB del server
            StartCoroutine(UpdateDatabase(jsonAvatarString));
        }
        catch (ExistingIdException ex)
        {
            Debug.LogException(ex);
        }
    }


    // Coroutine per salvare contenuti json nel database del server
    private IEnumerator UpdateDatabase(string jsonData)
    {
        yield return null;
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(String.Concat(_url, "/insert_avatars.php"), "POST"))
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

    private IEnumerator UpdateImage(string _imageBase64)
    {
        byte[] jsonToSend = Encoding.UTF8.GetBytes(_imageBase64);

        using (UnityWebRequest request = new UnityWebRequest(String.Concat(_url, "/upload_image.php"), "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dati inviati con successo al DB! Risposta: " + request.downloadHandler.text);
                _imagePath = request.downloadHandler.text;
                _waitUploadImage = true;
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
