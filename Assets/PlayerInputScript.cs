using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputScript : MonoBehaviour
{
    [SerializeField]
    internal PlayerScript PS;  

    public float moveX = 0;
    public float moveY = 0;

    public bool leftMove;  // 좌측 이동
    public bool rightMove; // 우측 이동
    public bool dashMove;  // 좌우 화살표 연속 클릭
    public bool climbMove;    // climb_back
    public bool climbSideMove; // climb_side
    public bool ledgeClimbMove;
    public bool boxMove;
    public bool jumpMove;  // space
    public bool attackMove;// D
    public bool crouchMove;// C crouch 토글
    public bool pullMove;  // shift 화살표 방향으로 pull or push
    public bool pushMove;
    // public bool keyEvent; //  열쇠 얻었을 때 이벤트

    public bool isSliding = false;
    float slidingThreshold = 0.3f;

    

    int dashCount = 1;            // 대쉬 가능 횟수  dash는 'D'누르는 걸로 
    int jumpCount = 2;   //  연속 점프 가능횟수

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (PS.isGrounded)
        {
            jumpCount = 2;  // 점프가능하게 
            // print("grounded 되어서 점프 가능");
        }
    }

    void HandleMovement()
    {
        InitMove(); // 모든 이동 플래그를 false로 초기화
        if(PS.meetPrincess){
            return; // 공주를 만난후에는 위치고정
        }

        if(Input.GetKey(KeyCode.Escape)){
            PS.canPullPush = false;
            // PS.DetachFromBox();
            boxMove = false;
            PS.transform.SetParent(null);
            
        }
        // if(PS.canOpenDoor){ // key를 얻고 나서 다시 당길 수 있게
        //     PS.canPullPush = true;
        // }

        if(PS.isBoxContact && PS.canPullPush){ // canPullPush가 가능할 때, 접촉했을 경우
            boxMove = true;
        }

        if(PS.isLadder){
            climbMove = true;
        }
        if(PS.isStickyWalled){
            climbSideMove = true;
        }

        // 속도를 기준으로 슬라이딩 상태를 판단
        if (Mathf.Abs(PS.rb.velocity.x) > slidingThreshold && !PS.isGrounded)
        {
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        // 슬라이딩 상태에서는 대쉬나 점프가 가능하게 처리
        if (isSliding)
        {
            // 슬라이딩 중 대쉬나 점프 입력 처리
            if (PS.inputS.jumpMove && !PS.moveS.isJumping)
            {
                jumpMove =true;
            }
            else if (PS.inputS.dashMove && !PS.moveS.isJumping)
            {
                dashMove = true;
            }
        }



        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        // 우측 또는 좌측 화살표 클릭
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            int direction = Input.GetKeyDown(KeyCode.RightArrow) ? 1 : -1;
            //좌우 이동 설정
            if (direction > 0){
                rightMove = true;
            } else {
                leftMove = true;
            }
        }

        // 스페이스바로 점프 처리
        if (Input.GetKey(KeyCode.Space))
        {
            if(jumpCount >0){
                jumpMove = true;
                PS.moveS.isJumping = true;
                jumpCount --;
                dashCount =1; // 점프할 때마다 대쉬 카운트 회복
            }
        }

        // 다른 키 입력 처리 시에도 대쉬 카운트를 초기화
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
        {
            
        }
        // 대쉬 처리
        if(Input.GetKey(KeyCode.D)){
            if(!PS.isGrounded && dashCount > 0){
                dashMove = true;
                dashCount --;
            }
        }

    }

    void InitMove()
    {
        leftMove = false;
        rightMove = false;
        dashMove = false;
        climbMove = false;
        climbSideMove = false;
        ledgeClimbMove = false;
        boxMove = false;
        jumpMove = false;
        attackMove = false;
        crouchMove = false;
        pullMove = false;
        pushMove = false;
    }
}
