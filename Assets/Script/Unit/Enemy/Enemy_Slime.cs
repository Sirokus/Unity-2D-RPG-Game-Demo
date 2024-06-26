using UnityEngine;

public enum SlimeType
{
    Big,
    Medium,
    Small
}

public class Enemy_Slime : Enemy_Skeleton
{
    [Header("Slime spesific")]
    [SerializeField] private SlimeType slimeType = SlimeType.Big;
    [SerializeField] private int slimeToCreate = 2;
    [SerializeField] private Vector2 minCreationVelocity, maxCreationVelocity;
    public GameObject slimePrefab;

    public override void Die()
    {
        base.Die();
        if (slimeType == SlimeType.Small)
            return;

        CreateSlimes(slimeToCreate, slimePrefab);
    }

    private void CreateSlimes(int _amountOfSlimes, GameObject _slimePrefab)
    {
        for (int i = 0; i < _amountOfSlimes; i++)
        {
            GameObject newSlime = Instantiate(_slimePrefab, transform.position, Quaternion.identity);
            newSlime.GetComponent<Enemy_Slime>().SetupSlime(i % 2 == 0);
        }
    }

    public void SetupSlime(bool _dir)
    {
        Flip(_dir);

        float xVelocity = Random.Range(minCreationVelocity.x, maxCreationVelocity.x) * Time.deltaTime;
        float yVelocity = Random.Range(minCreationVelocity.y, maxCreationVelocity.y) * Time.deltaTime;

        TimerManager.addTimer(.1f, false, () =>
        {
            SetVelocity(new Vector2(xVelocity * dir, yVelocity));
            isKnocked = true;
        });

        TimerManager.addTimer(1f, false, () => isKnocked = false);
    }
}
