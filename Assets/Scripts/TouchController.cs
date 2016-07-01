using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour
{

    public LayerMask blockLayer;
    public float blockAscendFactor = 5f;
  
    private Transform playerTransform;
    private List<GameObject> touchedBlocks;
  
    // Use this for initialization
    void Awake()
    {

        playerTransform = GetComponent<Transform>();
        touchedBlocks = new List<GameObject>();
    }

    void Update()
    {

        //check if touch is stationary or moving
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
                //does the Ray hit player?
            if (Physics.Raycast(ray, out hit, blockLayer))
            {
                if (hit.collider.tag == "Block")
                {
                    if (!touchedBlocks.Contains(hit.transform.gameObject))
                    {
                        GameObject block = hit.transform.gameObject;
                        //add touched block to list
                        touchedBlocks.Add(block);
                        //ascend touched block
                        AscendBlockOnTouch(block);
                        //if touched, color it darker shade
                        ChangeBlockColor(block, false);
                    }
                   
                }
                else
                {   
                    //if player taps other things except block, update block colour
                    LevelManager.instance.updateBlockColor = true;
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (CheckCurrentTouch())
            {
                //increment current target
                LevelManager.instance.currentTarget += 1;
                LevelManager.instance.updateBlockColor = true;
            }

            if (CheckLevelComplete())
            {
                Debug.Log("Level Complete");
                LevelManager.instance.isLevelComplete = true;
            }

            ResetBlockPosition();

            //reset each touched block color to normal 
            foreach (GameObject block in touchedBlocks)
            {
                ChangeBlockColor(block, true);
            }
            touchedBlocks.Clear();
        }
    }


   private bool CheckCurrentTouch()
    {
        Debug.Log("count matches target -" + touchedBlocks.Count);

        //if total blocks touched matches current target block touches
        if (touchedBlocks.Count == LevelManager.instance.currentTarget)
        {
            
            foreach (GameObject block in touchedBlocks)
            {

                Color clr;
                //whether color dictionary has this color entry ! if yes, return color value
                if (LevelManager.instance.colorEntry.TryGetValue(LevelManager.instance.targetColor , out clr))
                {
                    Debug.Log("dict clr -" + clr);
                    Debug.Log("touched block clr - " + block.GetComponent<Renderer>().material.color);
                    //whether our touched block's color matches with target color
                    Renderer rend = block.GetComponent<Renderer>();
                    if (rend.material.color != clr)
                    {
                        return false;
                    }
                   

                }
            }

            return true;
        }

        return false;
    }

    private bool CheckLevelComplete()
    {
        if (LevelManager.instance.currentTarget > LevelManager.instance.levelTarget)
        {
            return true;
        }

        return false;

    }

    private void AscendBlockOnTouch(GameObject block)
    {
        block.transform.position += Vector3.up * blockAscendFactor;
    }

    private void ResetBlockPosition()
    {
        foreach (GameObject block in touchedBlocks)
        {
            block.transform.position -= Vector3.up * blockAscendFactor;
        }
    }

 
    //change block color based on touch down on touch up
    private void ChangeBlockColor(GameObject block, bool shouldReset)
    {
        Renderer rend = block.GetComponent<Renderer>();
        Color col = rend.material.color;
        if (shouldReset)
        {
            rend.material.color = new Vector4(col.r, col.g + 50, col.b, col.a);
        }
        else
        {
            rend.material.color = new Vector4(col.r, col.g - 50, col.b, col.a);
        }
       
    }
}



