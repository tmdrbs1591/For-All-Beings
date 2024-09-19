using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearPanelCharImage : MonoBehaviour
{
    private Image charImage;

    [SerializeField] Sprite KnightSprite; // Knight ĳ������ ��������Ʈ
    [SerializeField] Sprite ArcherSprite; // Archer ĳ������ ��������Ʈ
    [SerializeField] Sprite DragoonSprite; // drn ĳ������ ��������Ʈ
    [SerializeField] Sprite MageSprite; // Mages ĳ������ ��������Ʈ


    // Start is called before the first frame update
    void Start()
    {
        charImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // CharManager���� ���� ĳ���͸� �����ͼ� �̹����� �����մϴ�.
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

