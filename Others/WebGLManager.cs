using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;

using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class JsonResponse
{
    public string data;
    public string user;
}

public class WebGLManager : MonoBehaviour
{
    //     //////////////////////
    //     // WEBGL IMPORT
    //     //////////////////////
    [DllImport("__Internal")]
    private static extern void CallReact(string userName, int score);

    // [DllImport("__Internal")]
    // private static extern void WalletLogin();

    [DllImport("__Internal")]
    private static extern void OpenCharacterDialog();

    [DllImport("__Internal")]
    private static extern void ShareScore(int score, string selectedImage);

    [DllImport("__Internal")]
    private static extern void UpdateScore(string user);

    [DllImport("__Internal")]
    private static extern void UpdateName(string userName);

    public string baseUrl = "https://54.159.10.224";

    // public string baseUrl2 = "http://10.0.0.164:8080";
    public string gameName = "battlegunz";
    public string inputName = "";
    public string imageUrl;
    public string historyId;
    public string ordinalAddress;
    public string walletAddress;
    public bool authenticated;
    public string eventId;
    public string discordId;
    public string user;
    public string isMobile = "mobile";
    public bool isEth = false;

    public string encryptedScore;

    private static WebGLManager _instance;

    //     public Scene currentScene;
    //     private UIManager _uiManager;

    public static WebGLManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WebGLManager>();
            }

            return _instance;
        }
    }

    //////////////////////
    // WEB GL
    //////////////////////
    // UNITY TO REACT
    public void HandleUpdateScore()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
                UpdateScore(user);
#endif
    }

    public void HandleShareScore()
    {
        int theScore = PlayerPrefs.GetInt("shareScore", 0);
        string selectedImage = PlayerPrefs.GetString(
            "selectedImage",
            "doge_1786_532510_07d4df1c07f73cd374a989734e882cb638fa60a1ec1a5b98138df417c87468cbi0"
        );
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            ShareScore (theScore, selectedImage);
#endif
    }

    //     public void HandleOpenLeaderboard()
    //     {
    // #if UNITY_WEBGL == true && UNITY_EDITOR == false
    //             OpenLeaderboard ();
    // #endif
    //     }

//     public void HandleWalletLogin()
//     {
// #if UNITY_WEBGL == true && UNITY_EDITOR == false
//             WalletLogin ();
// #endif
//     }

    public void HandleOpenCharacterDialog()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            OpenCharacterDialog ();
#endif
    }

    //     public void HandleUpdateName()
    //     {
    //         int score = PlayerPrefs.GetInt("shareScore", 0);
    //         string scoreData =
    //             $"{historyId}#{ordinalAddress}#{encryptedScore}#{inputName}#{eventId}#{discordId}#finish#{gameName}#{isEth}";
    //         // string scoreData = historyId + "#" + "bc1pkeuk7p2qlck7t0ah7t3xehaq8epfzrp3c425hp0ywsp2uyz0uzkq8z7gfz" + "#" + score + "#" + nameField.text + "#" + eventId + "#" + discordId + "#" + discordHandle;
    //         string encrypted = EncryptString("JesusIsTheKing11", scoreData);
    //         SendEncryptedScore(encrypted, true);
    //     }

    //     // REACT TO UNITY
    //     public void UpdateEventId(string paramString)
    //     {
    //         string[] parameters = paramString.Split(',');
    //         eventId = parameters[0];
    //         discordId = parameters[1];
    //     }

    public void UpdateBaseUrl(string serverEnv)
    {
        baseUrl = serverEnv;
    }

    public void UpdateCharacter(string paramString)
    {
        // Debug.Log($"[UpdateCharacter] paramString: {paramString}");
        imageUrl = paramString;
        string[] parameters = paramString.Split(',');
        // ChangePlayerSprite(parameters[0], parameters[1]);
        PlayerPrefs.SetString("selectedImage", parameters[1]);
        isMobile = parameters[2];
    }

    public void UpdateText(string inputText)
    {
        string formattedAddress;
        if (string.IsNullOrEmpty(inputText))
        {
            formattedAddress = "....";
            ordinalAddress = "....";
        }
        else
        {
            if (inputText.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                isEth = true;
            }
            formattedAddress =
                inputText.Substring(0, 4) + "...." + inputText.Substring(inputText.Length - 4);
            ordinalAddress = inputText;
        }

        PlayerPrefs.SetString("walletAddress", formattedAddress);

        if (formattedAddress != "....")
        {
            authenticated = true;
        }
        else
        {
            authenticated = false;
        }
    }

    //////////////////////
    // NORMAL FUNCTIONS
    //////////////////////

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("destroyed");
    }

    public void initiateScoreUpdate()
    {
        string selectedImage = PlayerPrefs.GetString(
            "selectedImage",
            "doge_1786_532510_07d4df1c07f73cd374a989734e882cb638fa60a1ec1a5b98138df417c87468cbi0"
        );
        int score = 0;
        string scoreData = $"{ordinalAddress}#{selectedImage}#0#start#{gameName}#{isEth}";
        string encrypted = EncryptString("JesusIsTheKing11", scoreData);
        SendEncryptedScore(encrypted, false);
    }

    public void finishScoreUpdate()
    {
        encryptedScore = EncryptString("JesusIsTheKing11", "10");
        string scoreData =
            $"{historyId}#{ordinalAddress}#{encryptedScore}#{inputName}#{eventId}#{discordId}#finish#{gameName}#{isEth}";
        string encrypted = EncryptString("JesusIsTheKing11", scoreData);
        SendEncryptedScore(encrypted, true);
    }

    // public void GameStart()
    // {
    //     string selectedImage = PlayerPrefs.GetString(
    //         "selectedImage",
    //         "doge_1786_532510_07d4df1c07f73cd374a989734e882cb638fa60a1ec1a5b98138df417c87468cbi0"
    //     );
    //     int score = 10;
    //     string scoreData = $"{ordinalAddress}#{selectedImage}#0#start#{gameName}#{isEth}";
    //     string encrypted = EncryptString("JesusIsTheKing11", scoreData);
    //     SendEncryptedScore(encrypted, false);
    // }

    // public void GameOver()
    // {
    //     string selectedImage = PlayerPrefs.GetString(
    //         "selectedImage",
    //         "doge_1786_532510_07d4df1c07f73cd374a989734e882cb638fa60a1ec1a5b98138df417c87468cbi0"
    //     );
    //     int score = PlayerPrefs.GetInt("shareScore", 0);
    //     string scoreData =
    //         $"{historyId}#{ordinalAddress}#{encryptedScore}#{inputName}#{eventId}#{discordId}#finish#{gameName}#{isEth}";
    //     string encrypted = EncryptString("JesusIsTheKing11", scoreData);
    //     SendEncryptedScore(encrypted, true);
    // }

    public static string EncryptString(string key, string plainText)
    {
        byte[] iv = new byte[16];
        byte[] array;

        // Make sure key is exactly 16 bytes
        key = key.Length > 16 ? key.Substring(0, 16) : key.PadRight(16, '0');

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (
                    CryptoStream cryptoStream = new CryptoStream(
                        (Stream)memoryStream,
                        encryptor,
                        CryptoStreamMode.Write
                    )
                )
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public void SendEncryptedScore(string encryptedScoreInput, bool isUpdate)
    {
        if (this != null)
        {
            StartCoroutine(SendEncryptedScoreToServer(encryptedScoreInput, isUpdate));
        }
    }

    private IEnumerator SendEncryptedScoreToServer(string encryptedScoreInput, bool isUpdate)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", encryptedScoreInput);

        if (isUpdate == false)
        {
            using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/play/score", form))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    // Get the response data
                    string response = www.downloadHandler.text;
                    JsonResponse jsonResponse = JsonUtility.FromJson<JsonResponse>(response);
                    string encryptedString = jsonResponse.data;
                    // Now you can use the response data
                    // If it's encrypted, you'll need to decrypt it
                    string decryptedResponse = DecryptString("GodLovesTheWorld", encryptedString);
                    historyId = decryptedResponse;
                }
                else
                {
                    Debug.Log(www.error);
                    Debug.LogWarning("Failed to send score");
                }
            }
        }
        else
        {
            // For example, the data you want to send
            string data = "{\"data\":\"" + encryptedScoreInput + "\"}";

            // Convert the string data to byte array
            byte[] byteData = System.Text.Encoding.UTF8.GetBytes(data);
            using (UnityWebRequest www = UnityWebRequest.Put($"{baseUrl}/play/score", byteData))
            {
                // Specify that we are sending JSON data
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    if (response.Contains("_id"))
                    {
                        user = response;
                        HandleUpdateScore();
                    }
                }
                else if (
                    www.result == UnityWebRequest.Result.ConnectionError
                    || www.result == UnityWebRequest.Result.ProtocolError
                )
                {
                    Debug.LogWarning("Failed to send encrypted score to server");
                }
            }
        }
    }

    public static string DecryptString(string key, string cipherText)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        // Make sure key is exactly 16 bytes
        key = key.Length > 16 ? key.Substring(0, 16) : key.PadRight(16, '0');

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (
                    CryptoStream cryptoStream = new CryptoStream(
                        (Stream)memoryStream,
                        decryptor,
                        CryptoStreamMode.Read
                    )
                )
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}
