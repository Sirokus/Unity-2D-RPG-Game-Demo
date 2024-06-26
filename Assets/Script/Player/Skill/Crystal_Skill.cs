using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal Mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Explosive Crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi Stacking Crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int stackAmount;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> crystalLeft = new List<GameObject>();
    private float timer;


    [Header("Skill Tree")]
    public SkillTreeNodeUI mirageBlinkBtn;
    public SkillTreeNodeUI explosionBtn;
    public SkillTreeNodeUI movingBtn;
    public SkillTreeNodeUI multipleBtn;



    protected override void Start()
    {
        base.Start();

        mirageBlinkBtn.onLockChanged += (bool val) => cloneInsteadOfCrystal = val;
        explosionBtn.onLockChanged += (bool val) => canExplode = val;
        movingBtn.onLockChanged += (bool val) => canMoveToEnemy = val;
        multipleBtn.onLockChanged += (bool val) => canUseMultiStacks = val;
    }

    public override void UseSkill(Player player)
    {
        base.UseSkill(player);

        if (!unlock)
            return;

        if (timer > 0 || !CanUseSkill()) return;

        if (CanUseMultiCrystal()) return;

        if (currentCrystal)
        {
            if (canMoveToEnemy) return;

            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneInsteadOfCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Crystal_Skill_Controller>().crystalDestroy();
            }
        }
        else
        {
            CreateCrystal();
        }
    }

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        Crystal_Skill_Controller crystalCon = currentCrystal.GetComponent<Crystal_Skill_Controller>();

        crystalCon.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(player.transform, 25));
    }

    public void CrystalControllerChooseRandomTarget(float radius)
    {
        currentCrystal.GetComponent<Crystal_Skill_Controller>().ChooseRandomEnemy(radius);
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (crystalLeft.Count > 0)
            {
                if (crystalLeft.Count == stackAmount) Invoke("ResetAbility", useTimeWindow);

                GameObject crystalToSpawn = crystalLeft[crystalLeft.Count - 1];

                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity);

                crystalLeft.Remove(crystalToSpawn);

                newCrystal.GetComponent<Crystal_Skill_Controller>().SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosetEnemy(player.transform, 25));

                if (crystalLeft.Count <= 0)
                {
                    timer = multiStackCooldown;
                    RefilCrystal();
                }

                return true;
            }
        }

        return false;
    }

    private void RefilCrystal()
    {
        int has = crystalLeft.Count;

        for (int i = has; i < stackAmount; i++)
        {
            crystalLeft.Add(crystalPrefab);
        }
    }

    protected override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;
    }

    private void ResetAbility()
    {
        if (timer > 0) return;
        timer = multiStackCooldown;
        RefilCrystal();
    }
}
