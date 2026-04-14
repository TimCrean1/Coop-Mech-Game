using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class ShotgunMuzzle : WeaponMuzzle
{
    private GraphicsBuffer _gBuffer;
    private List<Vector3> _hitPos = new List<Vector3>();

    public override void SendFireEvent(RaycastHit hit)
    {
    }

    private void Start()
    {
        _gBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 8, Marshal.SizeOf<Vector3>());
        bulletEffect.SetGraphicsBuffer("EndPositions", _gBuffer);
    }
    public override void SendFireEventList(List<RaycastHit> hitList)
    {
        if (bulletEffect)
        {
            foreach (RaycastHit hit in hitList)
            {
                _hitPos.Add(hit.point);
            }
            
            bulletEffect.SetInt("EndPositionsCount", _hitPos.Count);
            for (int i = _hitPos.Count; i < 8; i++)
            {
                _hitPos.Add(Vector3.zero);
            }
            _gBuffer.SetData(_hitPos);
            bulletEffect.SetVector3("StartPosition", transform.position);

            bulletEffect.SendEvent("OnFire");
        }
    }
    private void OnDestroy()
    {
        _gBuffer?.Release();
    }
}
