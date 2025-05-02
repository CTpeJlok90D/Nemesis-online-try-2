using Core.PlayerTablets;
using IngameDebugConsole;
using UnityEngine;
using Zenject;

public class PlayerTabletCheats : MonoBehaviour
{
    [Inject] private PlayerTabletList _playerTabletList;
    
    private void Awake()
    {
        DebugLogConsole.AddCommand<string>("Pass", "Pass player", Pass);
    }

    private void Pass(string playerNickname)
    {
        Target targets = new(playerNickname, _playerTabletList);

        foreach (PlayerTablet target in targets)
        {
            target.Pass();
        }
    }
}
