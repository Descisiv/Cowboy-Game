using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    public float DesiredDistance;
    public int myTurn;
    public int TilePos;
    public TurnManager TurnManager;
    int distance;
    int dir;
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.entities++;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnManager.turn == myTurn)
        {
            distance = TurnManager.PlayerPos - TilePos;
            dir = (int)transform.localScale.x;

            if(distance > 0)
            {
                if(dir > 0)
                {
                    if(distance == DesiredDistance)
                    {
                        //switch to attack when implemented
                        Wait();
                    }
                    else
                    {
                        MoveRight();
                    }
                }
                else
                {
                    Flip();
                }
            }
            else
            {
                if (dir > 0)
                {
                    Flip();
                }
                else
                {
                    if (distance == DesiredDistance * -1)
                    {
                        //swap with attack when possible
                        Wait();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }
            }
        }
    }

    void MoveRight()
    {
        TurnManager.occupied[TilePos + TurnManager.radius] = false;
        TurnManager.occupied[TilePos + TurnManager.radius + 1] = true;
        TilePos++;
        transform.position += Vector3.right;
        TurnManager.NextTurn();
    }
    void MoveLeft()
    {
        TurnManager.occupied[TilePos + TurnManager.radius] = false;
        TurnManager.occupied[TilePos + TurnManager.radius - 1] = true;
        TilePos--;
        transform.position += Vector3.left;
        TurnManager.NextTurn();
    }

    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        TurnManager.NextTurn();
    }

    void Wait()
    {
        TurnManager.NextTurn();
    }

}
