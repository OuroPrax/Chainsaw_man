using UnityEngine;

public class _test_MiniBoss: MonoBehaviour
{
    [SerializeField] MiniBossController controller;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            controller.ResetMe();
    }
}
