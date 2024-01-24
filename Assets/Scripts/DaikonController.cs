using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DaikonController : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    AudioSource audioSource;
    Animator animator;
    float horizontal; 
    float vertical;
    public float speed = 4.0f;
    public TextMeshProUGUI WaterCountText;
    public TextMeshProUGUI countdownText;
    public int WaterCount = 0;
    public int totalWaterCount = 4;
    public GameObject WaterVFXPrefab;
    float timer = 10.0f;  // Timer set to 10 seconds
    bool isGameOver = true;
    bool hasTriggeredCheckpoint = false;
    bool allowTimerUpdate = false; 
    bool isAnnouncementFinished = false;
    bool isCountdownPlaying = false;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI loseText;
    public TextMeshProUGUI ruleText;
    public AudioClip announceSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public GameObject ThankYou;
    Vector2 lookDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        ThankYou.SetActive(false);
        UpdateCountdownText();
        StartCoroutine(ShowGameRules()); 
           
    }
        

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver && allowTimerUpdate)
        {
            // Update the timer
            timer -= Time.deltaTime;

            // Check if the player ran out of time
            if (timer <= 0)
            {
                StartCoroutine(LoseGame());
            }
             UpdateCountdownText();
        }
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Move X", lookDirection.x);
        animator.SetFloat("Move Y", lookDirection.y);
    }
      IEnumerator ShowGameRules()
    {    
        PlaySound(announceSound);
        ruleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        ruleText.gameObject.SetActive(false);
        allowTimerUpdate = true;
        isGameOver = false;
        StartCoroutine(StartCountdown());
        isAnnouncementFinished = true;
        
    }
    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(2.0f); 
        
        // Start the countdown timer
        while (!isGameOver)
        {
            // Update the timer
            timer -= Time.deltaTime;

            // Check if the player ran out of time
            if (timer <= 0)
            {
                StartCoroutine(LoseGame());
            }

            UpdateCountdownText();


            yield return null;
        }
    }
    void FixedUpdate()
    {
       if (!isGameOver)
        {
            Vector2 position = rigidbody2d.position;
            position.x = position.x + speed * horizontal * Time.deltaTime;
            position.y = position.y + speed * vertical * Time.deltaTime;
            rigidbody2d.MovePosition(position);
        }
    }
    public void TriggerCheckpoint()
    {
        hasTriggeredCheckpoint = true;
        CheckWinCondition();

    }
        void CheckWinCondition()
    {
        // Check if the player has enough water and reached the checkpoint
        if (WaterCount >= totalWaterCount && hasTriggeredCheckpoint)
        {
            StartCoroutine(WinGame());
        }
    }
    public void ChangeWaterCount(int count)
        {
            WaterCount += count;
            UpdateWaterText(); 
            PlayWaterVFX();
            // Check if the player has enough water and reached the checkpoint
            if (WaterCount >= totalWaterCount && hasTriggeredCheckpoint)
            {
                WinGame();
            }
        }
    void UpdateWaterText()
    {
        // Assuming you have a TextMeshProUGUI variable for displaying candies count
        if (WaterCountText != null)
        {
            WaterCountText.text = "Water: " + WaterCount.ToString();
        }
    }
     void PlayWaterVFX()
    {
        // Play water drop VFX on the player
        if (WaterVFXPrefab != null)
        {
            GameObject WaterVFXObject = Instantiate(WaterVFXPrefab, transform.position, Quaternion.identity);
            WaterVFXObject.transform.parent = transform;

            ParticleSystem particleSystem = WaterVFXObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }
    }
    
     IEnumerator WinGame()
    {
        isGameOver = true;
        winText.gameObject.SetActive(true);
        winText.text = "Congrats! You Win!";
        PlaySound(winSound);
        // Display win message or perform other actions
        Debug.Log("Congrats! You Win!");
        yield return new WaitForSeconds(2.0f);
         winText.gameObject.SetActive(false);
        ThankYou.SetActive(true);
    }

    IEnumerator LoseGame()
    {
        isGameOver = true;
        loseText.gameObject.SetActive(true);
        loseText.text = "Haha!You lost!";
        PlaySound(loseSound);
        // Display lose message or perform other actions
        Debug.Log("Haha!You lost!");
        yield return new WaitForSeconds(2.0f);
        loseText.gameObject.SetActive(false);
        ThankYou.SetActive(true);
    }

    void RestartGame()
    {
        // Reset relevant variables
        WaterCount = 0;
        isGameOver = false;
        timer = 10.0f;
        speed = 4.0f;
        ThankYou.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
     void UpdateCountdownText()
    {
        if (countdownText != null)
        {
            countdownText.text = "Time: " + Mathf.CeilToInt(timer).ToString(); // Update the countdown text
        }
    }
    public void PlaySound(AudioClip clip)
    {
        Debug.Log(audioSource);
            if (audioSource == null)
        {
            Debug.LogError("audioSource is null!");
            return;
        }
        audioSource.PlayOneShot(clip);
    }
    
}