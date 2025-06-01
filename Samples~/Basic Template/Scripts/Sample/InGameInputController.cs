using UnityEngine;
using Random = UnityEngine.Random;

namespace DevelopKit.BasicTemplate
{
    public class InGameInputController : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Mathf.Abs(_camera.transform.position.z);
                CreateJemView(_camera.ScreenToWorldPoint(mousePos));
            }
        }
        
        private void CreateJemView(Vector3 createPos)
        {
            var jemView = AddressableUtil.Instantiate<JemView>("JemView");
            int randomJemID = Random.Range(0, 100);
            jemView.Initialize(randomJemID, transform, createPos);
        }
    }
}
