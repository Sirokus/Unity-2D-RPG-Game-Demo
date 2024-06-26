using System.Collections;
using UnityEngine;

public class Clone_Skill : Skill
{
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float CloneDuration = 1;
    [Space]
    [SerializeField] private bool CanAttack = true;
    [SerializeField] private float TraceEnemuRadius = 3f;

    [Header("Clone")]
    [SerializeField] public bool canCreateCloneOnDashStart;
    [SerializeField] public bool canCreateCloneOnDashEnd;
    [SerializeField] public bool canCreateCloneOnCounterAttack;
    [Header("Clone")]
    [Header("Duplicate")]
    [SerializeField] public bool canDuplicateClone;
    [SerializeField] private float DuplicateChance = 0.6f;

    [Header("Crystal instead of clone")]
    public bool crystalInseadOfClone;

    public void CreateClone(Transform trans)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject clone = Instantiate(clonePrefab);

        Clone_Skill_Controller controller = clone.GetComponent<Clone_Skill_Controller>();
        if (!controller)
            controller = clone.GetComponentInChildren<Clone_Skill_Controller>();

        controller.SetupClone(trans, CloneDuration, CanAttack, FindClosetEnemy(trans.transform, TraceEnemuRadius), canDuplicateClone, DuplicateChance);
    }

    public void CreateClone(Transform trans, Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject clone = Instantiate(clonePrefab);

        Clone_Skill_Controller controller = clone.GetComponent<Clone_Skill_Controller>();
        if (!controller)
            controller = clone.GetComponentInChildren<Clone_Skill_Controller>();

        controller.SetupClone(trans, CloneDuration, CanAttack, _offset, FindClosetEnemy(trans.transform, TraceEnemuRadius), canDuplicateClone, DuplicateChance);
    }

    public void CreateCloneOnDashStart()
    {
        if (canCreateCloneOnDashStart)
        {
            CreateClone(player.transform);
        }
    }

    public void CreateCloneOnDashEnd()
    {
        if (canCreateCloneOnDashEnd)
        {
            CreateClone(player.transform);
        }
    }

    public void CreateCloneOnCounterAttack()
    {
        if (canCreateCloneOnCounterAttack)
        {
            StartCoroutine(CreateCloneWithDelay(player.transform, new Vector3(2 * player.dir, 0, 0), 0.2f));
        }
    }
    private IEnumerator CreateCloneWithDelay(Transform _transform, Vector3 _offset, float _second)
    {
        yield return new WaitForSeconds(_second);

        CreateClone(_transform, _offset);
    }
}
