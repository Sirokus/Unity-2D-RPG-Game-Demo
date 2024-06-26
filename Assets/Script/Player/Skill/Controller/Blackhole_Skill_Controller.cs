using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> KeyCodeList;

    public float maxSize;
    public float growSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKey = true;
    private bool canAttack;

    private int attackAmount = 4;
    private float attackCoolDown = .3f;
    private float attackTimer;

    public List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    private float blackholeDuration;
    private float blackholeTimer;

    public bool playerCanExitState { get; private set; }

    public void SetupBlackhole(float _maxSize, float _growSpeed, int _attackAmount, float _attackCoolDown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        attackAmount = _attackAmount;
        attackCoolDown = _attackCoolDown;
        blackholeDuration = _blackholeDuration;

        blackholeTimer = blackholeDuration;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if (blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;

            DestroyHotKeys();

            if (targets.Count > 0)
            {
                canAttack = true;
                canCreateHotKey = false;
            }
            else
            {
                FinishBlackhole();
            }
        }

        AttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-0.1f, -0.1f), growSpeed * Time.deltaTime);

            if (transform.localScale.x <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void AttackLogic()
    {
        if (attackTimer < 0 && canAttack && attackAmount > 0)
        {
            attackTimer = attackCoolDown;
            int randIndex = Random.Range(0, targets.Count);
            float xOffset = Random.Range(0f, 1f) > 0.5f ? -1 : 1;

            if (SkillManager.instance.clone.crystalInseadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CrystalControllerChooseRandomTarget(SkillManager.instance.blackhole.GetBlackholeRadius());
            }
            else
            {
                PlayerManager.instance.player.sr.color = Color.clear;
                SkillManager.instance.clone.CreateClone(targets[randIndex], new Vector3(xOffset, 0));
            }

            attackAmount--;
            if (attackAmount <= 0)
            {
                Invoke("FinishBlackhole", .5f);
            }
        }
    }

    private void FinishBlackhole()
    {
        playerCanExitState = true;

        PlayerManager.instance.player.sr.color = Color.white;

        canShrink = true;
        canAttack = false;

        //PlayerManager.instance.player.ExitBlackholeState();
    }

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0) return;

        for (int i = 0; i<createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (KeyCodeList.Count > 0 && collision.GetComponent<Enemy>() && collision.GetComponent<CharacterStat>().isAlive())
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>())
        {
            collision.GetComponent<Enemy>().FreezeTime(false);
        }
    }

    private void CreateHotKey(Collider2D collision)
    {
        if (KeyCodeList.Count <= 0 || canAttack || !canCreateHotKey)
        {
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = KeyCodeList[Random.Range(0, KeyCodeList.Count)];
        KeyCodeList.Remove(choosenKey);

        Blackhole_Hotkey_Controller HotKeyCon = newHotKey.GetComponent<Blackhole_Hotkey_Controller>();

        HotKeyCon.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEemyToList(Transform _enemyTrans) => targets.Add(_enemyTrans);
}
