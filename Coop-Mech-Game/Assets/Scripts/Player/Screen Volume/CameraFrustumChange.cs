using UnityEngine;
using System.Collections;

public class CameraFrustumChange : MonoBehaviour
{
    /// <summary>
    /// 
    /// this script is adapted from the example provided in unity documentation https://docs.unity3d.com/Manual/ObliqueFrustum.html
    /// 
    /// </summary>


    [SerializeField, Range(-1.0f, 1.0f)] private float horizontalObliqueness = 0f;
    [SerializeField, Range(-1.0f, 1.0f)] private float verticalObliqueness = 0f;
    [SerializeField] private Camera cam;

    private void Start()
    {
        SetObliq(horizontalObliqueness, verticalObliqueness);
    }


#if UNITY_EDITOR
    private void Update()
    {
        SetObliq(horizontalObliqueness, verticalObliqueness);
    }
#endif

    void SetObliq(float horizObl, float vertObl)
    {
        Matrix4x4 mat = cam.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        cam.projectionMatrix = mat;
    }
}
