using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class player : MonoBehaviour
{
    public BulletUI bulletUI;
    Vector3 OnFlipStartScale;

    public bool HasTakenFireDamageThisTurn;
    public bool canGoToNextWave;
    public LineController lrController;
    public TurnManager TurnManager;

    public string state = "static";
    public Animator anim;
    bool moving;
    public bool loading;
    int dir;
    float i;
    public float RevolverDamage;
    public int TilePos;
    public float movingTime;
    public float flippingTime;
    public float speed;
    public float loadingTime;
    public float shootingTime;
    public float TimeTillBullet;
    public int bullets;
    //0 = normal, 1 = piercing, 2 = poison, 3 = fire, 4 = vamp
    public int bulletTypeCurrent;
    public int[] bulletTypes;
    public float bulletLifeSpan;
    public Vector3 offset;
    public LayerMask GunMask;



    private void Update()
    {
        //for debugging to automatically return turn to player
        if (Input.GetKeyDown(KeyCode.X))
        {
            TurnManager.turn = 0;
        }

        if(!HasTakenFireDamageThisTurn && TurnManager.onFire[TilePos + TurnManager.radius])
        {
            HasTakenFireDamageThisTurn = true;
            TurnManager.health -= TurnManager.FireDamage;
        }

        if(TurnManager.health <= 0)
        {
            Destroy(gameObject);
        }

        if (TurnManager.turn == 0 && !moving && state == "static")
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
            {
                if (!TurnManager.occupied[TilePos + TurnManager.radius + (int)Input.GetAxisRaw("Horizontal")])
                {
                    HasTakenFireDamageThisTurn = false;
                    TurnManager.occupied[TilePos + TurnManager.radius] = false;
                    TurnManager.occupied[TilePos + TurnManager.radius + (int)Input.GetAxisRaw("Horizontal")] = true;
                    state = "moving";
                    dir = (int)Input.GetAxisRaw("Horizontal");
                    moving = true;
                    StartCoroutine(MoveTimer());
                    TilePos = dir > 0 ? TilePos + 1 : TilePos - 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                HasTakenFireDamageThisTurn = false;
                OnFlipStartScale = transform.localScale;
                i = 0;
                state = "flipping";
                StartCoroutine(FlipTimer());
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                HasTakenFireDamageThisTurn = false;
                state = "loading";
                StartCoroutine(LoadTimer());
            }

            if (Input.GetKeyDown(KeyCode.Space) && bullets > 0)
            {
                HasTakenFireDamageThisTurn = false;
                state = "shooting";
                StartCoroutine(ShootTimer());
            }
        }

        if(state == "flipping")
        {
            i += Time.deltaTime / flippingTime;
            transform.localScale = Vector3.Lerp(OnFlipStartScale, new Vector3(OnFlipStartScale.x * -1, OnFlipStartScale.y, OnFlipStartScale.z), i);
        }

        if (moving == true)
        {
            anim.SetInteger("state", 1);
            transform.position += Vector3.right * speed * dir * Time.deltaTime;
        }
        else if(state == "loading")
        {
            anim.SetInteger("state", 3);
        }
        else if(state == "shooting")
        {
            anim.SetInteger("state", 2);
        }
        else
        {
            anim.SetInteger("state", 0);
        }
    }

    private void Shoot()
    {
        bullets--;
        
        
        List<int> loadedBullets = new List<int>{};
        for(int i = 0; i < bulletTypes.Length; i++)
        {
            if (bulletTypes[i] != -1)
            {
                loadedBullets.Add(bulletTypes[i]);
            }
        }
        for(int j = 1; j < loadedBullets.Count; j++)
        {
            bulletTypes[j - 1] = bulletTypes[j];
        }
        bulletTypes[bulletTypes.Length - 1] = -1;
        lrController.lr.startColor = bulletUI.bulletTypeColors[loadedBullets[0]];
        lrController.lr.endColor = bulletUI.bulletTypeColors[loadedBullets[0]];
        switch (loadedBullets[0]) {
            case 0:
                RevolverDamage = 10;
        RaycastHit2D GunRay = Physics2D.Raycast(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
        lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), GunRay.point + new Vector2(transform.lossyScale.x * 0.25f, 0) };
        StartCoroutine(DestroyBullet());
        lrController.lr.enabled = true;
        GameObject enemy = GunRay.collider.gameObject;

        if (enemy.layer == 7)
        {
            enemyMovement EnemyScript = enemy.GetComponent<enemyMovement>();
            EnemyScript.Health -= RevolverDamage;
        }
                break;
            case 1:
                RevolverDamage = 8;
                RaycastHit2D[] PierceRay = Physics2D.RaycastAll(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
                lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), PierceRay[0].point + new Vector2(transform.lossyScale.x * 10, 0) };
                StartCoroutine(DestroyBullet());
                lrController.lr.enabled = true;
                for (int i = 0; i < PierceRay.Length; i++)
                {
                    if (PierceRay[i].collider.gameObject.layer == 7)
                    {
                        PierceRay[i].collider.gameObject.GetComponent<enemyMovement>().Health -= RevolverDamage;
                    }
                }
                break;
            case 2:
                RevolverDamage = 6;
                RaycastHit2D PoisonRay = Physics2D.Raycast(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
                lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), PoisonRay.point + new Vector2(transform.lossyScale.x * 0.25f, 0) };
                StartCoroutine(DestroyBullet());
                lrController.lr.enabled = true;
                GameObject enemyP = PoisonRay.collider.gameObject;

                if (enemyP.layer == 7)
                {
                    enemyMovement EnemyScript = enemyP.GetComponent<enemyMovement>();
                    EnemyScript.poisonedTurnsRemaining += 4;
                    EnemyScript.Health -= RevolverDamage;
                }
                break;
            case 3:
                RevolverDamage = 2;
                RaycastHit2D FireRay = Physics2D.Raycast(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
                lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), FireRay.point + new Vector2(transform.lossyScale.x * 0.25f, 0) };
                StartCoroutine(DestroyBullet());
                lrController.lr.enabled = true;
                GameObject enemyF = FireRay.collider.gameObject;

                if (enemyF.layer == 7)
                {
                    enemyMovement EnemyScript = enemyF.GetComponent<enemyMovement>();
                    EnemyScript.Health -= RevolverDamage;
                    TurnManager.onFire[EnemyScript.TilePos + TurnManager.radius] = true;
                    TurnManager.TurnsLeftOnFire[EnemyScript.TilePos + TurnManager.radius] = 7;
                }
                break;
            case 4:
                RevolverDamage = 4;
                RaycastHit2D VampRay = Physics2D.Raycast(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
                lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), VampRay.point + new Vector2(transform.lossyScale.x * 0.25f, 0) };
                StartCoroutine(DestroyBullet());
                lrController.lr.enabled = true;
                GameObject enemyV = VampRay.collider.gameObject;

                if (enemyV.layer == 7)
                {
                    enemyMovement EnemyScript = enemyV.GetComponent<enemyMovement>();
                    EnemyScript.Health -= RevolverDamage;
                    TurnManager.health += 5;
                    TurnManager.health = Mathf.Clamp(TurnManager.health, 0, TurnManager.maxHealth);
                }
                break;
            case 5:
                RevolverDamage = 10;
                RaycastHit2D FlipRay = Physics2D.Raycast(transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), Vector3.right * transform.lossyScale.x, 10, GunMask);
                lrController.points = new Vector3[] { transform.position + new Vector3(offset.x * transform.lossyScale.x, offset.y, offset.z), FlipRay.point + new Vector2(transform.lossyScale.x * 0.25f, 0) };
                StartCoroutine(DestroyBullet());
                lrController.lr.enabled = true;
                GameObject enemyFl = FlipRay.collider.gameObject;

                if (enemyFl.layer == 7)
                {
                    enemyMovement EnemyScript = enemyFl.GetComponent<enemyMovement>();
                    EnemyScript.Health -= RevolverDamage;
                }
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                break;
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(bulletLifeSpan);
        lrController.lr.enabled = false;
    }

    IEnumerator ShootTimer()
    {
        canGoToNextWave = false;
        yield return new WaitForSeconds(TimeTillBullet);
        Shoot();
        yield return new WaitForSeconds(shootingTime - TimeTillBullet);
        if (bullets > 0)
        {
            StartCoroutine(ShootTimer());
        }
        else
        {
            TurnManager.NextTurn();
            TurnManager.NewRound();
            state = "static";
        }
        canGoToNextWave = true;
    }

    IEnumerator LoadTimer()
    {
        canGoToNextWave = false;
        loading = true;
        yield return new WaitForSeconds(loadingTime);
        if (bullets < 6)
        {
            bulletTypes[bullets] = bulletTypeCurrent;
            bullets++;
        }
        state = "static";
        TurnManager.NextTurn();
        TurnManager.NewRound();
        loading = false;
        canGoToNextWave = true;
    }
    IEnumerator MoveTimer()
    {
        canGoToNextWave = false;
        Vector3 StartPos = transform.position;
        yield return new WaitForSeconds(movingTime);
        moving = false;
        transform.position = StartPos + Vector3.right * speed * dir * movingTime;
        state = "static";
        TurnManager.NextTurn();
        TurnManager.NewRound();
        canGoToNextWave = true;
    }
    IEnumerator FlipTimer()
    {
        canGoToNextWave = false;
        Vector3 StartScale = transform.localScale;
        yield return new WaitForSeconds(flippingTime);
        transform.localScale = new Vector3(StartScale.x * -1, StartScale.y, StartScale.z);
        state = "static";
        TurnManager.NextTurn();
        TurnManager.NewRound();
        canGoToNextWave = true;
    }
}
