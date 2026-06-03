using Unity.Netcode;
using UnityEngine;

public class AbilityIndicator : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Material dashMat;
    [SerializeField] private Material jumpMat;
    [SerializeField] private GameObject rightUtilMat;
    [SerializeField] private GameObject leftUtilMat;

    [Header("Color Params")]
    [SerializeField] private Color emitColor = Color.orange;
    [SerializeField] private float emitIntensity = 7f;

    private Material utilMatRight;
    private Material utilMatLeft;


    private void Start()
    {
        Renderer rightRenderer = rightUtilMat.GetComponent<Renderer>();
        Renderer leftRenderer = leftUtilMat.GetComponent<Renderer>();

        if (rightRenderer != null)
            utilMatRight = rightRenderer.material;

        if (leftRenderer != null)
            utilMatLeft = leftRenderer.material;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetMaterialActiveRpc(string materialToSet, bool setOn)
    {
        if (!IsSpawned || !NetworkManager.IsSpawned)
        {
            // Debug.LogWarning("AbilityIndicator not spawned.");
            return;
        }
        switch (materialToSet)
        {
            case "dash":
                bool emit = setOn ? true : false;
                ChangeMat(dashMat, emit);
                break;

            case "jump":
                emit = setOn ? true : false;
                //jumpMat.SetEmission(emit);
                ChangeMat(jumpMat, emit);
                break;

            case "utilityLeft":
                emit = setOn ? true : false;
                //leftUtilMat.SetEmission(emit);
                ChangeMat(utilMatLeft, emit);
                break;

            case "utilityRight":
                emit = setOn ? true : false;
                //rightUtilMat.SetEmission(emit);
                ChangeMat(utilMatRight, emit);
                break;
        }
    }
    private void ChangeMat(Material _mat, bool turnOff)
    {
        //Debug.Log("Changing mat: " + _mat + " Turning Off: " + turnOff);

        if (turnOff)
        {
            _mat.SetColor("_EmissionColor", emitColor * 0f);
        }
        else
        {
            _mat.SetColor("_EmissionColor", emitColor * emitIntensity);
        }
    }
}