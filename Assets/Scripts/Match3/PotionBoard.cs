using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotionBoard : MonoBehaviour
{
    //tamanho do tabuleiro
    public int width = 6;
    public int height = 8;
    //variaveis pra centralizar o tabuleiro de acordo com seu tamanho
    public float spacingX;
    public float spacingY;
    //pega os prefabs desejados
    public GameObject[] potionPrefabs;
    
    public List<GameObject> potionsToDestroy = new();
    public GameObject potionParent;

    [SerializeField] private Potion selectedPotion;
    [SerializeField] private bool isProcessingMove; 

    public Node[,] potionBoard;
    public GameObject potionBoardGO;
    //Cria um array da Classe ArrayLayout
    public ArrayLayout arrayLayout;
     //
    public static PotionBoard Instance;

    //MOBILE INPUT
    private Vector2 initialClickPosition;
    private bool isDragging = false;
    private float touchHoldDuration = 0.1f; // Duração para considerar um toque leve
    private float touchTime = 0f; // Tempo do toque
    private bool isTouching = false;


    private void Awake() 
    {
        Instance = this;    
    }

    void Start()
    {
        InitializeBoard();
        CheckBoard(true);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialClickPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(initialClickPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())
            {
                if (isProcessingMove)
                    return;

                Potion potion = hit.collider.gameObject.GetComponent<Potion>();
                Debug.Log("Poção clicada: " + potion.gameObject);

                SelectPotion(potion);
                isDragging = true; // Começa o arrasto
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            //lógica para destacar a poção selecionada ou mostrar feedback visual
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 finalClickPosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(finalClickPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())
            {
                Potion targetPotion = hit.collider.gameObject.GetComponent<Potion>();
                Debug.Log("Poção alvo: " + targetPotion.gameObject);

                // Verifica se as poções estão adjacentes e faz a troca
                if (selectedPotion != null && IsAdjacent(selectedPotion, targetPotion))
                {
                    SwapPotion(selectedPotion, targetPotion);
                }
            }

            isDragging = false; // Finaliza o arrasto
            selectedPotion = null; // Reseta a poção selecionada
        }
    }

    void InitializeBoard()
    {   
        DestroyPotions();
        potionBoard = new Node[width, height];

        spacingX = (float)(width -1)/ 2;
        spacingY = (float)(height - 1) / 2;
        

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                if (arrayLayout.rows[y].row[x])
                {
                    potionBoard[x,y] = new Node(false, null);
                }
                else 
                {
                int randomIndex = Random.Range(0, potionPrefabs.Length);

                GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity);
                potion.transform.SetParent(potionParent.transform);
                potion.GetComponent<Potion>().SetIndicies(x, y);
                potionBoard[x,y] = new Node(true, potion);
                potionsToDestroy.Add(potion);
                }
            }
        }
       //if(CheckBoard(false))
       //{
       // InitializeBoard();
       //}
       //else{
       // Debug.Log("Deu boa");
       //}
    }

    public void DestroyPotions()
    {
        if (potionsToDestroy != null)
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                Destroy(potion);
            }
            potionsToDestroy.Clear();
        }
    }

     public bool CheckBoard(bool _takeAction)
    {
        Debug.Log("Checking Board");
        bool hasMatched = false;

        List<Potion> potionsToRemove = new();

        foreach (Node nodePotion in potionBoard)
        {
            if (nodePotion.potion != null)
            {
                nodePotion.potion.GetComponent<Potion>().isMatched = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //verifica se a poção é usável
                if (potionBoard[x,y].isUsable)
                {
                    //pega o node do potion
                    Potion potion = potionBoard[x,y].potion.GetComponent<Potion>();

                    if (!potion.isMatched)
                    {
                        MatchResult matchedPotions = IsConnected(potion);

                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            MatchResult superMatchPotions = SuperMatch(matchedPotions);

                            potionsToRemove.AddRange(superMatchPotions.connectedPotions);

                            foreach(Potion pot in superMatchPotions.connectedPotions)
                                pot.isMatched = true;

                            hasMatched = true;
                            
                        }
                    }

                }

            }
        }

        if (_takeAction)
        {

            foreach (Potion potionToRemove in potionsToRemove)
            {
                potionToRemove.isMatched = false;
            }

            RemoveAndRefill(potionsToRemove);

             if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }

        return hasMatched;
    }


    private void RemoveAndRefill(List<Potion> _potionsToRemove)
    {
        //Removing the potion and cleaning the board at the location 
        foreach (Potion potion in _potionsToRemove)
        {
            //getting its x and y indicies
            int _xIndex = potion.xIndex;
            int _yIndex = potion.yIndex;

            //Destroy the potion
            Destroy(potion.gameObject);

            //Create a blank node on the board
            potionBoard[_xIndex, _yIndex] = new Node(true, null);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y=0; y <height; y++)
            {
                if (potionBoard[x,y].potion == null)
                {
                    Debug.Log("The Location X: " + x + "Y: " + y + "is empty, reffill it");
                    RefillPotion(x,y);
                }
            }
        }
    }

    private void RefillPotion(int x, int y)
    {
        //y offset
        int yOffset = 1;

        //while the cell above our current cell is null and we're below the height of the board
        while (y + yOffset < height && potionBoard[x,y + yOffset].potion == null)
        {
            yOffset++;
        }

        if (y + yOffset < height && potionBoard[x,y + yOffset].potion != null)
        {
            Potion potionAbove = potionBoard[x,y + yOffset].potion.GetComponent<Potion>();

            //Move to correct Loc
            Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, potionAbove.transform.position.z);
            
            potionAbove.MoveToTarget(targetPos);
            //update indicies
            potionAbove.SetIndicies(x,y);
            potionBoard[x,y] = potionBoard[x,y + yOffset];

            potionBoard[x,y + yOffset] = new Node(true, null);
        }

        if (y + yOffset == height)
        {
            SpawnPotionAtTop(x);
        }
    }

    private void SpawnPotionAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationToMove = height - index;
        
        //get a random potion
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);

        newPotion.GetComponent<Potion>().SetIndicies(x, index);
        potionBoard[x, index] = new Node(true, newPotion);
        Vector3 targetPosition = new Vector3(newPotion.transform.position.x, newPotion.transform.position.y - locationToMove, newPotion.transform.position.z);
        newPotion.GetComponent<Potion>().MoveToTarget(targetPosition);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = height - 1; y >= 0; y--)
        {
            if (potionBoard[x,y].potion == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #region Cascading Potions

    //RefillPotions

    //SpawnPotionAtTop()


    #endregion


    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            foreach (Potion pot in _matchedResults.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();

                CheckDirection(pot, new Vector2Int(0, 1), extraConnectedPotions);

                CheckDirection(pot, new Vector2Int(0, -1), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    Debug.Log("Super Horizontal Match");
                    extraConnectedPotions.AddRange(_matchedResults.connectedPotions);

                    return new MatchResult
                    {
                        connectedPotions = extraConnectedPotions, direction = MatchDirection.Super
                    };
                }
            } 
            return new MatchResult
            {
                connectedPotions = _matchedResults.connectedPotions, direction = _matchedResults.direction
            };
        }


         else if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Potion pot in _matchedResults.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();

                CheckDirection(pot, new Vector2Int(1, 0), extraConnectedPotions);

                CheckDirection(pot, new Vector2Int(-1, 0), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    Debug.Log("Super Vertical Match");
                    extraConnectedPotions.AddRange(_matchedResults.connectedPotions);

                    return new MatchResult
                    {
                        connectedPotions = extraConnectedPotions, direction = MatchDirection.Super
                    };
                }
            } 
            return new MatchResult
            {
                connectedPotions = _matchedResults.connectedPotions, direction = _matchedResults.direction
            };
        }
        return null;
    }

    MatchResult IsConnected(Potion potion)
    {
        List<Potion> connectedPotions = new();
        PotionType potionType = potion.potionType;

        connectedPotions.Add(potion);

        //check right
        CheckDirection(potion, new Vector2Int(1,0), connectedPotions);
        //check left
        CheckDirection(potion, new Vector2Int(-1,0), connectedPotions);
        //match with 3 potions(Horizontal)
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Simple Horizontal Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        //match with more than 3 potions (Long Horizontal Match)
        else if (connectedPotions.Count >3)
        {
             Debug.Log("Long Horizontal Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal
            };
        }
        //clear out the connected potions
        connectedPotions.Clear();
        //readd our initial potion
        connectedPotions.Add(potion);

        //check up
        CheckDirection(potion, new Vector2Int(0,1), connectedPotions);
        //check down
        CheckDirection(potion, new Vector2Int(0,-1), connectedPotions);
        //match with 3 potions(Vertical)
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Simple Vertical Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        //match with more than 3 potions (Long Vertical Match)
        else if (connectedPotions.Count >3)
        {
             Debug.Log("Long Vertical Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical
            };
        } else
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.None
            };
        }
    }

    void CheckDirection(Potion pot, Vector2Int direction, List<Potion> connectedPotions)
    {
        PotionType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        //verifica se está dentro dos limites do board/tabuleiro
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x,y].isUsable)
            {
                Potion neighbourPotion = potionBoard[x,y].potion.GetComponent<Potion>();

                //confere se a poção vizinha também não deu match
                if (!neighbourPotion.isMatched && neighbourPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    #region Swapping Potions

    //select pot:on
    private void SelectPotion(Potion _potion)
    {
        //Se não tiver nenhuma poção selecionada irá selecionar a primeira que eu clicar com o mouse
        if (selectedPotion == null)
        {
            Debug.Log(_potion);
            selectedPotion = _potion;
        }
        //Se selecionar a mesma poção duas vezez a poção selecionada será considerada nula
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }
        //Se a poção selecionada não for nula e nem for a mesma poção ela irá realizar o movimento de troca
        else if (selectedPotion != _potion)
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
        //Poção selecionada retornará para null
    }
    //swap potion - logic
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        DoSwap(_currentPotion, _targetPotion);


        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }

    //do swap
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;

        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        //update indicies.
        int tempXIndex = _currentPotion.xIndex;
        int tempYIndex = _currentPotion.yIndex;
        _currentPotion.xIndex = _targetPotion.xIndex;
        _currentPotion.yIndex = _targetPotion.yIndex;
        _targetPotion.xIndex = tempXIndex;
        _targetPotion.yIndex = tempYIndex;

        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);
    }

    private IEnumerator ProcessMatches(Potion _currentPotion, Potion _targetPotion)
    {
        yield return new WaitForSeconds(0.2f);

        bool hasMatch = CheckBoard(true);

        if (!hasMatch)
        {
            DoSwap(_currentPotion, _targetPotion);
        }
        isProcessingMove = false;
    }


    //isAdjacent
    private bool IsAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        return Math.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Math.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
    }

    //ProcessMatches

    #endregion

}

public class MatchResult
{
    public List<Potion> connectedPotions;
    public MatchDirection direction;

}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}
