using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.PlayerTablets;
using UnityEngine;

public struct Target : IEnumerable<PlayerTablet>
{
    private List<PlayerTablet> _tablets;
    
    public IReadOnlyList<PlayerTablet> Tablets => _tablets;

    public string TargetsMessage => string.Join(", ", Tablets.Select(x => x.Nickname));
    
    public Target(string targetName, PlayerTabletList playerTabletList)
    {
        _tablets = new List<PlayerTablet>();
        
        switch (targetName)
        {
            case "@a":
                _tablets.AddRange(playerTabletList.ActiveTablets);
                break;
            case "@s":
                _tablets.Add(playerTabletList.Local);
                break;
            case "@r":
                PlayerTablet randomTablet = playerTabletList.ActiveTablets[Random.Range(0, _tablets.Count)];
                _tablets.Add(randomTablet);
                break;
            default:
                PlayerTablet playerTablet = playerTabletList.FindTabletByPlayerNickname(targetName);
                _tablets.Add(playerTablet);
                break;
        }
    }

    public override string ToString()
    {
        return TargetsMessage;
    }

    public IEnumerator<PlayerTablet> GetEnumerator()
    {
        return _tablets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_tablets).GetEnumerator();
    }
}
