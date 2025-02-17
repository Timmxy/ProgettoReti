using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class SaveAvatar : MonoBehaviour
{
    [SerializeField] private TMP_Text _idText;
    [SerializeField] private Camera _screenShotCamera;  // telecamera che salva immagine dell'avatar
    
    

    private int _resolutionWidth = 64;
    private int _resolutionHeight = 64;
    private string _imageBase64;
    
    public void GenerateStringId(GameObject avatar)
    {
        // esegue screen dell'avatar da salvare
        TakeScreenshot();
        
        // genera stringa riconoscitiva dell'avatar
        string id = avatar.GetComponent<Avatar>().GetGenderId() +
                    avatar.GetComponent<Avatar>().GetHeadId() +
                    avatar.GetComponent<Avatar>().GetBodyId() +
                    avatar.GetComponent<Avatar>().GetLegsId() +
                    avatar.GetComponent<Avatar>().GetSkinId();
        
        // aggiungere System.GUID all'ID
        string guid = Guid.NewGuid().ToString();
        
        // stampa sul Canvas l'ID per copiare
        this._idText.text = id;

        // salvo id e guid in un file json, percorso generico
        //string path = Application.persistentDataPath;
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyFolder");
        Debug.Log("Path: " + path);
        SaveJson(id, guid, _imageBase64, path);
        
        // DEBUG
        Debug.Log("ID SALVATO: " + id);
    }
    
    
    // Metodo per aggiungere un nuovo avatar al JSON
    public static void SaveJson(string setId, string setGuid, string setImage, string folderPath)
    {
        //nome del file json
        string fileName = "playerData.json";

        //percorso generico combinato al nome del file
        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log("filePath in SavaJson: " + filePath);

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
        avatarList.avatars.Add(new DataModel { id = setId, guid = setGuid, image = setImage});
        Debug.Log( "avatarList: "+ avatarList.ToString());


        // Serializza l'intera lista in JSON
        string jsonString = JsonUtility.ToJson(avatarList, true);
        
        // Verifica se la cartella esiste, altrimenti la crea
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        // Scrivi il JSON nel file
        File.WriteAllText(filePath, jsonString);
        
        Debug.Log($"Nuovo avatar salvato in: {filePath}");
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
    public string id;
    public string guid;
    public string image;

}

[System.Serializable]
public class AvatarList
{
    public List<DataModel> avatars = new List<DataModel>();
}


