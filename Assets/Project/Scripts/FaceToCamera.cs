using UnityEngine;

namespace Project.Scripts
{
    public class FaceToCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
            transform.position += _camera.transform.TransformDirection(Vector3.forward);
        }

        private void Update()
        {
            transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
        }
    }
}