using Core.PlayerTablets;
using IngameDebugConsole;
using UnityEngine;
using Zenject;

public class PlayerTabletCheats : MonoBehaviour
{
    private void Awake()
    {
        DebugLogConsole.AddCommand<string>("Pass", "Pass player", Pass);
    }

    private void Pass(string playerNickname)
    {
        Target targets = new(playerNickname);

        foreach (PlayerTablet target in targets)
        {
            target.Pass();
        }
    }
}
