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

public class SaveAvatarRPM : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldUrl; // InputField per l'IP del server
    [SerializeField] private string _idMuseo;
    [SerializeField] private int _idTotem;
    [SerializeField] private string _urlGlb;

    public void SaveJsonRPM()
    {
        string path = Application.persistentDataPath;
        Debug.Log("Path: " + path);
        SaveJson(path);
    }

    public void SaveJson(string folderPath)
    {
        string fileName = "playerData.json";
        string filePath = Path.Combine(folderPath, fileName);
        Debug.Log("filePath (in SaveJson): " + filePath);

        AvatarListRPM avatarList = new AvatarListRPM();
        AvatarListRPM avatarListDatabase = new AvatarListRPM();

        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                avatarList = JsonUtility.FromJson<AvatarListRPM>(existingJson) ?? new AvatarListRPM();
            }
        }

        try
        {
            // Aggiunta dell'avatar con i nuovi campi richiesti
            avatarList.avatarsRPM.Add(new DataModelRPM
            {
                id_museo = _idMuseo,
                id_totem = _idTotem,
                url_glb = _urlGlb
            });

            avatarListDatabase.avatarsRPM.Add(new DataModelRPM
            {
                id_museo = _idMuseo,
                id_totem = _idTotem,
                url_glb = _urlGlb
            });

            string jsonString = JsonConvert.SerializeObject(avatarList, Formatting.Indented);
            string jsonAvatarString = JsonConvert.SerializeObject(avatarListDatabase, Formatting.Indented);

            File.WriteAllText(filePath, jsonString);
            Debug.Log($"Nuovo avatar salvato in: {filePath}");

            StartCoroutine(UpdateDatabase(jsonAvatarString));
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private IEnumerator UpdateDatabase(string jsonData)
    {
        yield return null;
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonData);

        string tmp = String.Concat("https://", this._inputFieldUrl.text);
        using (UnityWebRequest request = new UnityWebRequest(String.Concat(tmp, "/insert_avatarsRPM.php"), "POST"))
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
}