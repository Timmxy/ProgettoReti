using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using static Utility;

public class LoadAvatarRPM : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldUrl;
    [SerializeField] private string _idMuseo;
    [SerializeField] private int _idTotem;

    // Callback per ricevere i dati
    public delegate void OnAvatarsLoaded(List<DataModelRPM> avatars);

    public void LoadAvatars()
    {
        if (_idMuseo == null)
            StartCoroutine(FetchAvatarsFromDatabase(null, null));
        else if (_idMuseo != null && _idTotem == 0)
            StartCoroutine(FetchAvatarsFromDatabase(_idMuseo, null));
        else
            StartCoroutine(FetchAvatarsFromDatabase(_idMuseo, _idTotem));
    }

    private IEnumerator FetchAvatarsFromDatabase(string idMuseo, int? idTotem)
    {
        // Costruisci l'URL base
        string baseUrl = String.Concat("https://", _inputFieldUrl.text, "/download_avatarsRPM.php");

        // Aggiungi i parametri di filtro se presenti
        StringBuilder urlBuilder = new StringBuilder(baseUrl);
        bool hasParameters = false;

        if (!string.IsNullOrEmpty(idMuseo))
        {
            urlBuilder.Append(hasParameters ? "&" : "?");
            urlBuilder.Append("id_museo=").Append(UnityWebRequest.EscapeURL(idMuseo));
            hasParameters = true;
        }

        if (idTotem.HasValue)
        {
            urlBuilder.Append(hasParameters ? "&" : "?");
            urlBuilder.Append("id_totem=").Append(idTotem.Value);
            hasParameters = true;
        }

        string finalUrl = urlBuilder.ToString();
        Debug.Log("URL richiesta: " + finalUrl);

        using (UnityWebRequest request = UnityWebRequest.Get(finalUrl))
        {
            // Imposta l'header per accettare JSON
            request.SetRequestHeader("Accept", "application/json");

            // Invia la richiesta
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // Deserializza la risposta JSON
                    string responseJson = request.downloadHandler.text;
                    Debug.Log("Risposta server: " + responseJson);

                    // Deserializza direttamente nel formato specificato
                    AvatarListRPM avatarList = JsonConvert.DeserializeObject<AvatarListRPM>(responseJson);

                    if (avatarList != null && avatarList.avatarsRPM != null)
                    {
                        Debug.Log($"Caricati {avatarList.avatarsRPM.Count} avatar RPM con successo");
                    }
                    else
                    {
                        // Se la risposta arriva in un formato diverso, prova a convertirla
                        var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
                        if (response != null && response.ContainsKey("status") && response["status"].ToString() == "success" && response.ContainsKey("avatarsRPM"))
                        {
                            // Otteniamo l'array degli avatar dalla risposta
                            var avatarsArray = JsonConvert.DeserializeObject<List<DataModelRPM>>(JsonConvert.SerializeObject(response["avatarsRPM"]));
                            if (avatarsArray != null)
                            {
                                Debug.Log($"Convertiti {avatarsArray.Count} avatar RPM con successo");
                                yield break;
                            }
                        }

                        Debug.LogError("Formato risposta non riconosciuto: " + responseJson);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    Debug.LogError("Errore durante la deserializzazione della risposta: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError($"Errore nella richiesta HTTP: {request.error}");
            }
        }
    }
}