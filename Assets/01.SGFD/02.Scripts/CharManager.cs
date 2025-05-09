using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Character
{
    Knight, Archer , Dragoon ,Mage
}

public class CharManager : MonoBehaviour
{

    public static CharManager instance;  // 싱글톤 패턴을 위한 정적 인스턴스 변수
                                         // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this; // 인스턴스가 null일 경우, 자기 자신을 instance에 할당
        else if (instance != null) return; // 이미 다른 인스턴스가 존재하면, 이 인스턴스를 파괴하고 리턴합니다.
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 객체를 파괴하지 않도록 설정합니다.
    }

    public Character currentCharacter; // 현재 선택된 캐릭터를 저장할 변수

    public void CharChangeKnight()
    {
        currentCharacter = Character.Knight;

        Debug.Log("기사 선택");
    }
    public void CharChangeArcher()
    {
        currentCharacter = Character.Archer;
        Debug.Log("궁수 선택");

    }
    public void CharChangeDragoon()
    {
        currentCharacter = Character.Dragoon;
        Debug.Log("용기사 선택");

    }
    public void CharChangeMage()
    {
        currentCharacter = Character.Mage;
        Debug.Log("마법사 선택");

    }
}
