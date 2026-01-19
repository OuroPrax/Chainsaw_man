using UnityEngine;

public class _test_Nico : MonoBehaviour
{
    [SerializeField] bool useFPS;
    [SerializeField] bool cleanMaxScore;
    [SerializeField] GameObject target;
    [SerializeField] float value;
    [SerializeField] ResultHandler resultHandler;

    private void Awake()
    {
        if(useFPS)
            Application.targetFrameRate = 60;
        if(cleanMaxScore)
            PlayerPrefs.DeleteKey("MaxScore");
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            target.GetComponent<IDamageable>().TakeDamage(value, target.transform.position);
        if (Input.GetKeyDown(KeyCode.O))
            resultHandler.ShowResult();
    }
}
