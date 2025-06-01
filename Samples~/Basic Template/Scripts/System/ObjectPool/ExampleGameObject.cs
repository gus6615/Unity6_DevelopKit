using UnityEngine;
using UnityEngine.UI;

namespace DevelopKit.BasicTemplate
{
    public class ExampleGameObject : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image image;
        
        private void Start()
        {
            rectTransform.anchoredPosition = new Vector2(Random.Range(-1000, 1000), Random.Range(-500, 500));
            image.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}