using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static Utility;

public class DisplayAvatarLoader : MonoBehaviour
{
    [SerializeField] private GameObject _button;
    [SerializeField] private GameObject _canvasAvatarCreator;
    [SerializeField] private GameObject _canvasAvatarLoader;
    [SerializeField] private Transform _contentTransform;
    [SerializeField] private string _url;

    public void LoadJson()
    {
        StartCoroutine(LoadJsonCoroutine());
    }

    private IEnumerator LoadJsonCoroutine()
    {
        //nome del file 
        string nomeFile = "playerData.json";
        //percoso generico del file
        string filePath = Path.Combine(Application.persistentDataPath, nomeFile);
        // prendo il json dal database
        yield return StartCoroutine(GetJsonFromDatabase(filePath));

        _canvasAvatarCreator.SetActive(false);
        _canvasAvatarLoader.SetActive(true);

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
                    // per ciascun avatar, istanzio bottone nel pannello Load 

                    // istanzia il prefab del bottone come figlio del content (scrollview)
                    GameObject temp = Instantiate(_button, _contentTransform);
                    //  carico img
                    temp.GetComponentInChildren<UnityEngine.UI.Image>().sprite = Base64ToSprite(avatarInfo.ImagePath);
                    // associo id
                    temp.GetComponent<LoadAvatar>().SetAvatarId(avatarInfo.IdAvatar);

                    // Opzionale: Reset di posizione, scala e rotazione
                    _button.transform.localPosition = Vector3.zero;
                    _button.transform.localScale = Vector3.one;
                }
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

    private IEnumerator GetJsonFromDatabase(string filePath)
    {
        // è possibile specificare l'id chiamando /download_avatars.php?id=xxx
        using (UnityWebRequest request = new UnityWebRequest(String.Concat(_url, "/download_avatars.php"), "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dati inviati con successo! Risposta: " + request.downloadHandler.text);
                AvatarList avatarList = JsonUtility.FromJson<AvatarList>(request.downloadHandler.text);
                try
                {
                    // Serializza l'oggetto AvatarList in JSON
                    string jsonString = JsonUtility.ToJson(avatarList, true); // 'true' per formattarlo in modo leggibile

                    // Scrive il JSON nel file
                    File.WriteAllText(filePath, jsonString);

                    Debug.Log($"File JSON salvato con successo in: {filePath}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Errore durante il salvataggio del JSON: {ex.Message}");
                }
            }
            else
            {
                Debug.LogError("Errore nell'invio: " + request.error);
            }
        }
    }

    private Sprite Base64ToSprite(string base64)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        return texture.LoadImage(imageBytes) ? TextureToSprite(texture) : null;
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

