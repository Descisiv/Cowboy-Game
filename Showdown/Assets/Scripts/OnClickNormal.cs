using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickNormal : MonoBehaviour
{
    public player Player;
    public int BulletTypeToSet;
public void ClickButton()
    {
        if (!Player.loading)
        {
            Player.bulletTypeCurrent = BulletTypeToSet;
        }
    }
}
