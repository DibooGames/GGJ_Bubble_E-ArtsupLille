using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
  
    public static GameManager instance;

    public GameObject player;
    public GameObject Camera;
    public SensitivityController sensitivityController;
    public FlyMovement flyMovement;
    public PlayerInput playerInput;



    public float BubbleIntegrity = 100;
    public float BubbleIntegrityMax = 100;

    public float dryDamageRate = 5f; // New field for custom damage rate

    public bool isInDryArea = false;
    
    public UnityEvent OnLose;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        isInDryArea = false;

        
    }
  
    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {

        // Decrease BubbleIntegrity over time if in dry area
        if (isInDryArea)
        {
            BubbleIntegrity -= dryDamageRate * Time.deltaTime;
        }

        if (BubbleIntegrity <= 0)
        {
            BubbleIntegrity = 0;
            Debug.Log("Game Over");
            OnLose?.Invoke();

        }

        if (BubbleIntegrity > BubbleIntegrityMax)
        {
            BubbleIntegrity = BubbleIntegrityMax;
        }



    }

    public void ReloadStats()
    {
        BubbleIntegrity = BubbleIntegrityMax;
        
    }

    public void UpdateIntegrity()
    {
        if (isInDryArea)
        {
            BubbleIntegrity -= dryDamageRate * Time.deltaTime;
        }

        if (BubbleIntegrity <= 0)
        {
            BubbleIntegrity = 0;
            Debug.Log("Game Over");
            OnLose?.Invoke();

        }

        if (BubbleIntegrity > BubbleIntegrityMax)
        {
            BubbleIntegrity = BubbleIntegrityMax;
        }
    }

}
