using UnityEngine;

public enum EAIState
{
    //����
    Idle,
    //Ѳ�ߣ��ƶ���
    Move,
    //����
    Chat,
    //����
    Run,
    //׷��
    Chase,
    //����
    Atk,
    //����
    Alertness,
    //����
    Dead
}

public interface IAiObj
{
    public Transform objTransform { get; }
    public Vector3 curPos { get; }
    public Vector3 targetPos { get; }
    public float atkRange { get; }
    public Vector3 bornPos { get; set; }

    public void Move(Vector3 dirOrPos);
    public void StopMove();
}

public class AIStateMachine : StateMachine
{

}
