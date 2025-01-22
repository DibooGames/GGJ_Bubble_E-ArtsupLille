using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatWind : MonoBehaviour
{
    [SerializeField] private float upwardForce = 10f; // Configurable force parameter

   public BoxCollider boxCollider;
   [SerializeField] private GameObject Player;



    void Update()

    {
        if(boxCollider.bounds.Contains(Player.transform.position))
        {
            Debug.Log("Player is in the wind zone");
            Player.GetComponent<Rigidbody>().AddForce(Vector3.up * upwardForce, ForceMode.Force);
        }




    }

    
}
