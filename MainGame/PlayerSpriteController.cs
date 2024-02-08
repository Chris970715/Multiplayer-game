using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerSpriteController : NetworkBehaviour
{


    [SerializeField] private SpriteRenderer playerCharacterSprite;

    [Networked(OnChanged = nameof(OnImageLinkChanged))] public NetworkString<_512> imageLink { get; set;}


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SendImageLInk(NetworkString<_512> inputString)
    {
        imageLink = inputString;
    }

        private static void OnImageLinkChanged(Changed<PlayerSpriteController> changed)
    {
            changed.Behaviour.onChangeLink(changed.Behaviour.imageLink);
    }

    private void onChangeLink(NetworkString<_512> theLink)
    {
            StartCoroutine(DownloadSprite(theLink));
    }

    private IEnumerator DownloadSprite(NetworkString<_512> theLink)
    {
        string url = theLink.ToString();
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // Debug.Log($"url: {url}");
            yield return www.SendWebRequest();
            // Debug.Log($"[DownloadSprite] www.result: {www.result}");
            if (www.result != UnityWebRequest.Result.Success)
            {
                // Debug.LogWarning($"Failed to download sprite from URL: {url}, Error: {www.error}");
                yield break;
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            Sprite newSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.one * 0.5f,
                80f
            );

            playerCharacterSprite.sprite = newSprite;

            if(Object.HasStateAuthority)
            {
                GlobalManagers.Instance.WebGLManager.initiateScoreUpdate();
            }

            if (newSprite == null)
            {
                Debug.LogWarning($"Failed to create sprite from downloaded texture for URL: {url}");
                yield break;
            }
        }
    }
    
    
}
