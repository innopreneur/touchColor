using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{

  

    private Transform myTransform;
    private Renderer myRenderer;
    private Rigidbody myRbgd;
    private SphereCollider myCollider;
    public float g = 10f;
    private float forceX, forceZ;

    // Use this for initialization
    void Awake()
    {

        myTransform = GetComponent<Transform>();
        myRbgd = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
        myCollider = GetComponent<SphereCollider>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.isStarted)
        {
            var gravity = new Vector3(
               Input.acceleration.x,
               Input.acceleration.z,
               Input.acceleration.y
           ) * g;

            myRbgd.AddForce(gravity, ForceMode.Acceleration);

        }
    }


        void OnCollisionEnter(Collision other)
    {
        Transform objTransform = other.gameObject.GetComponent<Transform>();

        if(objTransform.tag == "Ball")
        {
            UpdateBallProp(objTransform);
        }
        else if(objTransform.tag == "Wall")
        {
            myCollider.isTrigger = false;
        }
    }

    private void UpdateBallProp(Transform otherBall)
    {
        Renderer rend = otherBall.GetComponent<Renderer>();

        if (otherBall.localScale.x > myTransform.localScale.x)
        {
            //disable me
            this.gameObject.SetActive(false);
        }
        else
        {
            //change my color to other ball color
            myRenderer.material.color = rend.material.color;

            //increase my size by half of other ball size
          //  myTransform.localScale += otherBall.localScale * 0.5f;
        }

        //update my position based on size gain
        myTransform.position = new Vector3(myTransform.position.x, myTransform.localScale.y, myTransform.position.z);
    }

}
