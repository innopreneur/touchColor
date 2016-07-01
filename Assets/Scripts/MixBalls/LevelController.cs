using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    /*** inspector contolled variables ***/
    public int columns;
    public GameObject border;
    public GameObject block;
    public GameObject ball;
    public GameObject pickup;
    public float wallHeight = 10f;
    public int maxBlockSizeFactor = 6;
    public int ballCount, pickupCount;
    public int ballSizeFactor, pickupSizeFactor;

    /*** internal variables ****/

    public static LevelController instance;
    public int pickupsEnabledCount;
    private float screenHeight, screenWidth, leftPoint, rightPoint, topPoint, bottomPoint;
    private Camera mainCam;

    private BoxCollider topCollider, bottomCollider, leftCollider, rightCollider;
    private Renderer topRenderer, bottomRenderer, leftRenderer, rightRenderer;
    private GameObject topBorder, bottomBorder, leftBorder, rightBorder;
    private Vector3 topLeft, bottomLeft, topRight, bottomRight = Vector3.zero;
    private float cell, xMax, xMin, yMax, yMin;
    private int rows;
    private Vector3 bottomBlockPos, topBlockPos, leftBlockPos, rightBlockPos;
    private Vector3 horizontalBlockSize, verticalBlockSize;
    Vector3[,] totalPos;
    private int tempPosIndex = -1;

    void Awake()
    {
        //singleton check
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }

        
        //assign main camera
        mainCam = Camera.main;

        //retrieve boundary points
        topLeft = mainCam.ScreenToWorldPoint(new Vector3(0f, Screen.height, 0f));
        bottomLeft = mainCam.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        topRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        bottomRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));

        //assign boundary points
        xMax = bottomRight.x;
        xMin = bottomLeft.x;
        yMax = topLeft.z;
        yMin = bottomLeft.z;

        //basic unit width
        cell = (xMax - xMin) / columns;
        //basic unit height
        rows = Mathf.CeilToInt((yMax - yMin) / cell);

        totalPos = new Vector3[rows,columns];

        //pickup to be triggered
        pickupsEnabledCount = pickupCount;
        
    }


    void Start()
    {
        GetAllPositions();
        GenerateBorders();
        GenerateBlocks();
        GenerateBalls();
        GeneratePickUps();

    }

    private void GetAllPositions()
    {
        for (int i = 1; i <= rows; i++)
        {
            for (int j = 1; j <= columns; j++)
            {
                totalPos[i - 1, j - 1] = new Vector3(xMin + cell * j, 1f, yMin + cell * i);
            }
        }
    }

    private void GenerateBorders()
    {

        //instantiate and set borders
        topBorder = (GameObject)Instantiate(border, Vector3.zero, Quaternion.identity);
        topBorder.name = "TopBorder";
        topBorder.transform.position = new Vector3(topLeft.x, 1f, topLeft.z);
        topBorder.transform.localScale = new Vector3(bottomRight.x * 4, wallHeight, cell);
        topBorder.GetComponent<BoxCollider>().size = topBorder.transform.localScale;

        bottomBorder = (GameObject)Instantiate(border, Vector3.zero, Quaternion.identity);
        bottomBorder.name = "BottomBorder";
        bottomBorder.transform.position = new Vector3(topLeft.x, 1f, bottomLeft.z);
        bottomBorder.transform.localScale = new Vector3(bottomRight.x * 4, wallHeight, cell);
        bottomBorder.GetComponent<BoxCollider>().size = bottomBorder.transform.localScale;

        leftBorder = (GameObject)Instantiate(border, Vector3.zero, Quaternion.identity);
        leftBorder.name = "LeftBorder";
        leftBorder.transform.position = new Vector3(topLeft.x, 1f, topLeft.z);
        leftBorder.transform.localScale = new Vector3(cell, wallHeight, bottomRight.z * 4);
        leftBorder.GetComponent<BoxCollider>().size = leftBorder.transform.localScale;

        rightBorder = (GameObject)Instantiate(border, Vector3.zero, Quaternion.identity);
        rightBorder.name = "RightBorder";
        rightBorder.transform.position = new Vector3(topRight.x, 1f, topRight.z);
        rightBorder.transform.localScale = new Vector3(cell, wallHeight, bottomRight.z * 4);
        rightBorder.GetComponent<BoxCollider>().size = rightBorder.transform.localScale;


    }

    private void GenerateBlocks()
    {

        //blocks starting Positions
        bottomBlockPos = totalPos[0, GetRandom(0, columns)];
        topBlockPos = new Vector3(Random.Range(xMin, xMax), 1f, yMax);
        leftBlockPos = new Vector3(xMin, 1f, Random.Range(yMin, yMax));
        rightBlockPos = new Vector3(xMax, 1f, Random.Range(yMin, yMax));

        //array of all block positions
        Vector3[] blockPos = new Vector3 [] { bottomBlockPos, topBlockPos, leftBlockPos, rightBlockPos };

        //horizontal / vertical block size
        horizontalBlockSize = new Vector3(cell * Random.Range(columns/2, columns), wallHeight, cell);
        verticalBlockSize = new Vector3(cell, wallHeight, cell * Random.Range(columns/2, columns));

        //array of all block sizes
        Vector3[] blockSize = new Vector3[] { horizontalBlockSize, verticalBlockSize };

        int blockCount = Mathf.CeilToInt(Mathf.Sqrt(GameManager.instance.levelCount));
       
 
        for (int i = 0; i < blockCount; i++)
        {
            int random = Random.Range(0, 4);

            
            GameObject _block;
            int size = Random.Range(4, columns * (3/2));

            if (random == 0)
            {
                //bottom position blocks with vertical alignment
                _block = InstantiateBlock(block, 0, 0, columns, false, true, size);
                _block.transform.localScale = new Vector3(cell, wallHeight, cell * size);
            }
            else if(random == 1)
            {
                //top position blocks with vertical alignment
                _block = InstantiateBlock(block, rows - 1, 0, columns, false, true, size);
                _block.transform.localScale = new Vector3(cell, wallHeight, cell * size);
            }
            else if (random == 2)
            {
                //left position blocks with horizontal alignment
                _block = InstantiateBlock(block, 0, 0, rows, true, false, size);
                _block.transform.localScale = new Vector3(cell * size, wallHeight, cell);
            }
            else if (random == 3)
            {
                //right position blocks with horizontal alignment
                _block = InstantiateBlock(block, columns - 1, 0, rows, true, false, size);
                _block.transform.localScale = new Vector3(cell * size, wallHeight, cell);
            }
        }
    }

    private void GenerateBalls()
    {
        //instantiate balls at random position
        for (int i = 0; i < ballCount; i++)
        {

            Vector3 pos = Vector3.zero;

            while (pos == Vector3.zero)
            {
                pos = totalPos[Random.Range(3, rows - 3), Random.Range(3, columns - 3)];

            }
            GameObject _ball = (GameObject)Instantiate(ball, pos, Quaternion.identity);
            _ball.transform.localScale = Vector3.one * cell * ballSizeFactor;
        }
       
    }

    private void GeneratePickUps()
    {   
        //instantiate pickups at random position
        for (int i = 0; i < pickupCount; i++)
        {
            Vector3 pos = Vector3.zero;
           
            while (pos == Vector3.zero)
            {
                pos = totalPos[Random.Range(2, rows - 2), Random.Range(2, columns - 2)];
            }

            GameObject _pickup = (GameObject)Instantiate(pickup, pos, Quaternion.identity);
            _pickup.transform.localScale = Vector3.one * cell * pickupSizeFactor;

        }
    }

    private int GetRandom(int min, int max)
    {
        tempPosIndex = Random.Range(min, max);
        return tempPosIndex;
    }

    private void RemovePosition(int i, int j)
    {
        Debug.Log("Remove Position - (" + i + "," + j + ")");
        totalPos[i, j] = Vector3.zero;
    }

    private void RemovePositionRange(int fromX, int toX, int fromY, int toY)
    {
        for (int i = fromX; i < toX; i++)
        {
            for (int j = fromY; j < toY; j++)
            {
                totalPos[i, j] = Vector3.zero;
                Debug.Log("Removed - (i, j) - (" + i + "," + j + ")");
            }
        }
    }

    private GameObject InstantiateBlock(GameObject prefab, int nonRandom, int min, int max, bool isRowRandom, bool isVertical, int size)
    {
        Vector3 randPos;
        int randomNo;
        GameObject _obj = new GameObject() ;
        bool loopMore = true;

        randomNo = GetRandom(min, max);

        if (!isRowRandom)
        {
                
                randPos = totalPos[nonRandom, randomNo];
         
        }
        else
        {
                randPos = totalPos[randomNo, nonRandom];
   
        }

        

        while (loopMore)
        {

            if (randPos != Vector3.zero)
            {
                _obj = (GameObject)Instantiate(prefab, randPos, Quaternion.identity);
                loopMore = false;

                if (!isVertical)
                {
                    RemovePositionRange(randomNo, randomNo, nonRandom, size);
                    RemovePosition(randomNo, nonRandom);
                }
                else
                {
                    RemovePositionRange(nonRandom, size, randomNo, randomNo);
                    RemovePosition(nonRandom, randomNo);
                }
            }
                
            else
            {
                randomNo = GetRandom(0, columns);
                randPos = totalPos[0, randomNo];
            }
        }

        return _obj;
    }
}
