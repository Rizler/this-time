using UnityEngine;
using System.Collections;

namespace Prototype02
{
    public class CameraFacingBillboard : MonoBehaviour
    {
        public Camera m_Camera;

        void Update()
        {
            if (!m_Camera) m_Camera = Camera.main;

            transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
                m_Camera.transform.rotation * Vector3.up);
        }
    }
}