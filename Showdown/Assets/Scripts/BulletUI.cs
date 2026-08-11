using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUI : MonoBehaviour
{
    public Image DisplayBullet;
    public Color[] bulletTypeColors;
    public Image[] fills;
    public Image[] slots;
    public Sprite[] BulletTypes;
    public player Player;
    public List<int> loadedBullets;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        loadedBullets = new List<int> { };
        for (int i = 0; i < Player.bulletTypes.Length; i++)
        {
            if (Player.bulletTypes[i] != -1)
            {
                loadedBullets.Add(Player.bulletTypes[i]);
            }
        }
        DisplayBullet.sprite = BulletTypes[Player.bulletTypeCurrent];

        for(int i = 0; i < fills.Length; i++)
        {
            fills[i].enabled = false;
        }
        for (int j = 0; j < fills.Length; j++)
        {
            if (Player.bullets > j)
            {
                fills[j].enabled = true;
                fills[j].color = bulletTypeColors[loadedBullets[j]];
            } 
        }
    }
}
