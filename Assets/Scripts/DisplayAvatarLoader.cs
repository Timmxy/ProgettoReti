using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Utility;

public class DisplayAvatarLoader : MonoBehaviour
{
    [SerializeField] private GameObject _button;                // bottone collegato alla logica di questo script (ButtonLoadAvatar)
    [SerializeField] private GameObject _canvasAvatarCreator;   // canvas con creatore avatar (spegnere alla pressione del tasto)
    [SerializeField] private GameObject _canvasAvatarLoader;    // canvas con avatar salvati da caricare (accendere alla pressione del bottone)
    [SerializeField] private Transform _contentTransform;       // padre sotto cui istanziare i bottoni avatar salvati

    [SerializeField] private InputField _inputFieldUrl; // inputfield in cui utente inserisce IP del server, trasformato poi in URL
    [SerializeField] private string _url;   // url per connettersi al server con DB



    // fa partire la coroutine LoadJsonCoroutine()
    public void LoadJson()
    {
        StartCoroutine(LoadJsonCoroutine());
    }

    // deserializza il json con gli avatar salvati e lo salva in locale, creando GUI per selezionare quale avatar caricare
    private IEnumerator LoadJsonCoroutine()
    {
        // nome del file da salvare in locale
        string nomeFile = "playerData.json";
        // percoso generico del file
        string filePath = Path.Combine(Application.persistentDataPath, nomeFile);
        // prende il json dal database
        yield return StartCoroutine(GetJsonFromDatabase(filePath));

        // accende e spegne le relative canvas
        _canvasAvatarCreator.SetActive(false);
        _canvasAvatarLoader.SetActive(true);

        // verifica se il file esiste gia' in locale
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

                    // opzionale: reset di posizione, scala e rotazione
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

    // coroutine per ricavare il json dal DB
    private IEnumerator GetJsonFromDatabase(string filePath)
    {
        string tmp = String.Concat("http://", this._inputFieldUrl.text);
        // web request "GET" per richiedere dati dalla tabella del DB
        // è possibile specificare l'id chiamando /download_avatars.php?id=xxx
        using (UnityWebRequest request = new UnityWebRequest(String.Concat(tmp, "/download_avatars.php"), "GET"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Dati ricevuti con successo! Risposta: " + request.downloadHandler.text);
                AvatarList avatarList = JsonUtility.FromJson<AvatarList>(request.downloadHandler.text);
                try
                {
                    // serializza l'oggetto AvatarList in JSON
                    string jsonString = JsonUtility.ToJson(avatarList, true); // 'true' per formattarlo in modo leggibile

                    // scrive il JSON nel file
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

    // converte un array di byte in Sprite
    private Sprite Base64ToSprite(string base64)
    {
        byte[] imageBytes = Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(2, 2);
        return texture.LoadImage(imageBytes) ? TextureToSprite(texture) : null;
    }

    // converte una Texture2D in una Sprite
    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}

