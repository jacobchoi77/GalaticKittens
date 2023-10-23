using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OnUIButtonHover : MonoBehaviour{
    [SerializeField] private Sprite m_normalSprite;

    [SerializeField] private Sprite m_hoverSprite;

    private Image buttonImageUI;

    private void Start(){
        buttonImageUI = GetComponent<Image>();
    }

    public void OnHover(bool isHover){
        buttonImageUI.sprite = isHover ? m_hoverSprite : m_normalSprite;
    }
}