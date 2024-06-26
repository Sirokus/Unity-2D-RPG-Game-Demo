using UnityEngine;

public interface IAttachable
{
    void AttachTo(Transform parent);
    void DettachTo(Transform parent);
}
