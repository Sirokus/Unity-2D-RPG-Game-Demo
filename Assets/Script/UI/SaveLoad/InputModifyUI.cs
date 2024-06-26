using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputModifyUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI ActionName;
    [SerializeField] private TextMeshProUGUI Key;
    private GameAction action;
    private KeyCode key;
    private bool isModify;

    private void Awake()
    {
        button.onClick.AddListener(onClick);
    }

    public void setup(GameAction action, KeyCode key)
    {
        this.action = action;
        this.key = key;
        ActionName.text = Utility.ToString(action);
        Key.text = this.key.ToString();
    }

    private void OnGUI()
    {
        if (isModify && Input.anyKeyDown)
        {
            KeyCode newKey = Event.current.keyCode;
            if (Input.GetKeyDown(KeyCode.Mouse0))
                newKey = KeyCode.Mouse0;
            else if (Input.GetKeyDown(KeyCode.Mouse1))
                newKey = KeyCode.Mouse1;

            if (newKey != KeyCode.Escape && KeyMgr.set(action, newKey))
            {
                key = newKey;
            }

            isModify = false;
            Key.text = key.ToString();
            Invoke("complete", 0);
        }
    }

    void complete()
    {
        KeyMgr.ins.pauseInput = false;
    }

    public void onClick()
    {
        if (!isModify)
        {
            isModify = true;
            Key.text = "";
            KeyMgr.ins.pauseInput = true;
        }
    }
}
