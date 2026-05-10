using UnityEngine;

public class MechBarScreen : MonoBehaviour
{
    [SerializeField] private Material _gradientMaterial;

    /// <summary>
    /// input factor should take into account the fact that a factor of 0 will result in a "full" gradient
    /// </summary>
    /// <param name="factor01"></param>
    public void SetGradientFactor(float factor01)
    {
        _gradientMaterial.SetFloat("BarPercent", factor01);
    }
}
