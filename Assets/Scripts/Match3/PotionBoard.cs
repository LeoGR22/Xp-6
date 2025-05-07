using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = UnityEngine.Random;

public class PotionBoard : MonoBehaviour
{
    // Tamanho do tabuleiro
    private int width = 4;
    private int height = 4;
    public BoardSizeData widthData;
    public BoardSizeData heightData;

    // SetLevel
    SetLevel level;
    bool win;

    // Variáveis para centralizar o tabuleiro de acordo com seu tamanho
    private float spacingX;
    private float spacingY;

    // Cria uma lista com os prefabs desejados
    public GameObject[] potionPrefabs;

    public List<GameObject> potionsToDestroy = new();
    public GameObject potionParent;

    // Prefab do VFX de explosão (branco)
    public GameObject explosionVFXPrefab;

    // Mapeamento de cores por ItemType
    private Dictionary<ItemType, Color> potionColors = new Dictionary<ItemType, Color>
    {
        { ItemType.Violet, new Color(0.67f, 0.39f, 0.86f) }, 
        { ItemType.Green, new Color(0.39f, 0.78f, 0.39f) },  
        { ItemType.Red, new Color(0.86f, 0.39f, 0.39f) },    
        { ItemType.Orange, new Color(0.94f, 0.63f, 0.31f) }  
    };

    [SerializeField] private Potion selectedPotion;
    [SerializeField] private bool isProcessingMove;

    public Node[,] potionBoard;
    public GameObject potionBoardGO;

    // Cria um array da Classe ArrayLayout
    public ArrayLayout arrayLayout;

    public static PotionBoard Instance;

    // Variáveis que armazenam o toque na tela mobile e definem as poções selecionadas
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    Potion clickedPotion = null;
    public Potion targetPotion = null;

    private bool playerMadeAMove = false;

    // Variáveis para armazenar os potions coletados
    public ObjectiveBoardData violetPotionCount;
    public ObjectiveBoardData greenPotionCount;
    public ObjectiveBoardData redPotionCount;
    public ObjectiveBoardData orangePotionCount;

    public GameEvent WinGame;
    public GameEvent LoseGame;
    public BooleanSO canLose;

    // Define qual tipo de condição de derrota
    private Timer timer;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        win = false;
        width = widthData.GetSize();
        height = heightData.GetSize();
        level = FindObjectOfType<SetLevel>();
        timer = FindObjectOfType<Timer>();

        InitializeBoard();
        CheckBoard(true);
    }

    private void Update()
    {
        CheckUserActions();
    }

    private void CheckUserActions()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;

            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())
            {
                if (isProcessingMove) return;

                clickedPotion = hit.collider.gameObject.GetComponent<Potion>();
                Debug.Log("Poção clicada: " + clickedPotion.gameObject);

                SelectPotion(clickedPotion);
            }
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPosition = Input.GetTouch(0).position;

            if (Mathf.Abs(endTouchPosition.x - startTouchPosition.x) > Mathf.Abs(endTouchPosition.y - startTouchPosition.y))
            {
                if (endTouchPosition.x > startTouchPosition.x)
                {
                    targetPotion = GetPotionAt(clickedPotion.xIndex + 1, clickedPotion.yIndex);
                    playerMadeAMove = true;
                    SelectPotion(targetPotion);
                    Debug.Log("Swipe para direita");
                }
                else
                {
                    targetPotion = GetPotionAt(clickedPotion.xIndex - 1, clickedPotion.yIndex);
                    playerMadeAMove = true;
                    SelectPotion(targetPotion);
                    Debug.Log("Swipe para esquerda");
                }
            }
            else
            {
                if (endTouchPosition.y > startTouchPosition.y)
                {
                    targetPotion = GetPotionAt(clickedPotion.xIndex, clickedPotion.yIndex + 1);
                    playerMadeAMove = true;
                    SelectPotion(targetPotion);
                    Debug.Log("Swipe para cima");
                }
                else
                {
                    targetPotion = GetPotionAt(clickedPotion.xIndex, clickedPotion.yIndex - 1);
                    playerMadeAMove = true;
                    SelectPotion(targetPotion);
                    Debug.Log("Swipe para baixo");
                }
            }
        }
    }

    // Método para obter a poção na posição especificada
    private Potion GetPotionAt(int xIndex, int yIndex)
    {
        if (xIndex >= 0 && xIndex < width && yIndex >= 0 && yIndex < height)
        {
            if (potionBoard[xIndex, yIndex].potion != null)
            {
                return potionBoard[xIndex, yIndex].potion.GetComponent<Potion>();
            }
        }
        return null; // Retorna nulo se a posição estiver fora dos limites ou não houver poção
    }

    void InitializeBoard()
    {
        bool validBoard = false;

        while (!validBoard)
        {
            DestroyPotions();
            potionBoard = new Node[width, height];

            spacingX = (float)(width - 1) / 2;
            spacingY = (float)(height - 1) / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 position = new Vector2(x - spacingX, y - spacingY);
                    if (arrayLayout.rows[y].row[x])
                    {
                        potionBoard[x, y] = new Node(false, null);
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, potionPrefabs.Length);
                        GameObject potion = Instantiate(potionPrefabs[randomIndex], position, Quaternion.identity);
                        potion.transform.SetParent(potionParent.transform);
                        potion.GetComponent<Potion>().SetIndicies(x, y);
                        potionBoard[x, y] = new Node(true, potion);
                        potionsToDestroy.Add(potion);
                    }
                }
            }

            // Ensure no initial matches and at least one move exists
            validBoard = !CheckBoard(false) && HasPossibleMoves();
        }
    }

    public void DestroyPotions()
    {
        if (potionsToDestroy != null)
        {
            foreach (GameObject potion in potionsToDestroy)
            {
                if (potion != null)
                {
                    Destroy(potion);
                }
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
                // Verifica se a poção é usável
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
                {
                    // Pega o node do potion
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if (!potion.isMatched)
                    {
                        MatchResult matchedPotions = IsConnected(potion);

                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            MatchResult superMatchPotions = SuperMatch(matchedPotions);

                            potionsToRemove.AddRange(superMatchPotions.connectedPotions);

                            foreach (Potion pot in superMatchPotions.connectedPotions)
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
                if (potionToRemove != null)
                {
                    potionToRemove.isMatched = false;
                }
            }

            foreach (Potion potionToRemove in potionsToRemove)
            {
                if (potionToRemove != null)
                {
                    if (potionToRemove.potionType == ItemType.Violet && violetPotionCount != null)
                    {
                        if (violetPotionCount.count > 0) { violetPotionCount.count--; }
                    }
                    else if (potionToRemove.potionType == ItemType.Green && greenPotionCount != null)
                    {
                        if (greenPotionCount.count > 0) { greenPotionCount.count--; }
                    }
                    else if (potionToRemove.potionType == ItemType.Red && redPotionCount != null)
                    {
                        if (redPotionCount.count > 0) { redPotionCount.count--; }
                    }
                    else if (potionToRemove.potionType == ItemType.Orange && orangePotionCount != null)
                    {
                        if (orangePotionCount.count > 0) { orangePotionCount.count--; }
                    }
                }
            }

            if (violetPotionCount.count + greenPotionCount.count + orangePotionCount.count + redPotionCount.count <= 0)
            {
                if (!win)
                {
                    win = true;

                    GameObject targetPlayer = GameObject.FindWithTag("Player");
                    PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();
                    playerManager.AddMoney((int)timer.GetMovesLeft() * 5);

                    level.PassLevel();
                }
                canLose.value = false;
                WinGame.Raise();
            }

            if (hasMatched && playerMadeAMove)
            {
                playerMadeAMove = false;
                timer.DecreaseMove();
            }

            RemoveAndRefill(potionsToRemove);
        }

        return hasMatched;
    }

    private void RemoveAndRefill(List<Potion> _potionsToRemove)
    {
        // Inicia a coroutine para animar e destruir as poções
        StartCoroutine(AnimateAndRemovePotions(_potionsToRemove));
    }

    private IEnumerator AnimateAndRemovePotions(List<Potion> _potionsToRemove)
    {
        // Calcula o ponto central do grupo de poções
        Vector3 centerPoint = Vector3.zero;
        int validPotionCount = 0;
        Potion firstValidPotion = null;

        foreach (Potion potion in _potionsToRemove)
        {
            if (potion != null && potion.gameObject != null)
            {
                centerPoint += potion.transform.position;
                validPotionCount++;
                if (firstValidPotion == null)
                {
                    firstValidPotion = potion; // Guarda a primeira poção válida
                }
            }
        }

        // Evita divisão por zero
        if (validPotionCount > 0)
        {
            centerPoint /= validPotionCount;
        }
        else
        {
            // Se não houver poções válidas, prossegue sem animação
            yield return StartCoroutine(RefillBoardSimultaneously());
            yield break;
        }

        // Anima todas as poções para o centro
        float animationDuration = 0.2f; // Duração do efeito (ajustável)
        foreach (Potion potion in _potionsToRemove)
        {
            if (potion != null && potion.gameObject != null)
            {
                potion.MoveToTarget(centerPoint, animationDuration);
            }
        }

        // Aguarda até que todas as poções terminem de se mover
        yield return new WaitUntil(() => AreAllPotionsSettled());

        // Instancia o VFX de explosão no ponto central com a cor correspondente
        if (explosionVFXPrefab != null && validPotionCount > 0 && firstValidPotion != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, centerPoint, Quaternion.identity);
            ParticleSystem particleSystem = vfx.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = potionColors[firstValidPotion.potionType];
            }
        }

        // Destroi as poções marcadas para remoção e limpa o tabuleiro
        foreach (Potion potion in _potionsToRemove)
        {
            if (potion != null && potion.gameObject != null)
            {
                int _xIndex = potion.xIndex;
                int _yIndex = potion.yIndex;
                // Limpa a referência no tabuleiro antes de destruir
                if (_xIndex >= 0 && _xIndex < width && _yIndex >= 0 && _yIndex < height)
                {
                    potionBoard[_xIndex, _yIndex] = new Node(true, null);
                }
                Destroy(potion.gameObject);
            }
        }

        // Inicia a coroutine para preencher simultaneamente
        yield return StartCoroutine(RefillBoardSimultaneously());
    }

    private IEnumerator RefillBoardSimultaneously()
    {
        bool hasEmptySpaces = true;

        while (hasEmptySpaces)
        {
            hasEmptySpaces = false;
            List<int> columnsToRefill = new List<int>();

            // Identifica colunas com pelo menos um espaço vazio
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (potionBoard[x, y].potion == null && potionBoard[x, y].isUsable)
                    {
                        columnsToRefill.Add(x);
                        hasEmptySpaces = true;
                        break; // Uma vez que a coluna tem um espaço vazio, não precisa verificar mais
                    }
                }
            }

            if (hasEmptySpaces)
            {
                // Processa uma poção por coluna simultaneamente
                foreach (int x in columnsToRefill)
                {
                    RefillColumnSingleStep(x);
                }

                // Aguarda todas as poções terminarem de se mover
                yield return new WaitUntil(() => AreAllPotionsSettled());
                // Adiciona um pequeno atraso para efeito visual
                yield return new WaitForSeconds(0.05f);
            }
        }

        // Após preencher todas as colunas, verifica o tabuleiro novamente
        yield return StartCoroutine(ResolveCascadingMatches());
        yield return StartCoroutine(TryShuffleWithDelay());
    }

    private void RefillColumnSingleStep(int x)
    {
        // Encontra o menor índice y vazio na coluna
        int lowestEmptyY = -1;
        for (int y = 0; y < height; y++)
        {
            if (potionBoard[x, y].potion == null && potionBoard[x, y].isUsable)
            {
                lowestEmptyY = y;
                break;
            }
        }

        if (lowestEmptyY == -1) return; // Coluna não tem espaços vazios

        // Procura a primeira poção acima do espaço vazio
        int yOffset = 1;
        while (lowestEmptyY + yOffset < height && potionBoard[x, lowestEmptyY + yOffset].potion == null)
        {
            yOffset++;
        }

        if (lowestEmptyY + yOffset < height && potionBoard[x, lowestEmptyY + yOffset].potion != null)
        {
            // Move a poção de cima para o espaço vazio
            Potion potionAbove = potionBoard[x, lowestEmptyY + yOffset].potion.GetComponent<Potion>();
            Vector3 targetPos = new Vector3(x - spacingX, lowestEmptyY - spacingY, potionAbove.transform.position.z);
            potionAbove.MoveToTarget(targetPos);
            potionAbove.SetIndicies(x, lowestEmptyY);
            potionBoard[x, lowestEmptyY] = potionBoard[x, lowestEmptyY + yOffset];
            potionBoard[x, lowestEmptyY + yOffset] = new Node(true, null);
        }
        else
        {
            // Instancia uma nova poção no topo para o espaço vazio
            SpawnPotionAtTop(x, lowestEmptyY);
        }
    }

    private void SpawnPotionAtTop(int x, int targetY)
    {
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        Potion potionComponent = newPotion.GetComponent<Potion>();
        potionComponent.SetIndicies(x, targetY);
        potionBoard[x, targetY] = new Node(true, newPotion);
        Vector3 targetPosition = new Vector3(x - spacingX, targetY - spacingY, newPotion.transform.position.z);
        potionComponent.MoveToTarget(targetPosition);
    }

    public bool HasPossibleMoves()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
                {
                    Potion currentPotion = potionBoard[x, y].potion.GetComponent<Potion>();
                    ItemType potionType = currentPotion.potionType;
                    if (CheckPotentialMatch(x, y, potionType))
                    {
                        Debug.Log($"Possible move at ({x}, {y}) for {potionType}");
                        return true;
                    }
                }
            }
        }
        Debug.Log("No possible moves found on the board.");
        return false;
    }

    private void TryShuffleBoardIfNoMoves()
    {
        if (!HasPossibleMoves())
        {
            int maxAttempts = 100;
            int attempts = 0;

            do
            {
                ShuffleBoard();
                attempts++;
            } while (!HasPossibleMoves() && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Não foi possível gerar um tabuleiro com jogadas possíveis após várias tentativas.");
            }
        }
    }

    private bool CheckPotentialMatch(int x, int y, ItemType potionType)
    {
        Vector2Int[] directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        foreach (var dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;
            if (newX >= 0 && newX < width && newY >= 0 && newY < height && potionBoard[newX, newY].isUsable && potionBoard[newX, newY].potion != null)
            {
                Potion target = potionBoard[newX, newY].potion.GetComponent<Potion>();
                SwapForCheck(x, y, newX, newY);
                bool hasMatch = CheckBoard(false);
                SwapForCheck(newX, newY, x, y);
                if (hasMatch)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SwapForCheck(int x1, int y1, int x2, int y2)
    {
        var temp = potionBoard[x1, y1];
        potionBoard[x1, y1] = potionBoard[x2, y2];
        potionBoard[x2, y2] = temp;

        potionBoard[x1, y1].potion?.GetComponent<Potion>().SetIndicies(x1, y1);
        potionBoard[x2, y2].potion?.GetComponent<Potion>().SetIndicies(x2, y2);
    }

    public void ShuffleBoard()
    {
        Debug.Log("Nenhum movimento possível. Embaralhando o tabuleiro...");

        List<Potion> allPotions = new List<Potion>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
                {
                    allPotions.Add(potionBoard[x, y].potion.GetComponent<Potion>());
                }
            }
        }

        potionBoard = new Node[width, height];
        List<Vector2> availablePositions = new List<Vector2>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!arrayLayout.rows[y].row[x])
                {
                    availablePositions.Add(new Vector2(x, y));
                    potionBoard[x, y] = new Node(true, null);
                }
                else
                {
                    potionBoard[x, y] = new Node(false, null);
                }
            }
        }

        foreach (Potion potion in allPotions)
        {
            if (availablePositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                Vector2 pos = availablePositions[randomIndex];
                int x = (int)pos.x;
                int y = (int)pos.y;

                potionBoard[x, y] = new Node(true, potion.gameObject);
                potion.SetIndicies(x, y);
                Vector3 targetPos = new Vector3(x - spacingX, y - spacingY, potion.transform.position.z);
                potion.MoveToTarget(targetPos);

                availablePositions.RemoveAt(randomIndex);
            }
        }
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = height - 1; y >= 0; y--)
        {
            if (potionBoard[x, y].potion == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #region Cascading Potions

    // SpawnPotionAtTop já está definido acima

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
                        connectedPotions = extraConnectedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = _matchedResults.connectedPotions,
                direction = _matchedResults.direction
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
                        connectedPotions = extraConnectedPotions,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = _matchedResults.connectedPotions,
                direction = _matchedResults.direction
            };
        }
        return null;
    }

    MatchResult IsConnected(Potion potion)
    {
        List<Potion> connectedPotions = new();
        ItemType potionType = potion.potionType;

        connectedPotions.Add(potion);

        // Check right
        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        // Check left
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);
        // Match with 3 potions (Horizontal)
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Simple Horizontal Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        // Match with more than 3 potions (Long Horizontal Match)
        else if (connectedPotions.Count > 3)
        {
            Debug.Log("Long Horizontal Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal
            };
        }
        // Clear out the connected potions
        connectedPotions.Clear();
        // Readd our initial potion
        connectedPotions.Add(potion);

        // Check up
        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        // Check down
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);
        // Match with 3 potions (Vertical)
        if (connectedPotions.Count == 3)
        {
            Debug.Log("Simple Vertical Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        // Match with more than 3 potions (Long Vertical Match)
        else if (connectedPotions.Count > 3)
        {
            Debug.Log("Long Vertical Match with " + connectedPotions[0].potionType + " potion");

            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongVertical
            };
        }
        else
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
        ItemType potionType = pot.potionType;
        int x = pot.xIndex + direction.x;
        int y = pot.yIndex + direction.y;

        // Verifica se está dentro dos limites do board/tabuleiro
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();

                // Confere se a poção vizinha também não deu match
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

    private bool AreAllPotionsSettled()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].potion != null)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();
                    if (potion != null && potion.isMoving)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    #region Swapping Potions

    // Select potion
    private void SelectPotion(Potion _potion)
    {
        if (selectedPotion == null)
        {
            Debug.Log(_potion);
            selectedPotion = _potion;
        }
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }
        else if (selectedPotion != _potion)
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }

    // Swap potion - logic
    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        DoSwap(_currentPotion, _targetPotion);
        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }

    // Do swap
    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

        // Update indices
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
        isProcessingMove = true;

        // Inicia a animação da troca
        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);

        // Espera até que a troca termine
        while (_currentPotion.isMoving || _targetPotion.isMoving)
            yield return null;

        bool hasMatch = CheckBoard(true);

        if (!hasMatch)
        {
            // Reverte a troca
            DoSwap(_currentPotion, _targetPotion);
            _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
            _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);
            while (_currentPotion.isMoving || _targetPotion.isMoving)
                yield return null;
            isProcessingMove = false;
            yield break;
        }

        yield return StartCoroutine(ResolveCascadingMatches());

        // Verifica se há jogadas possíveis e embaralha se necessário
        yield return StartCoroutine(TryShuffleWithDelay());

        isProcessingMove = false;
    }

    private IEnumerator ResolveCascadingMatches()
    {
        bool hasMoreMatches = true;

        while (hasMoreMatches)
        {
            hasMoreMatches = CheckBoard(false);
            if (hasMoreMatches)
            {
                CheckBoard(true); // Isso chama RemoveAndRefill, que inicia animações
                yield return new WaitUntil(() => AreAllPotionsSettled()); // Espera todas as poções pararem
            }
        }
    }

    private IEnumerator TryShuffleWithDelay()
    {
        yield return new WaitUntil(() => AreAllPotionsSettled()); // Garante que o tabuleiro está estável

        if (!HasPossibleMoves())
        {
            Debug.Log("No moves detected after settling.");
            int maxAttempts = 100;
            int attempts = 0;

            do
            {
                ShuffleBoard();
                yield return new WaitUntil(() => AreAllPotionsSettled()); // Espera o shuffle terminar
                attempts++;
            } while (!HasPossibleMoves() && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to generate a solvable board after max attempts.");
            }
            else
            {
                Debug.Log("Board shuffled successfully!");
            }
        }
        else
        {
            Debug.Log("Moves available; no shuffle needed.");
        }
    }

    // IsAdjacent
    private bool IsAdjacent(Potion _currentPotion, Potion _targetPotion)
    {
        if (_targetPotion != null)
        {
            return Math.Abs(_currentPotion.xIndex - _targetPotion.xIndex) + Math.Abs(_currentPotion.yIndex - _targetPotion.yIndex) == 1;
        }
        else
        {
            return false;
        }
    }

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