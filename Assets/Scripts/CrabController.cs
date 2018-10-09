using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonoBehaviour {


    GameObject player;

    bool inCombat = false;

    void EnableCombat()
    {
        player.GetComponent<PlayerMovement>().ToggleInCombat();
        player.GetComponent<PlayerMovement>().SetTarget(gameObject);
        inCombat = true;

        Debug.Log("Enabled");
    }


    public void ToggleCombat()
    {
        inCombat = !inCombat;
    }

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {

        float dist = Vector3.Distance(player.transform.position, transform.position);
		if(dist < 10 && !inCombat)
        {
            EnableCombat();
        }
	}

    void OnDestroy()
    {
        if (player != null) { player.GetComponent<PlayerMovement>().BreakFromCombat(); } 
    }
}
