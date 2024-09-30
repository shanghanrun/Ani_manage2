using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isFirstGame = true;

    void Awake(){
        //싱글톤 패넌 적용: 이미 인스턴스가 존재하면 삭제
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);//씬 전환시 파괴 안되게
        } else{
            Destroy(gameObject);
        }
    }
    
}
