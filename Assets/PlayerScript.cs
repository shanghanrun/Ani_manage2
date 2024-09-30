using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    internal PlayerInputScript inputS;
    [SerializeField]
    internal PlayerMovementScript moveS;

    // enum anime {
    //     idle, hurt, dead, 
    //     walk, dash, run, jump, land,
    //     roll, airSpin, slide, wallSlide_left, wallSlide_right,
    //     wallLand_left, wallLand_right,
    //     climbBack, climbSide, ledgeClimb, crouch, crouchWalk,
    //     pushPullIdle, push, pull,
    //     swordIdle, swordAttack, swordStab, swordRun,
    //     gunIdle, shoot
    // }

    //자기 자신 컴포넌트 참조
    public Animator anim;
    public Animator princessAnim;
    public Rigidbody2D rb;
    Renderer playerRenderer;
    Color originalColor;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;

    public bool isGrounded = true;
    public bool isWalled = false;
    public bool isLadder = false;
    public bool isStickyWalled = false;
    public bool isLedgeClimbed = false;
    public bool isBoxContact = false;
    public bool canPullPush = true;
    public bool leverActivated = false;
    public bool openBox = false;
    public bool gotKey = false;
    public bool gameOver = false;
    public bool canOpenDoor = false;
    public bool touchDoor = false;
    public bool canMeetPrincess = false;
    public bool getDoor2 = false;
    public bool meetPrincess = false;
    

    GameObject box;
    GameObject lever;
    [SerializeField] GameObject hiddenKey;
    // [SerializeField] public GameObject spark1; //PlayerMovementScript가 적당
    [SerializeField] GameObject spark2;
    [SerializeField] GameObject door2;
    public Sprite newBoxSprite; // 박스가 바뀔 이미지
    public Sprite lever2Sprite; //  바뀔 손잡이
    public Sprite princess2; // 최종 이미지

   

    //다른 요소 참조
    string currentState = "idle";
    string currentPrincessState = "princess";
    Vector3 meetPos;

    void Awake()
    {
        print("Main PlayerScript Awake");
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;

        box = GameObject.FindGameObjectWithTag("Box");
        lever =GameObject.Find("lever1");
    }

    void Update(){
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);
        isWalled = Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);

        // box를 떼는 것은 여기에 두어야 
        if (!canPullPush && isBoxContact)
        {
            // Release the box by setting its parent to null
            foreach (Transform child in transform)
            {
                if (child.tag == "Box")
                {
                    child.SetParent(null);  // Detach the box
                    isBoxContact = false;   // Player is no longer in contact with the box
                    print("Box released after Escape key pressed.");
                    break;
                }
            }
        }
        if (canOpenDoor)
        { // key를 얻고 나서 다시 당길 수 있게
            canPullPush = true;
        }

        if (meetPrincess) transform.position = meetPos;
    }
    // State management
    public void ChangeState(string newState)
    {
        if (newState != currentState)
        {
            anim.Play(newState);
            currentState = newState;
        }
    }
    public void ChangePrincessState(string newState){
        if (newState != currentPrincessState){
            princessAnim.Play(newState);
            currentPrincessState = newState;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag =="Ladder"){
            isLadder = true;
        }
        // if(other.gameObject.tag =="Princess"){
        //     SpriteRenderer princessSR = lever.GetComponent<SpriteRenderer>();
        //     if (princessSR != null)
        //     {
        //         princessSR.sprite = princess2;
        //     }
        // }
        if (other.gameObject.tag == "Princess") // Princess를  isTrigger로 해야 플레이어와 겹칠 수 있다.
        {
            print("공주만남");
            meetPrincess = true;  // PlayerInput에서 Update에서 meetPrincess하면 return되게 하면 움직이지 않게된다. 
            meetPos = transform.position;
        }

    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag =="Ladder"){
            isLadder = false;
        }
    }
    void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag =="Sticky_Wall"){
            isStickyWalled = true;
        }
        if(other.gameObject.tag =="Lever"){
            SpriteRenderer leverSR = lever.GetComponent<SpriteRenderer>(); 
            if(leverSR !=null){
                leverSR.sprite = lever2Sprite;
                leverActivated = true;
            }
        }
        if(other.gameObject.tag =="Box"){
            // if(hiddenKey !=null){ // 키는 한번 받으면 없앤다. 그러므로 없어지지 않았을 때
                if(box !=null){
                    SpriteRenderer boxSR = box.GetComponent<SpriteRenderer>();
                    if (boxSR !=null){
                        boxSR.sprite = newBoxSprite;
                        openBox = true;
                    }
                    if(!gotKey && !canOpenDoor){ // 이미 키를 가졌을 때와 키를 가져서 문을 열수 있는 상황에서는 나타나지 않게 (반댇일 경우만 나타나게)
                        hiddenKey.SetActive(true);
                    }
                }
                
                if(canPullPush){
                    isBoxContact = true;
                    other.gameObject.transform.SetParent(transform, true);//true를 하면 자식위치 유지
                }
                else { // 상자에 접촉하면 다시 밀 수 있게 한다.(이 코드의미 더 살펴야 될듯)
                    canPullPush = true; 
                }
            // }
        }
        if(other.gameObject.tag == "Key"){
            gotKey = true; 
            canOpenDoor = true;
            if(hiddenKey !=null){
                hiddenKey.SetActive(false); // 플레이어가 취득했으므로 모습 사라짐
                // Destroy(hiddenKey); // 다시 box에 부딛치면 나타나면 안된다. 
            }
            spark2.SetActive(true);
        }
        if(other.gameObject.tag =="Door"){
            if(canOpenDoor){
                touchDoor = true;
                canMeetPrincess = true;
                //player 이동시키기
                // rb.isKinematic = true; // 중력작용없애서 그 자리에 있게 만듬
                rb.gravityScale = 0;
                StartCoroutine(TransportToDoor2());
                // transform.position = new Vector2(door2.transform.position.x, door2.transform.position.y);
                // transform.localScale = transform.localScale * 1.8f;
                // getDoor2 =true;
                
            }
        }
        
        if(other.gameObject.tag =="Dead"){
            gameOver = true;
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Sticky_Wall")
        {
            isStickyWalled = false;
            isLedgeClimbed = true;
        }
        // if(other.gameObject.tag =="Box"){
        //     if(!canPullPush){
        //         isBoxContact = false;
        //         other.gameObject.transform.SetParent(null);
        //     }
        // }
    }

    private IEnumerator TransportToDoor2()
    {
        // 점점 투명해지기
        float duration = 3f; // 투명해지는 시간
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1, 0, normalizedTime); // 알파 값 변경
            playerRenderer.material.color = newColor;
            yield return null; // 다음 프레임까지 대기
        }

        // Door2 위치로 이동
        transform.position = new Vector2(door2.transform.position.x, door2.transform.position.y);
        transform.localScale = transform.localScale * 1.3f; // 크기 커지게

        // 점점 나타나기
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(0, 1, normalizedTime); // 알파 값 변경
            playerRenderer.material.color = newColor;
            yield return null; // 다음 프레임까지 대기
        }
        // rb.isKinematic = false;

        touchDoor = false; // 초기화
    }


}