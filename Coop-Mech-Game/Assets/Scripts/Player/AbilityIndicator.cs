using UnityEngine;

public class AbilityIndicator : MonoBehaviour
{

    [SerializeField] private Material dashMat;
    [SerializeField] private Material jumpMat;
    [SerializeField] private GameObject rightUtilMat;
    [SerializeField] private GameObject leftUtilMat;

    private Material utilMat1;
    private Material utilMat2;


    private void Start()
    {
        utilMat1 = rightUtilMat.GetComponent<Material>();
        utilMat2 = leftUtilMat.GetComponent<Material>();
    }

    private void SetMaterialActive(string materialToSet, bool setOn)
    {
        switch (materialToSet)
        {
            case "dash":
                float emit = setOn ? 1f : 0f;
                //dashMat.SetEmission(emit);
                break;

            case "jump":
                emit = setOn ? 1f : 0f;
                //jumpMat.SetEmission(emit);
                break;

            case "utilityLeft":
                emit = setOn ? 1f : 0f;
                //leftUtilMat.SetEmission(emit);
                break;

            case "utilityRight":
                emit = setOn ? 1f : 0f;
                //rightUtilMat.SetEmission(emit);
                break;
        }
    }
}
