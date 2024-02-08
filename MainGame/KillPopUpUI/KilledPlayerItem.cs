using UnityEngine;
using TMPro;

public class KilledPlayerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eliminatedPlayerText;

    private void Start()
    {
        Destroy(gameObject, 3);
    }

    public void SetName(string txt)
    {
        eliminatedPlayerText.text = $" ELIMINATED <color=#FF4348>{txt}!";
    }
}
