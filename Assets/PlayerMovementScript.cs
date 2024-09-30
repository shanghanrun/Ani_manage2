using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public GameObject fireworks;

    PlayerScript ps;

    Rigidbody2D rb;
    float moveSpeed = 4;
    float dashSpeed = 7f;
    float jumpPower = 6f;
    float sideClimbSpeed = 2f;
    SpriteRenderer spriteRenderer;

    // 아래는 움직임의 상태
    bool isRight = true;  // 기본적인 케릭터의 방향
    // public bool isGrounded = true;
    // public bool isWalled = false;
    public bool isJumping = false;

    
    // bool isMovingSpark = false;
    Camera mainCamera;

    void Start()
    {
        ps = GetComponentInParent<PlayerScript>();
        rb = ps.GetComponent<Rigidbody2D>();
        spriteRenderer = ps.GetComponent<SpriteRenderer>();

        mainCamera = Camera.main; // 메인카메라 초기화
        
        rb.velocity = new Vector2(0, 0);
    }

    void Update(){
        

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();

    }

    void HandleMovement(){

        // if(ps.leverActivated){  카메라 이동관련코드는 CameraController로 이관
        //     MoveSpark();
        // }
        

        if(ps.inputS.boxMove){
            MoveBox(); return;
        }
        
        if(ps.inputS.climbMove){
            Climb();
            return; // 사다리를 타는 동안, 다른 상태 전환을 막기 위해
        }
        if(ps.inputS.climbSideMove){
            ClimbSide(); return;
        }
        if(ps.inputS.ledgeClimbMove){
            LedgeClimb(); return;
        }
       
        if (ps.inputS.dashMove){ // 현재 우측방향 대쉬만 가능하다.(에미메이션)
            if(isRight){
                Move(dashSpeed, "dash");
            }

        } else if(ps.inputS.rightMove || ps.inputS.leftMove){
            Move(moveSpeed * Mathf.Sign(ps.inputS.moveX), "run");


        } else if(ps.inputS.jumpMove){  // 3단점프가능하게 하기 위해 && !isJumping없앰
            Jump();
        } 
        //! 공중에 있을 때 "jump"상태유지
        else if(!ps.isGrounded && isJumping){
            ps.ChangeState("jump");
        }
        
        else if(ps.isGrounded || (ps.isGrounded && !ps.canPullPush)){
            ps.ChangeState("idle");
        }
        else if(ps.leverActivated){

        }
        if(ps.meetPrincess){
            ps.ChangePrincessState("kiss2");
            // 폭죽효과 실행
            fireworks.SetActive(true);
        }

        FlipSprite();
    }
    
    void FlipSprite(){
        if (ps.inputS.moveX > 0)  // 오른쪽 키를 눌렀는데
        {
            if(!isRight){     // 케릭터 방향이 왼쪽을 향하고 있다면
                spriteRenderer.flipX = false; // 뒤집지 않은 것이 우측이다.
            } 
            isRight = true; // 케릭터가 오른쪽 방향을 향한다.
        }
        else if (ps.inputS.moveX < 0) // 왼쪽 키를 눌렀는데
        {
            if(isRight){
                spriteRenderer.flipX = true; // 뒤집어서 왼쪽으로 만듬
            } 
            isRight = false;
        }
        
    }

    void Climb(){
        ps.ChangeState("climb_back");
        float ver = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, ver * moveSpeed);
    }

    void MoveBox(){
        ps.ChangeState("pull_push_idle");

        float hor = Input.GetAxis("Horizontal");

        // if(hor == 0){  // 항상 입력이 안될 때도 업데이트 해줘야 된다.
        //     rb.velocity = new Vector2(0, rb.velocity.y);
        //     return;  // return 을 해서 아래 내용을 읽지 않토록
        // } 


        if((isRight && hor<0) || (!isRight && hor >0)){
                ps.ChangeState("pull");        
        } else {
                ps.ChangeState("push");
        }
        
        rb.velocity = new Vector2(hor * moveSpeed * 0.3f, rb.velocity.y);
    }
    void ClimbSide(){
        ps.ChangeState("climb_side");
        float ver = Input.GetAxis("Vertical");
        rb.velocity = new Vector2(rb.velocity.x, ver * sideClimbSpeed);
    }
    void LedgeClimb(){
        ps.ChangeState("ledge_climb");
    }

    void Move(float speed, string state){
        List<string> dashLandStates = new List<string>{"dash", "land"};

        if(rb.velocity.x != speed){
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
        if( dashLandStates.Contains(state)){
            ps.ChangeState(state);
            // Invoke는 메소드를 문자열로 넣어야 된다.
            Invoke("ChangeToIdleState", 0.5f);
        } else{
            ps.ChangeState(state);
        }
    }

    void ChangeToIdleState(){
        ps.ChangeState("idle");
    }
    
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        ps.ChangeState("jump");
        
        isJumping = true;
    }
    // void StopMove(){
    //     ps.ChangeState("idle");
    // }
    // IEnumerator DoFireworks()
    // {
    //     //파티클을 생성하고 잠시 후 파괴
    //     Instantiate(fireworksPrefab, fireworksPrefab.transform.position, Quaternion.identity);
    //     fireworksPrefab.GetComponent<ParticleSystem>().Play();
    //     // 2초후 파티클 파괴
    //     yield return new WaitForSeconds(2f);
    //     fireworksPrefab.SetActive(false);

    // }

}
