using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SniperMuzzle : WeaponMuzzle
{
    private GraphicsBuffer _gBuffer;
    private List<Vector3> _hitPos = new List<Vector3>();

    public override void SendFireEvent(RaycastHit hit)
    {
        //bulletEffect.SetVector3("Bullet_LineEnd", hit.point);
        //bulletEffect.SetVector3("Bullet_LineStart", transform.position);

        //bulletEffect.SendEvent("OnFire");
    }

    private void Start()
    {
        _gBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 8, Marshal.SizeOf<Vector3>());
        bulletEffect.SetGraphicsBuffer("EndPositions", _gBuffer);

        Debug.LogWarning("does sniper flash have graphics buffer property?: " + bulletEffect.HasGraphicsBuffer("EndPositions"));
        Debug.LogWarning("does sniper flash have end pos count property?: " + bulletEffect.HasInt("EndPositionsCount"));
        Debug.LogWarning("does sniper flash have start position property?: " + bulletEffect.HasVector3("StartPosition"));
    }

    public override void SendFireEventList(List<RaycastHit> inputHits)
    {
        if (bulletEffect)
        {
            Debug.Log("Received fire event list in sniper muzzle, hit count: " + inputHits.Count);
            _hitPos.Clear();

            _hitPos.Add(transform.position);
            Debug.Log("adding start pos: " + _hitPos[0]);

            foreach (RaycastHit hit in inputHits)
            {
                _hitPos.Add(hit.point);
                Debug.Log("adding hit pos: " + hit.point);
            }

            Debug.Log("sniper end positions count (should be same as hit count): " + _hitPos.Count);
            bulletEffect.SetInt("EndPositionsCount", _hitPos.Count);

            for(int i = 0;  i < _hitPos.Count-1; i++)
            {
                Debug.DrawLine(_hitPos[i], _hitPos[i + 1], Color.chartreuse, 10f);
            }

            //for (int i = _hitPos.Count; i < 4; i++)
            //{
            //    _hitPos.Add(Vector3.zero);
            //    Debug.Log("adding zero vector: " + _hitPos[i]);
            //}
            _gBuffer.SetData(_hitPos);
            //bulletEffect.SetVector3("StartPosition", transform.position);

            bulletEffect.SendEvent("OnFire");
        }
    }

    private void OnDestroy()
    {
        _gBuffer?.Release();
    }
}
