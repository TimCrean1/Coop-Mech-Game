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
        _gradientMaterial.SetFloat("_BarPercent", factor01);
        Debug.Log(_gradientMaterial + " has bar percent: " + _gradientMaterial.HasFloat("_BarPercent") + ", setting factor to: " + factor01);
    }

    private void Start()
    {
        // do this to reset the bar to full when the game starts
        _gradientMaterial.SetFloat("_BarPercent", 0f);
    }
}
