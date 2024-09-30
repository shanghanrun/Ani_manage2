using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button 관련 기능을 사용하기 위해 추가
using UnityEngine.SceneManagement; //로드 씬

public class CameraController : MonoBehaviour
{
    public GameObject uiPanel;
    public GameObject uiPanel2; // gameover  패널
    public GameObject princess; // 
    public Button startButton;
    public Button restartButton; //  게임오버시 나오는 버튼
    public Transform playerTr;
    Vector3 playerPos;
    public PlayerScript  PS;
    Animator animator;
    bool isIntroComplete = false;
    bool isGameStarted = false;
   
    Vector3 startPos; // Princess 이 있는 위치
    Vector3 defaultPos = new Vector3(0, 0, -10); // 0,0, -10 기본위치
    float waitTime = 9.3f; //처음 Princess보여주는 시간
    // float speed = 0.1f; // 카메라 내려오는 속도  journeyTime으롷 대신한다.

    // public float smoothSpeed = 0.125f; //부드러운 카메라이동속도
    // public Vector3 offset; // 플레이어와 카메라 사이의 오프셋
    // Vector3 velocity = Vector3.zero;

    [SerializeField] GameObject spark1;
    [SerializeField] Transform spot; // 이동할 지점
    [SerializeField] float transferSpeed = 4f;
    [SerializeField] float returnSpeed = 3f;
    [SerializeField] GameObject ground5;
    [SerializeField] GameObject ground8;

    public SoundManager SM;

    bool isReturningToPlayer = false;
    

    void Start()
    {

        playerPos = playerTr.position;
        if(SM == null){
            SM = FindObjectOfType<SoundManager>();
            if(SM == null){
                Debug.Log("SoundManager가 존재하지 않습니다.");
            }
        }

        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        
        //음악 재생
        SM.PlaySoundEffect(SoundManager.SoundEffect.TitleMusic);
        StartCoroutine(SM.FadeOutMusicAfterDelay());
        

        // 카메라 시작위치 초기화
        if (princess != null)
        {
            Vector3 temp = princess.transform.position;
            startPos = new Vector3(temp.x, temp.y, -10);
            print("StartPos: " + startPos);
        }
        transform.position = startPos;
        animator = transform.GetComponent<Animator>();

        // Apply Root Motion을 꺼서 애니메이션이 이동을 방해하지 않도록 설정
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }


    

    // Start 버튼 클릭 시 호출되는 함수
    void StartGame()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI 패널 비활성화
        }

        isGameStarted = true; // 게임이 시작되었음을 표시
        StartCoroutine(MoveToPosition()); // 카메라 움직임 시작
    }

    // 코루틴으로 1초 대기 후 천천히 내려오는 함수
    IEnumerator MoveToPosition()
    {
        // 1초 대기
        yield return new WaitForSeconds(waitTime);

        // 천천히 startPos에서 defaultPos로 이동
        float elapsedTime = 0f;
        float journeyTime = 6f; // 천천히 내려오는 시간 (2초)

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(startPos, defaultPos, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 도착하면 위치를 정확히 defaultPos로 설정
        transform.position = defaultPos;

        isIntroComplete = true;
    }



    // Update is called once per frame
    void Update()
    {
        // 매 프레임마다 playerPos 업데이트: 그래야 따라갈 수 있다.
        // playerPos = playerTr.position;  이렇게 업데이트 하지 말고, 차라리 직접 playerTr.position사용하는 것 좋다.
        if (PS.canMeetPrincess)
        {
            transform.position = new Vector3(princess.transform.position.x, princess.transform.position.y, -10);
            return; // 아래 내용으로 업데이트 안되게
        }
        if (PS.gameOver){
            GameOver();
        }
        if ( PS.getDoor2){
            transform.position = playerTr.position;
        }

        if (!isIntroComplete || !isGameStarted) return;


        if( PS.leverActivated){ 
            MoveSpark();
        }
        else if(!isReturningToPlayer){ // 다시 플레이어에게 돌아가는 중이 아닌 경우
            NormalCameraWalk();
        }   
    }

    
    void MoveSpark(){  // 카메라 관련코드 CameralController로 이관
        if(spark1 !=null){
            spark1.SetActive(true);
        }

        // 카메라가 spark1을 따라가도록 위치를 업데이트
        Vector3 targetPosition = spark1.transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        // 2초간 머무른 후에 이동하게 하기 위해 코린틴 사용
        StartCoroutine(MoveSparkCoroutine());

    }
    IEnumerator MoveSparkCoroutine(){  //! return값이 있으므로 void로 정의하면 안된다.
        // spark1을 특정 지점으로 이동
        yield return new WaitForSeconds(2f);

        float step = transferSpeed * Time.deltaTime;
        spark1.transform.position = Vector3.MoveTowards(spark1.transform.position, spot.position, step);

        // spark1이 특정 지점에 도착했는지 체크
        if (Vector3.Distance(spark1.transform.position, spot.position) < 0.1f)
        {
            //감추어진 그라운드 나타나기
            ground5.SetActive(true);
            ground8.SetActive(true);
            
            // 여기서 폭죽효과 및 잠시 머물러 있기(물이 땅으로 변한 것을 이해하게 하기 위해)

            //  PS.leverActivated = false; // 이동 완료 플래그 해제
            StartCoroutine(ReturnToPlayer()); // 플레이어에게 돌아가는 코루틴 시작

        }
    }

    private IEnumerator ReturnToPlayer()
    {
        // spark1 비활성화
        if (spark1 != null)
        {
            spark1.SetActive(false);
        }
        // 3초간 기다렸다가 간다.
        yield return new WaitForSeconds(3f);
        PS.leverActivated = false; // 그래야 사운드가 멈춘다.

        // 플레이어 위치로 카메라 이동
        float elapsedTime = 0f;
        float journeyTime = 1f; // 카메라가 플레이어에게 돌아가는 시간

        Vector3 playerPosition = new Vector3(playerTr.position.x, playerTr.position.y, -10); // 실시간 플레이어 위치

        while (elapsedTime < journeyTime)
        {
            transform.position = Vector3.Lerp(transform.position, playerPosition, elapsedTime / journeyTime);
            elapsedTime += Time.deltaTime * returnSpeed;
            yield return null; // 다음 프레임까지 대기
        }

        // 최종적으로 플레이어 위치로 정확하게 설정
        transform.position = playerPosition;

        // 이동 완료 플래그 해제
        //  PS.leverActivated = false;
        GameObject lever = GameObject.Find("lever1");
        Collider2D leverCollider = lever.GetComponent<Collider2D>();
        if(leverCollider !=null){
            leverCollider.enabled = false;
        }
    }

    public void StartMovingSpark() // 외부에서 호출할 수 있는 메서드
    {
         PS.leverActivated = true; // 스파크 이동 시작
    }

    void GameOver(){
        uiPanel2.SetActive(true);
    }
    void RestartGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void NormalCameraWalk()
    {
        Vector3 cameraPos = transform.position;
        if(PS.meetPrincess){
            cameraPos = princess.transform.position;
        }

        // X 좌표에 따른 카메라 위치 설정
        if (playerTr.position.x > 15)
        {
            cameraPos.x = 25;
        }
        else if (playerTr.position.x > 8)
        {
            cameraPos.x = 8;
        }
        else
        {
            cameraPos.x = 0;
        }

        // Y 좌표에 따른 카메라 위치 설정
        if (playerTr.position.y > 4.5)
        {
            cameraPos.y = 4f;
        }
        else
        {
            cameraPos.y = 0;
        }

        // 카메라 위치를 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, new Vector3(cameraPos.x, cameraPos.y, -10), 0.1f);
    }


}
