using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static event Action OnJumpEnded;
    public static event Action<Transform> OnStarTransform;

    [HideInInspector]
    public bool isRunning;

    [Header("Timer")]
    [SerializeField] private float cooldownTime = 2f;
    [SerializeField] private float timer = 2f;

    [Space]
    [Header("Move Settings")]
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float rotateDuration = 2f;
    [SerializeField] private float jumpPower = 100f;
    [SerializeField] private float  delayBeforeJump = 0.2f;

    [Space]
    [Header("CheckPoints")]
    [SerializeField] private List<Transform> checkPointsList = new List<Transform>();
    [SerializeField] private int checkPointIndex = 0;

    

    [Space]
    [Header("Ragdoll")]
    private Collider mainColider;
    private Collider[] allColiders;
    private Rigidbody[] rigidbodys;

    [Space]
    [Header("Stars")]
    [SerializeField] private List<GameObject> starsList = new List<GameObject>();
    [SerializeField] private List<Color> colorList = new List<Color>();
    [SerializeField] private int currentIndex = 0;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private float offsetY = 0.2f;
    [SerializeField] private float cooldownStar = 0.2f;
    [SerializeField] private float timerStar;

    [Space]
    [SerializeField] private Transform downPos;
    [SerializeField] private float durationDown = 2f;

    private bool isGameStarted = false;
    private Vector3 startPosition;
    private Vector3 startScale;

    private AnimationHandler animationHandler ;

    private void Awake()
    {
        startPosition = transform.position;
        startScale = transform.localScale;
        animationHandler = GetComponent<AnimationHandler>();

        //Ragdoll
        mainColider = GetComponent<Collider>();
        rigidbodys = GetComponentsInChildren<Rigidbody>(true);
        timerStar = cooldownStar;
    }

    private void Start()
    {
        OnStarTransform?.Invoke(starsList[currentIndex].transform);
    }


    private void Upddate()
    {
            ActivateStar();
    }

    private void Update()
    {
        if(GameManager.instance.isGameOver == false )
        {
            if (isGameStarted)
            {
                //Do the Scall
                //ActivateStar();
                TimeBetweenSpawn();

                //Do the Movemeent
                TimeBetweenMovement();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
            DeathMovement();

    }


    #region Pen Movement

    //Time Between Movement
    private void TimeBetweenMovement()
    {
        if(timer >= cooldownTime)
        {
            StartCoroutine(JumpCoroutine());
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    //Movement Coroutine Cast aniamtion before jump

    IEnumerator JumpCoroutine()
    {
        animationHandler.JumpAnimation(true);
        yield return new WaitForSeconds(delayBeforeJump);
        PlayerMovement();
    }

    // Move The Pen Toward Paint Pails
    private void PlayerMovement()
    {
        isRunning = true;
        AudioManger.instance.Play("JumpGirl");
        transform.DOJump(checkPointsList[checkPointIndex].position, jumpPower, 1, moveDuration,
            false).OnComplete(() =>
            {
                isRunning = false;
                animationHandler.JumpAnimation(false);
                OnJumpEnded?.Invoke();
                IncreaseIndex();
            }
        ); 
    }

    private void  Running() => isRunning = false;

    // Update the next index to Move Toward Paint Pails
    private void IncreaseIndex()
    {
        if (checkPointIndex < checkPointsList.Count - 1)
            checkPointIndex++;
    }


    //
    public void DeathMovement()
    {
        transform.DOMoveY(downPos.position.y, durationDown);
        //transform.position = Vector3.MoveTowards(transform.position, downPos.position, 2f);
    }
    #endregion

    

    // Linked to button to Start the game
    public void StartGame() => isGameStarted = true;

    // Reset Player Settings
    public void ResetPlayerSettings()
    {
        transform.position = startPosition;
        transform.localScale = startScale;
        isGameStarted = false;
        checkPointIndex = 0;
        timer = cooldownTime;
    }

    #region RagDoll

    public void DoRagdoll(bool isRagdoll)
    {
       /* foreach (var rb in rigidbodys)
        {
            rb.useGravity = false;
        }*/

       // mainColider.enabled = !isRagdoll;
       
        GetComponent<Animator>().enabled = !isRagdoll;
    }

    #endregion


    #region StarsMovements


    private void TimeBetweenSpawn()
    {
        if(timerStar >= cooldownStar)
        {
            timerStar = 0;
            ActivateStar();
        }
        else
        {
            timerStar += Time.deltaTime;
        }
    }


    private void ActivateStar()
    {
       
        if (Input.GetKey(KeyCode.UpArrow))
        {
                Debug.Log("++");
                CreateStars();
                RandomRotation(starsList[currentIndex].transform);
                int randomIndex = UnityEngine.Random.Range(0, colorList.Count - 1);

                starsList[currentIndex].GetComponent<MeshRenderer>().material.color = colorList[randomIndex];
                OnStarTransform?.Invoke(starsList[starsList.Count - 1].transform);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (currentIndex > 0)
            {
                Debug.Log("--");
                starsList[currentIndex].SetActive(false);
                starsList.Remove(starsList[currentIndex]);
                currentIndex--;
                OnStarTransform?.Invoke(starsList[starsList.Count-1].transform);
            }
        }
    }


    private void CreateStars()
    {
        var lastStartPos = starsList[currentIndex].transform.position;
        Vector3 spawnPos = new Vector3(lastStartPos.x, lastStartPos.y + offsetY , lastStartPos.z); 
        
        GameObject star = Instantiate(starPrefab, spawnPos , Quaternion.Euler(90f,0,0) , this.transform);
        //star.transform.position = spawnPos;
        starsList.Add(star);
        currentIndex++;
    }

    private void   RandomRotation(Transform trans)
    {
        var randomZ = UnityEngine.Random.Range(40, 150);
        trans.Rotate(Vector3.forward * randomZ);
    }
    #endregion

}
