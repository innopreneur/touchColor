using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            LevelController.instance.pickupsEnabledCount--;
            GameManager.instance.increaseTime = true;
            gameObject.SetActive(false);
        }
    }
}
