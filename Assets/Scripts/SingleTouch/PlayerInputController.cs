using UnityEngine;
using System.Collections;

public class PlayerInputController : MonoBehaviour {

    public LayerMask playerLayer;
    public float forceFactorY = 2f;

    private Transform playerTransform;
    private Rigidbody playerRbgd;
    public float g = 9.8f;

    // Use this for initialization
    void Awake () {

        playerTransform = GetComponent<Transform>();
        playerRbgd = GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        var gravity = new Vector3(
           Input.acceleration.x,
           Input.acceleration.z,
           Input.acceleration.y
       ) * g;

        if(Input.acceleration.z > 0.2)
        {
            playerRbgd.useGravity = false;
            
        }
        else
        {
            playerRbgd.useGravity = true;
        }
        playerRbgd.AddForce(gravity, ForceMode.Acceleration);

    }

    private void AddUpwardsForce()
    {
        Vector3 vel = playerRbgd.velocity;
        playerRbgd.AddForce(new Vector3(vel.x, vel.y + forceFactorY, vel.z));

    }

    private void AddSideForce()
    {
        float x = Input.acceleration.x;
        float y = Input.acceleration.y;
        //playerRbgd.AddForce(Vector3.right * x * 5);
        playerTransform.Translate(new Vector3( x * Time.deltaTime * 20, y * Time.deltaTime * 20, 0));
    }
}
