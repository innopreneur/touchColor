using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{

    public int rows; //Z
    public int columns; //X
    public GameObject blockPrefab;
    public GameObject parentFloor;
    public float gapBetweenBlocks = 0.2f;
    public float gapAroundBlocks = 1f;
    public float blockSize = 2f;
    public Color[] blockColors;
    public Dictionary<string, Color> colorEntry;//FOR PROTOTYPE ONLY
    public static LevelManager instance = null;
    public bool updateBlockColor = false;
    public bool isLevelComplete = false;
    public int levelTarget = 5;
    public int currentTarget = 1;
    public Text colorTarget, targetLevel, targetCurrent;

    public Material blockMaterial;

    private Transform floorTransform;
    private float deltaBlockPosX, deltaBlockPosZ, totalBlockPosX, totalBlockPosZ;
    private Renderer blockRenderer;
    private GameObject[] blockPool;
    private int poolCount = 0;

    public string targetColor = ""; //FOR PROTOTYPE ONLY
    private string[] colorNames = { "red", "green", "blue", "white", "yellow" }; //FOR PROTOTYPE ONLY


    void Awake()
    {
        //make level manager instance as singleton
        if (instance == null)
        {
            instance = this;       
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    
    void Start()
    {

        blockColors = InitializeColors(blockColors);
        floorTransform = parentFloor.GetComponent<Transform>();
        blockPool = new GameObject[columns * rows];
        colorEntry = new Dictionary<string, Color>();

        //starting X and Z position of block generation
        SetBlockDefaultPositions();

        //Generate blocks
        GenerateBlocks();

        //Initialize Color dictionary
        InitializeColorDictionary();

        targetColor = colorNames[Random.Range(0, colorNames.Length)];

        colorTarget.text = targetColor;
        targetLevel.text = "Target - " + levelTarget;
        targetCurrent.text = "Current - " + currentTarget;
    }

    // Update is called once per frame
    void Update()
    {

        if (updateBlockColor || isLevelComplete)
        {
            //update colors of blocks after each turn
            UpdateBlockColor();

        }

        targetCurrent.text = "Current - " + currentTarget;
    }

    void GenerateBlocks()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {

                GameObject newBlock = (GameObject)Instantiate(blockPrefab, new Vector3(deltaBlockPosX, 1, deltaBlockPosZ), Quaternion.identity);
                newBlock.transform.localScale = Vector3.one * blockSize;
                blockRenderer = newBlock.GetComponent<Renderer>();
                blockRenderer.material = blockMaterial;
                blockRenderer.material.color = GetRandomBlockColor();
                blockPool[poolCount] = newBlock;

                //increment current Z position of block generation
                deltaBlockPosZ += (gapBetweenBlocks + blockSize);
                poolCount++;
            }

            //total position occupied by block on Z axis
            totalBlockPosZ = deltaBlockPosZ;

            //reset Z default position
            deltaBlockPosZ = -((floorTransform.localScale.z - blockSize) / 2);

            //increment current X position of block generation
            deltaBlockPosX += (gapBetweenBlocks + blockSize);
        }

        //total position occupied by block on X axis
        totalBlockPosX = deltaBlockPosX;
    }

    //updates color of every block
    private void UpdateBlockColor()
    {
        for (int i = 0; i < blockPool.Length; i++)
        {
            blockPool[i].GetComponent<Renderer>().material.color = GetRandomBlockColor();

        }
        updateBlockColor = false;
        Debug.Log("Block color updated");
    }

    private Vector3 GetTotalBlockAreaAfterGeneration()
    {
        totalBlockPosX = columns * (gapBetweenBlocks + blockSize);
        totalBlockPosZ = rows * (gapBetweenBlocks + blockSize);

        return new Vector3(totalBlockPosX, blockSize, totalBlockPosZ);
    }

    //returns random color from array of pre-defined colors
    private Color GetRandomBlockColor()
    {
        return blockColors[Random.Range(0, blockColors.Length)];
    }

    //sets block position to start block generation from
    private void SetBlockDefaultPositions()
    {
        deltaBlockPosX = -((floorTransform.localScale.x - blockSize) / 2);
        deltaBlockPosZ = -((floorTransform.localScale.z - blockSize) / 2);
    }

    //FOR PROTOTYPE ONLY
    //fill dictionary with color entries
    private void InitializeColorDictionary()
    {

        colorEntry.Add("red", blockColors[0]);
        colorEntry.Add("green", blockColors[1]);
        colorEntry.Add("blue", blockColors[2]);
        colorEntry.Add("white", blockColors[3]);
        colorEntry.Add("yellow", blockColors[4]);

    }




    //TODO
    void GenerateUnderLyingPlatform()
    {
        //TODO automate floor generation based on screen size and width
        //Vector3 screenBoundsMax = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width - 1, 1, (float)Screen.height - 1));
        //Vector3 screenBoundsMin = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width - (Screen.width - 1), 1, (float)Screen.height - (Screen.height - 1)));

        Vector3 blockArea = GetTotalBlockAreaAfterGeneration();
        floorTransform.localScale = new Vector3(blockArea.x + gapAroundBlocks, 1, blockArea.z + gapAroundBlocks);

    }

    private Color[] InitializeColors(Color[] blockColors)
    {
        blockColors = new Color[5];
        blockColors[0] = Color.red;
        blockColors[1] = Color.green;
        blockColors[2] = Color.blue;
        blockColors[3] = Color.white;
        blockColors[4] = Color.yellow;

        return blockColors;
    }
}
