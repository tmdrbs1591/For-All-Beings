using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanelCharImage : MonoBehaviour
{
    private Image charImage;

    [SerializeField] Sprite KnightSprite; // Knight 캐릭터의 스프라이트
    [SerializeField] Sprite ArcherSprite; // Archer 캐릭터의 스프라이트
    [SerializeField] Sprite DragoonSprite; // drn 캐릭터의 스프라이트
    [SerializeField] Sprite MageSprite; // Mages 캐릭터의 스프라이트


    // Start is called before the first frame update
    void Start()
    {
        charImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // CharManager에서 현재 캐릭터를 가져와서 이미지를 설정합니다.
        switch (CharManager.instance.currentCharacter)
        {
            case Character.Knight:
                charImage.sprite = KnightSprite;
                break;

            case Character.Archer:
                charImage.sprite = ArcherSprite;
                break;

            case Character.Dragoon:
                charImage.sprite = DragoonSprite;
                break;

            case Character.Mage:
                charImage.sprite = MageSprite;
                break;
        }
    }
}

