using System.Collections.Generic;
using UnityEngine;

public class StarLevelUI : MonoBehaviour
{
    private int _level = 0;
    [SerializeField] private List<GameObject> stars = new();

    public void SetUp(int level)
    {
        _level = level;
        Refresh();
    }

    public void Refresh()
    {
        HideAllStars();
        for (int i = 0; i < _level; i++)
        {
            stars[i].SetActive(true);
        }
    }

    private void HideAllStars()
    {
        foreach (var star in stars)
        {
            star.SetActive(false);
        }
    }
}