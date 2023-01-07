using UnityEngine;
using UnityEngine.UI;

namespace Core.Enemies
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _root;
        
        private float _maxHp;
        private Camera _mainCamera;

        public void Initialize(float maxHP)
        {
            _maxHp = maxHP;
            UpdateBar(maxHP);
            Show();
            _root.transform.position =
                new Vector3(_root.transform.position.x, _root.transform.position.y, _root.transform.position.z);
        }

        public void UpdateBar(float newHPAmount)
        {
            float newRatio = newHPAmount / _maxHp;
            _image.fillAmount = newRatio;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            Quaternion rotation = _mainCamera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.up);
        }
    }
}