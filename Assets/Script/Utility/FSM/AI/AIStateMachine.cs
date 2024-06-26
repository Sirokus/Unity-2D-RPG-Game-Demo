using UnityEngine;

public enum EAIState
{
    //ÏĞÖÃ
    Idle,
    //Ñ²Âß£¨ÒÆ¶¯£©
    Move,
    //ÁÄÌì
    Chat,
    //ÌÓÅÜ
    Run,
    //×·»÷
    Chase,
    //¹¥»÷
    Atk,
    //¾¯¾õ
    Alertness,
    //ËÀÍö
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
