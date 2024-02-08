using UnityEngine;
using TMPro;

public class PlayerLeaderBoardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickname, killedAmount, deathAmount;

    public void Set(string nickname, int kills, int deaths)
    {
        this.playerNickname.text = nickname;
        this.killedAmount.text = kills.ToString();
        this.deathAmount.text = deaths.ToString();
    }
}
