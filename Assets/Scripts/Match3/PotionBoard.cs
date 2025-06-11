using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI.Table;
using Random = UnityEngine.Random;
using CandyCoded.HapticFeedback;
using DG.Tweening;
public class PotionBoard : MonoBehaviour
{
    private int width = 4;
    private int height = 4;
    public BoardSizeData widthData;
    public BoardSizeData heightData;

    public BooleanSO tutorialSO;
    private GameObject tutorialGO;

    public BooleanSO canMove;

    SetLevel level;
    bool win;

    private float spacingX;
    private float spacingY;

    public GameObject[] potionPrefabs;
    public List<GameObject> potionsToDestroy = new();
    public GameObject potionParent;

    public GameObject explosionVFXPrefab;

    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private float extraMargin = 0.1f;
    [SerializeField] private float backgroundScale = 0.8f;
    private List<GameObject> backgroundTiles = new List<GameObject>();

    public GameObject centerTilePrefab;
    public GameObject topLeftTilePrefab;
    public GameObject topRightTilePrefab;
    public GameObject bottomLeftTilePrefab;
    public GameObject bottomRightTilePrefab;
    public GameObject tileParent;
    private List<GameObject> tilesToDestroy = new();

    private Dictionary<ItemType, Color> potionColors = new Dictionary<ItemType, Color>

{
    { ItemType.Violet, new Color(0.67f, 0.39f, 0.86f) },
    { ItemType.Green, new Color(0.82f, 0.71f, 0.55f) },
    { ItemType.Red, new Color(0.86f, 0.39f, 0.39f) },
    { ItemType.Orange, new Color(0.94f, 0.63f, 0.31f) },
    { ItemType.Blue, new Color(0.39f, 0.58f, 0.93f) }
};

    [SerializeField] private Potion selectedPotion;
    [SerializeField] private bool isProcessingMove;

    public Node[,] potionBoard;
    public GameObject potionBoardGO;

    public ArrayLayout arrayLayout;

    public static PotionBoard Instance;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    Potion clickedPotion = null;
    public Potion targetPotion = null;

    private bool playerMadeAMove = false;

    public ObjectiveBoardData violetPotionCount;
    public ObjectiveBoardData greenPotionCount;
    public ObjectiveBoardData redPotionCount;
    public ObjectiveBoardData orangePotionCount;
    public ObjectiveBoardData bluePotionCount;

    [SerializeField] private float finalPotionScale = 0.7f;
    [SerializeField] private float worldSpacing = 0.9f;
    [SerializeField] private float uiSpacing = 50f;
    [SerializeField] private float worldCenterX = 0f;
    [SerializeField] private float uiCenterX = 0f;

    private GameObject violetSymbolGO;
    private GameObject greenSymbolGO;
    private GameObject redSymbolGO;
    private GameObject orangeSymbolGO;
    private GameObject blueSymbolGO;

    public GameObject violetCountUI;
    public GameObject greenCountUI;
    public GameObject redCountUI;
    public GameObject orangeCountUI;
    public GameObject blueCountUI;

    public GameEvent WinGame;
    public GameEvent LoseGame;
    public BooleanSO canLose;
    private bool won = false;

    private Timer timer;

    [SerializeField] private GameObject comboMessagePrefab;
    [SerializeField] private Sprite[] comboMessageSprites;
    private int cascadeComboCount = 0;

    // Controle de inatividade e animação
    private float lastMoveTime;
    private bool isShaking;
    private readonly float inactivityThreshold = 5f;
    private Coroutine shakeCoroutine;
    private List<(List<Potion> matchPotions, int x1, int y1, int x2, int y2)> possibleMatches;
    private (List<Potion> matchPotions, int x1, int y1, int x2, int y2)? selectedMatch;

    private static int checkBoardCallCount = 0;
    private HashSet<Potion> processedPotions = new HashSet<Potion>();

    [SerializeField] private GameObject glowVFXPrefab;

    private List<ParticleSystem> activeGlowEffects = new List<ParticleSystem>();

    // Tutorial
    private bool isTutorialLevel;
    [SerializeField] private TutorialBoardLayout tutorialLayout;

    [System.Serializable]
    public class TutorialBoardLayout
    {
        public int width;
        public int height;
        public TutorialTile[] tiles;
    }

    [System.Serializable]
    public class TutorialTile
    {
        public int x;
        public int y;
        public ItemType potionType;
        public bool isUsable;
    }

    private void Awake()
    {
        Instance = this;

        isTutorialLevel = tutorialSO.value;

        if (isTutorialLevel)
        {
            tutorialLayout = new TutorialBoardLayout
            {
                width = 3,
                height = 3,
                tiles = new TutorialTile[]
                {
            new TutorialTile { x = 0, y = 0, potionType = ItemType.Violet, isUsable = true },
            new TutorialTile { x = 1, y = 0, potionType = ItemType.Blue, isUsable = true },
            new TutorialTile { x = 2, y = 0, potionType = ItemType.Red, isUsable = true },
            new TutorialTile { x = 0, y = 1, potionType = ItemType.Red, isUsable = true },
            new TutorialTile { x = 1, y = 1, potionType = ItemType.Red, isUsable = true },
            new TutorialTile { x = 2, y = 1, potionType = ItemType.Blue, isUsable = true },
            new TutorialTile { x = 0, y = 2, potionType = ItemType.Orange, isUsable = true },
            new TutorialTile { x = 1, y = 2, potionType = ItemType.Green, isUsable = true },
            new TutorialTile { x = 2, y = 2, potionType = ItemType.Violet, isUsable = true }
                }
            };
        }
    }

    void Start()
    {
        win = false;
        level = FindObjectOfType<SetLevel>();
        timer = FindObjectOfType<Timer>();

        lastMoveTime = Time.time;
        possibleMatches = new List<(List<Potion> matchPotions, int x1, int y1, int x2, int y2)>();
        selectedMatch = null;
        InitializeBoard();
        AssignCountUIs();

        violetSymbolGO = GameObject.FindGameObjectWithTag("Violet");
        greenSymbolGO = GameObject.FindGameObjectWithTag("Green");
        redSymbolGO = GameObject.FindGameObjectWithTag("Red");
        orangeSymbolGO = GameObject.FindGameObjectWithTag("Orange");
        blueSymbolGO = GameObject.FindGameObjectWithTag("Blue");
        tutorialGO = GameObject.FindGameObjectWithTag("Tutorial");

        if (isTutorialLevel)
        {
            timer.ChangeMoves(3);

            redPotionCount.count = 3;
            violetPotionCount.count = 0;
            greenPotionCount.count = 0;
            orangePotionCount.count = 0;
            bluePotionCount.count = 0;
        }

        UpdateUIVisibility();
        RepositionCountUIs();

        CheckBoard(true);
    }

    private bool hasLost = false;

    private void Update()
    {
        if (canMove.value)
        {
            bool isVictory = violetPotionCount.count + greenPotionCount.count + orangePotionCount.count +
                             redPotionCount.count + bluePotionCount.count <= 0 && !won;
            if (isVictory)
            {
                if (!win)
                {
                    win = true;
                    GameObject targetPlayer = GameObject.FindWithTag("Player");
                    PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();
                    playerManager.AddMoney(10 + (int)timer.GetMovesLeft() * 5);
                    level.PassLevel();
                }
                canLose.value = false;
                won = true;
                WinGame.Raise();
                return;
            }

            if (!won && timer.GetMovesLeft() > 0)
            {
                CheckUserActions();
                if (!isShaking && Time.time - lastMoveTime > inactivityThreshold && AreAllPotionsSettled() && !isTutorialLevel)
                {
                    if (shakeCoroutine != null)
                    {
                        StopCoroutine(shakeCoroutine);
                        shakeCoroutine = null;
                    }
                    shakeCoroutine = StartCoroutine(ShakePossibleMatch());
                }
            }
        }
    }

    private void CheckUserActions()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null && hit.collider.gameObject.GetComponent<Potion>())
                {
                    if (isProcessingMove) return;

                    clickedPotion = hit.collider.gameObject.GetComponent<Potion>();
                }
            }
            else if (touch.phase == TouchPhase.Moved && clickedPotion != null)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 delta = currentTouchPosition - startTouchPosition;
                float swipeThreshold = 50f;

                if (delta.magnitude > swipeThreshold)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        if (delta.x > 0)
                        {
                            targetPotion = GetPotionAt(clickedPotion.xIndex + 1, clickedPotion.yIndex);
                        }
                        else
                        {
                            targetPotion = GetPotionAt(clickedPotion.xIndex - 1, clickedPotion.yIndex);
                        }
                    }
                    else
                    {
                        if (delta.y > 0)
                        {
                            targetPotion = GetPotionAt(clickedPotion.xIndex, clickedPotion.yIndex + 1);
                        }
                        else
                        {
                            targetPotion = GetPotionAt(clickedPotion.xIndex, clickedPotion.yIndex - 1);
                        }
                    }

                    if (targetPotion != null)
                    {
                        playerMadeAMove = true;
                        SelectPotion(clickedPotion);
                        SelectPotion(targetPotion);
                        clickedPotion = null;
                    }
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                clickedPotion = null;
                targetPotion = null;
            }
        }
    }

    private void CheckLoseCondition()
    {
        // Verifica se há animações em andamento
        if (isAnimatingPotions)
        {
            Debug.Log("CheckLoseCondition: Animações em andamento, adiando verificação de derrota.");
            return;
        }

        bool isVictory = violetPotionCount.count + greenPotionCount.count + orangePotionCount.count +
                         redPotionCount.count + bluePotionCount.count <= 0 && !won;
        if (isVictory)
        {
            if (!win)
            {
                win = true;
                GameObject targetPlayer = GameObject.FindWithTag("Player");
                PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();
                playerManager.AddMoney(10 + (int)timer.GetMovesLeft() * 5);
                level.PassLevel();
            }
            canLose.value = false;
            won = true;
            WinGame.Raise();
            return;
        }

        Debug.Log($"CheckLoseCondition: MovesLeft={timer.GetMovesLeft()}, HasPossibleMoves={HasPossibleMoves()}");
        if (!hasLost && !won)
        {
            timer.CheckLose();
            if (timer.GetMovesLeft() <= 0 && !HasPossibleMoves() && canLose.value)
            {
                hasLost = true;
                timer.CheckLose();
            }
        }
    }

    private Potion GetPotionAt(int xIndex, int yIndex)
    {
        if (xIndex >= 0 && xIndex < width && yIndex >= 0 && yIndex < height)
        {
            if (potionBoard[xIndex, yIndex].potion != null)
            {
                return potionBoard[xIndex, yIndex].potion.GetComponent<Potion>();
            }
        }
        return null;
    }

    void InitializeBoard()
    {
        win = false;

        if (isTutorialLevel && tutorialLayout != null)
        {
            width = tutorialLayout.width;
            height = tutorialLayout.height;
        }
        else
        {
            width = widthData.GetSize();
            height = heightData.GetSize();
        }

        level = FindObjectOfType<SetLevel>();
        timer = FindObjectOfType<Timer>();

        lastMoveTime = Time.time;
        possibleMatches = new List<(List<Potion> matchPotions, int x1, int y1, int x2, int y2)>();
        selectedMatch = null;

        DestroyPotions();
        DestroyTiles();
        potionBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + .75f;

        CreateVisualBoard();

        if (isTutorialLevel && tutorialLayout != null)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 position = new Vector2(x - spacingX, y - spacingY);
                    TutorialTile tile = tutorialLayout.tiles.FirstOrDefault(t => t.x == x && t.y == y);

                    if (tile == null || !tile.isUsable)
                    {
                        potionBoard[x, y] = new Node(false, null);
                    }
                    else
                    {
                        GameObject potionPrefab = potionPrefabs.FirstOrDefault(p => p.GetComponent<Potion>().potionType == tile.potionType);
                        if (potionPrefab == null)
                        {
                            Debug.LogError($"Nenhum prefab encontrado para o tipo {tile.potionType} em ({x},{y})");
                            potionBoard[x, y] = new Node(false, null);
                            continue;
                        }

                        GameObject potion = Instantiate(potionPrefab, position, Quaternion.identity);
                        potion.transform.SetParent(potionParent.transform);
                        potion.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        SpriteRenderer potionRenderer = potion.GetComponent<SpriteRenderer>();
                        if (potionRenderer != null)
                        {
                            potionRenderer.sortingOrder = 3;
                        }
                        potion.GetComponent<Potion>().SetIndicies(x, y);
                        potionBoard[x, y] = new Node(true, potion);
                        potionsToDestroy.Add(potion);
                    }
                }
            }
        }
        else
        {
            bool validBoard = false;
            while (!validBoard)
            {
                DestroyPotions();
                DestroyTiles();
                potionBoard = new Node[width, height];

                spacingX = (float)(width - 1) / 2;
                spacingY = (float)((height - 1) / 2) + .75f;

                CreateVisualBoard();

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
                            potion.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                            SpriteRenderer potionRenderer = potion.GetComponent<SpriteRenderer>();
                            if (potionRenderer != null)
                            {
                                potionRenderer.sortingOrder = 3;
                            }
                            potion.GetComponent<Potion>().SetIndicies(x, y);
                            potionBoard[x, y] = new Node(true, potion);
                            potionsToDestroy.Add(potion);
                        }
                    }
                }

                validBoard = !CheckBoard(false) && HasPossibleMoves();
            }
        }
    }

    private void UpdateUIVisibility()
    {
        if (violetSymbolGO != null)
            violetSymbolGO.SetActive(violetPotionCount.count > 0);
        if (greenSymbolGO != null)
            greenSymbolGO.SetActive(greenPotionCount.count > 0);
        if (redSymbolGO != null)
            redSymbolGO.SetActive(redPotionCount.count > 0);
        if (orangeSymbolGO != null)
            orangeSymbolGO.SetActive(orangePotionCount.count > 0);
        if (blueSymbolGO != null)
            blueSymbolGO.SetActive(bluePotionCount.count > 0);

        if (violetCountUI != null)
            violetCountUI.SetActive(violetPotionCount.count > 0);
        if (greenCountUI != null)
            greenCountUI.SetActive(greenPotionCount.count > 0);
        if (redCountUI != null)
            redCountUI.SetActive(redPotionCount.count > 0);
        if (orangeCountUI != null)
            orangeCountUI.SetActive(orangePotionCount.count > 0);
        if (blueCountUI != null)
            blueCountUI.SetActive(bluePotionCount.count > 0);
    }

    private void RepositionCountUIs()
    {
        List<GameObject> symbolGOs = new List<GameObject> { violetSymbolGO, greenSymbolGO, redSymbolGO, orangeSymbolGO, blueSymbolGO };
        List<GameObject> countUIs = new List<GameObject> { violetCountUI, greenCountUI, redCountUI, orangeCountUI, blueCountUI };
        List<GameObject> activeSymbolGOs = symbolGOs.Where(go => go != null && go.activeSelf).ToList();
        List<GameObject> activeCountUIs = countUIs.Where(ui => ui != null && ui.activeSelf).ToList();

        if (activeSymbolGOs.Count == 0 || activeCountUIs.Count == 0)
        {
            Debug.LogWarning("Nenhum símbolo ou UI ativo encontrado.");
            return;
        }

        float worldYPosition = symbolGOs.FirstOrDefault(go => go != null)?.transform.position.y ?? 0f;

        float worldTotalWidth = (activeSymbolGOs.Count - 1) * worldSpacing;
        float worldStartX = worldCenterX - (worldTotalWidth / 2f);

        float uiTotalWidth = (activeCountUIs.Count - 1) * uiSpacing;
        float uiStartX = uiCenterX - (uiTotalWidth / 2f);

        for (int i = 0; i < activeSymbolGOs.Count; i++)
        {
            if (activeSymbolGOs[i] != null)
            {
                Vector3 newWorldPos = new Vector3(worldStartX + (i * worldSpacing), worldYPosition, activeSymbolGOs[i].transform.position.z);
                activeSymbolGOs[i].transform.position = newWorldPos;
                Debug.Log($"Reposicionando {activeSymbolGOs[i].name} para {newWorldPos}");
            }
        }

        for (int i = 0; i < activeCountUIs.Count; i++)
        {
            if (activeCountUIs[i] != null)
            {
                RectTransform uiRect = activeCountUIs[i].GetComponent<RectTransform>();
                if (uiRect != null)
                {
                    float currentY = uiRect.anchoredPosition.y;
                    float newX = uiStartX + (i * uiSpacing);
                    Vector2 newCanvasPos = new Vector2(newX, currentY);
                    uiRect.anchoredPosition = newCanvasPos;
                    Debug.Log($"Reposicionando {activeCountUIs[i].name} no Canvas para anchoredPosition: {newCanvasPos}");
                }
                else
                {
                    Debug.LogWarning($"RectTransform não encontrado em {activeCountUIs[i].name}.");
                }
            }
        }
    }

    private void CreateVisualBoard()
    {
        tilesToDestroy.Clear();
        backgroundTiles.Clear(); // Não usaremos mais backgroundTiles separadamente, mas mantemos a limpeza por segurança

        // Loop para criar os tiles do tabuleiro
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (isTutorialLevel && tutorialLayout != null)
                {
                    TutorialTile tutorialTile = tutorialLayout.tiles.FirstOrDefault(t => t.x == x && t.y == y);
                    if (tutorialTile == null || !tutorialTile.isUsable) continue;
                }
                else if (arrayLayout.rows[y].row[x]) continue;

                Vector2 position = new Vector2(x - spacingX, y - spacingY);
                GameObject tilePrefab;

                // Selecionar o prefab do tile
                if (x == 0 && y == 0)
                    tilePrefab = bottomLeftTilePrefab;
                else if (x == 0 && y == height - 1)
                    tilePrefab = topLeftTilePrefab;
                else if (x == width - 1 && y == 0)
                    tilePrefab = bottomRightTilePrefab;
                else if (x == width - 1 && y == height - 1)
                    tilePrefab = topRightTilePrefab;
                else
                    tilePrefab = centerTilePrefab;

                // Instanciar o tile
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.transform.SetParent(tileParent.transform, false);

                SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
                if (tileRenderer != null)
                {
                    tileRenderer.sortingOrder = 1; // Tiles acima do fundo
                }

                // Adicionar sprite de fundo como filho do tile
                if (backgroundSprites != null && backgroundSprites.Length >= 9)
                {
                    Sprite spriteToUse;
                    // Determinar qual sprite usar com base na posição
                    if (x == 0 && y == 0)
                        spriteToUse = backgroundSprites[0]; // Canto inferior esquerdo
                    else if (x == width - 1 && y == 0)
                        spriteToUse = backgroundSprites[2]; // Canto inferior direito
                    else if (x == 0 && y == height - 1)
                        spriteToUse = backgroundSprites[3]; // Canto superior esquerdo
                    else if (x == width - 1 && y == height - 1)
                        spriteToUse = backgroundSprites[4]; // Canto superior direito
                    else if (x == 0)
                        spriteToUse = backgroundSprites[1]; // Borda esquerda
                    else if (x == width - 1)
                        spriteToUse = backgroundSprites[5]; // Borda direita
                    else if (y == 0)
                        spriteToUse = backgroundSprites[6]; // Borda inferior
                    else if (y == height - 1)
                        spriteToUse = backgroundSprites[7]; // Borda superior
                    else
                        spriteToUse = backgroundSprites[8]; // Centro

                    // Criar GameObject filho para o sprite de fundo
                    GameObject bgTile = new GameObject($"BackgroundTile_{x}_{y}");
                    bgTile.transform.SetParent(tile.transform, false); // Filho do tile
                    bgTile.transform.localPosition = Vector3.zero; // Centralizado no tile
                    SpriteRenderer bgRenderer = bgTile.AddComponent<SpriteRenderer>();
                    bgRenderer.sprite = spriteToUse;
                    bgRenderer.sortingOrder = 0; // Fundo abaixo do tile

                    // Aplicar escala e margem
                    float bgScale = backgroundScale + extraMargin; // A margem aumenta o tamanho do fundo
                    bgTile.transform.localScale = new Vector3(bgScale, bgScale, 1f);
                }
                else
                {
                    Debug.LogWarning("Array de sprites de fundo incompleto ou não atribuído. Necessário 9 sprites.");
                }

                tilesToDestroy.Add(tile);
            }
        }
    }

    private void DestroyTiles()
    {
        if (tilesToDestroy != null)
        {
            foreach (GameObject tile in tilesToDestroy)
            {
                if (tile != null)
                {
                    Destroy(tile); // Destroi o tile e seus filhos (incluindo o sprite de fundo)
                }
            }
            tilesToDestroy.Clear();
        }

        // backgroundTiles não é mais necessário, mas mantemos a limpeza por segurança
        backgroundTiles.Clear();
    }

    private void AssignCountUIs()
    {
        CountLabel[] countLabels = FindObjectsOfType<CountLabel>();

        foreach (CountLabel countLabel in countLabels)
        {
            string labelName = countLabel.GetName();
            switch (labelName)
            {
                case "Violet":
                    violetCountUI = countLabel.gameObject;
                    break;
                case "Green":
                    greenCountUI = countLabel.gameObject;
                    break;
                case "Red":
                    redCountUI = countLabel.gameObject;
                    break;
                case "Orange":
                    orangeCountUI = countLabel.gameObject;
                    break;
                case "Blue":
                    blueCountUI = countLabel.gameObject;
                    break;
            }
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
            DestroyTiles();
        }
    }

    private bool isAnimatingPotions = false;
    private bool CheckBoard(bool _takeAction)
    {
        checkBoardCallCount++;
        Debug.Log($"CheckBoard call count: {checkBoardCallCount}, takeAction: {_takeAction}");

        bool hasMatched = false;

        List<Potion> potionsToRemove = new();
        HashSet<Potion> potionsToRemoveSet = new HashSet<Potion>();
        Dictionary<ItemType, List<Potion>> potionsByTypeInMatch = new Dictionary<ItemType, List<Potion>>();

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
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
                {
                    Potion potion = potionBoard[x, y].potion.GetComponent<Potion>();

                    if (processedPotions.Contains(potion) || potion.isMatched)
                    {
                        continue;
                    }

                    if (!potion.isMatched)
                    {
                        MatchResult matchedPotions = IsConnected(potion);

                        if (matchedPotions.connectedPotions.Count >= 3)
                        {
                            MatchResult superMatchPotions = SuperMatch(matchedPotions);

                            foreach (Potion pot in superMatchPotions.connectedPotions)
                            {
                                if (!potionsToRemoveSet.Contains(pot) && !processedPotions.Contains(pot))
                                {
                                    potionsToRemove.Add(pot);
                                    potionsToRemoveSet.Add(pot);

                                    ItemType type = pot.potionType;
                                    if (!potionsByTypeInMatch.ContainsKey(type))
                                    {
                                        potionsByTypeInMatch[type] = new List<Potion>();
                                    }
                                    potionsByTypeInMatch[type].Add(pot);
                                }
                            }

                            foreach (Potion pot in superMatchPotions.connectedPotions)
                            {
                                pot.isMatched = true;
                            }

                            hasMatched = true;
                        }
                    }
                }
            }
        }

        if (_takeAction)
        {
            if (tutorialSO.value)
            {
                tutorialGO.GetComponent<TutorialManager>().Next();
            }

            List<(Potion potion, ItemType type)> potionsToAnimate = new();
            Dictionary<ItemType, int> potionsToReduce = new();

            foreach (var kvp in potionsByTypeInMatch)
            {
                ItemType type = kvp.Key;
                List<Potion> potionsOfType = kvp.Value;
                int matchSize = potionsOfType.Count;

                AudioManager.Instance.PlaySFX("Match");
                HapticFeedback.LightFeedback();

                bool isRelevantForObjective = false;
                switch (type)
                {
                    case ItemType.Violet:
                        if (violetPotionCount != null && violetPotionCount.count > 0)
                        {
                            potionsToReduce[type] = matchSize;
                            isRelevantForObjective = true;
                        }
                        break;
                    case ItemType.Green:
                        if (greenPotionCount != null && greenPotionCount.count > 0)
                        {
                            potionsToReduce[type] = matchSize;
                            isRelevantForObjective = true;
                        }
                        break;
                    case ItemType.Red:
                        if (redPotionCount != null && redPotionCount.count > 0)
                        {
                            potionsToReduce[type] = matchSize;
                            isRelevantForObjective = true;
                        }
                        break;
                    case ItemType.Orange:
                        if (orangePotionCount != null && orangePotionCount.count > 0)
                        {
                            potionsToReduce[type] = matchSize;
                            isRelevantForObjective = true;
                        }
                        break;
                    case ItemType.Blue:
                        if (bluePotionCount != null && bluePotionCount.count > 0)
                        {
                            potionsToReduce[type] = matchSize;
                            isRelevantForObjective = true;
                        }
                        break;
                }

                if (isRelevantForObjective)
                {
                    foreach (Potion potion in potionsOfType)
                    {
                        if (potion != null && !processedPotions.Contains(potion))
                        {
                            potionsToAnimate.Add((potion, type));
                            processedPotions.Add(potion);
                        }
                    }
                }
            }

            if (potionsToAnimate.Count > 0)
            {
                string potionTypes = string.Join(", ", potionsToAnimate.Select(p => p.type.ToString()));
                Debug.Log($"Starting animation for {potionsToAnimate.Count} potions: [{potionTypes}]");
                StartCoroutine(AnimatePotionToUI(potionsToAnimate, potionsToReduce));
            }
            else
            {
                if (!isAnimatingPotions)
                    CheckLoseCondition();
            }

            if (hasMatched && playerMadeAMove)
            {
                playerMadeAMove = false;
                timer.DecreaseMove();
            }

            if (potionsToRemove.Count > 0)
            {
                RemoveAndRefill(potionsToRemove);
            }
        }

        return hasMatched;
    }

    private void RemoveAndRefill(List<Potion> _potionsToRemove)
    {
        StartCoroutine(AnimateAndRemovePotions(_potionsToRemove));
    }

    private IEnumerator AnimateAndRemovePotions(List<Potion> _potionsToRemove)
    {
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
                    firstValidPotion = potion;
                }
            }
        }

        if (validPotionCount > 0)
        {
            centerPoint /= validPotionCount;
        }
        else
        {
            StartCoroutine(RefillBoardSimultaneously());
            yield break;
        }

        float animationDuration = 0.2f;
        foreach (Potion potion in _potionsToRemove)
        {
            if (potion != null && potion.gameObject != null)
            {
                potion.MoveToTarget(centerPoint, animationDuration);
            }
        }

        yield return new WaitUntil(() => AreAllPotionsSettled());

        if (explosionVFXPrefab != null && validPotionCount > 0 && firstValidPotion != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, centerPoint, Quaternion.identity);
            Color potionColor = potionColors[firstValidPotion.potionType]; // Obtém a cor da poção

            // Obtém todos os ParticleSystem (raiz e filhos)
            ParticleSystem[] particleSystems = vfx.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem particleSystem in particleSystems)
            {
                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    main.startColor = potionColor; // Define a cor para cada sistema de partículas
                }
            }

            // Aplica configurações adicionais ao ParticleSystem principal
            ParticleSystem mainParticleSystem = vfx.GetComponent<ParticleSystem>();
            if (mainParticleSystem != null)
            {
                vfx.transform.localScale *= 1.5f;
                var emission = mainParticleSystem.emission;
                emission.rateOverTimeMultiplier *= 1.5f;
                ParticleSystemRenderer renderer = vfx.GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = 2;
                }
            }
        }

        foreach (Potion potion in _potionsToRemove)
        {
            if (potion != null && potion.gameObject != null)
            {
                int _xIndex = potion.xIndex;
                int _yIndex = potion.yIndex;
                if (_xIndex >= 0 && _xIndex < width && _yIndex >= 0 && _yIndex < height)
                {
                    if (potionBoard[_xIndex, _yIndex].potion == potion.gameObject)
                    {
                        potionBoard[_xIndex, _yIndex] = new Node(true, null);
                    }
                }
                Destroy(potion.gameObject);
            }
        }

        potionsToDestroy.RemoveAll(p => _potionsToRemove.Contains(p.GetComponent<Potion>()));

        StartCoroutine(RefillBoardSimultaneously());
    }

    private IEnumerator RefillBoardSimultaneously()
    {
        bool hasEmptySpaces = true;

        while (hasEmptySpaces)
        {
            hasEmptySpaces = false;
            List<int> columnsToRefill = new List<int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (potionBoard[x, y].potion == null && potionBoard[x, y].isUsable)
                    {
                        columnsToRefill.Add(x);
                        hasEmptySpaces = true;
                        break;
                    }
                }
            }

            if (hasEmptySpaces)
            {
                foreach (int x in columnsToRefill)
                {
                    RefillColumnSingleStep(x);
                }

                yield return new WaitUntil(() => AreAllPotionsSettled());
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return StartCoroutine(ResolveCascadingMatches());
        yield return StartCoroutine(TryShuffleWithDelay());
    }

    private void RefillColumnSingleStep(int x)
    {
        int lowestEmptyY = -1;
        for (int y = 0; y < height; y++)
        {
            if (potionBoard[x, y].potion == null && potionBoard[x, y].isUsable)
            {
                lowestEmptyY = y;
                break;
            }
        }

        if (lowestEmptyY == -1) return;

        int yOffset = 1;
        while (lowestEmptyY + yOffset < height && potionBoard[x, lowestEmptyY + yOffset].potion == null)
        {
            yOffset++;
        }

        if (lowestEmptyY + yOffset < height && potionBoard[x, lowestEmptyY + yOffset].potion != null)
        {
            Potion potionAbove = potionBoard[x, lowestEmptyY + yOffset].potion.GetComponent<Potion>();
            Vector3 targetPos = new Vector3(x - spacingX, lowestEmptyY - spacingY, potionAbove.transform.position.z);
            potionAbove.MoveToTarget(targetPos);
            potionAbove.SetIndicies(x, lowestEmptyY);
            potionBoard[x, lowestEmptyY] = potionBoard[x, lowestEmptyY + yOffset];
            potionBoard[x, lowestEmptyY + yOffset] = new Node(true, null);
        }
        else
        {
            SpawnPotionAtTop(x, lowestEmptyY);
        }
    }

    private void SpawnPotionAtTop(int x, int targetY)
    {
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject newPotion = Instantiate(potionPrefabs[randomIndex], new Vector2(x - spacingX, height - spacingY), Quaternion.identity);
        newPotion.transform.SetParent(potionParent.transform);
        newPotion.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        SpriteRenderer potionRenderer = newPotion.GetComponent<SpriteRenderer>();
        if (potionRenderer != null)
        {
            potionRenderer.sortingOrder = 3;
        }
        Potion potionComponent = newPotion.GetComponent<Potion>();
        potionComponent.SetIndicies(x, targetY);
        potionBoard[x, targetY] = new Node(true, newPotion);
        Vector3 targetPosition = new Vector3(x - spacingX, targetY - spacingY, newPotion.transform.position.z);
        potionComponent.MoveToTarget(targetPosition);
    }

    public bool HasPossibleMoves()
    {
        possibleMatches = new List<(List<Potion> matchPotions, int x1, int y1, int x2, int y2)>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
                {
                    Potion currentPotion = potionBoard[x, y].potion.GetComponent<Potion>();
                    ItemType potionType = currentPotion.potionType;
                    var (hasMatch, matchPotions, x1, y1, x2, y2) = CheckPotentialMatch(x, y, potionType);
                    if (hasMatch)
                    {
                        possibleMatches.Add((matchPotions, x1, y1, x2, y2));
                    }
                }
            }
        }
        return possibleMatches.Count > 0;
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
        }
    }

    private (bool hasMatch, List<Potion> matchPotions, int x1, int y1, int x2, int y2) CheckPotentialMatch(int x, int y, ItemType potionType)
    {
        Vector2Int[] directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
        foreach (var dir in directions)
        {
            int newX = x + dir.x;
            int newY = y + dir.y;
            if (newX >= 0 && newX < width && newY >= 0 && newY < height && potionBoard[newX, newY].isUsable && potionBoard[newX, newY].potion != null)
            {
                SwapForCheck(x, y, newX, newY);

                List<Potion> matchPotions = new List<Potion>();
                bool hasMatch = false;

                Potion movedPotion = potionBoard[newX, newY].potion?.GetComponent<Potion>();
                if (movedPotion != null)
                {
                    MatchResult matchResult = IsConnected(movedPotion);
                    if (matchResult.connectedPotions.Count >= 3)
                    {
                        matchPotions.AddRange(matchResult.connectedPotions);
                        hasMatch = true;
                    }
                }

                movedPotion = potionBoard[x, y].potion?.GetComponent<Potion>();
                if (movedPotion != null)
                {
                    MatchResult matchResult = IsConnected(movedPotion);
                    if (matchResult.connectedPotions.Count >= 3)
                    {
                        matchPotions.AddRange(matchResult.connectedPotions.Where(p => !matchPotions.Contains(p)));
                        hasMatch = true;
                    }
                }

                SwapForCheck(newX, newY, x, y);

                if (hasMatch)
                {
                    return (true, matchPotions, x, y, newX, newY);
                }
            }
        }
        return (false, new List<Potion>(), x, y, -1, -1);
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

    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.LongHorizontal)
        {
            HashSet<Potion> uniquePotions = new HashSet<Potion>(_matchedResults.connectedPotions);
            foreach (Potion pot in _matchedResults.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();

                CheckDirection(pot, new Vector2Int(0, 1), extraConnectedPotions);
                CheckDirection(pot, new Vector2Int(0, -1), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    uniquePotions.UnionWith(extraConnectedPotions);
                    return new MatchResult
                    {
                        connectedPotions = new List<Potion>(uniquePotions),
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = new List<Potion>(uniquePotions),
                direction = _matchedResults.direction
            };
        }
        else if (_matchedResults.direction == MatchDirection.Vertical || _matchedResults.direction == MatchDirection.LongVertical)
        {
            HashSet<Potion> uniquePotions = new HashSet<Potion>(_matchedResults.connectedPotions);
            foreach (Potion pot in _matchedResults.connectedPotions)
            {
                List<Potion> extraConnectedPotions = new();

                CheckDirection(pot, new Vector2Int(1, 0), extraConnectedPotions);
                CheckDirection(pot, new Vector2Int(-1, 0), extraConnectedPotions);

                if (extraConnectedPotions.Count >= 2)
                {
                    uniquePotions.UnionWith(extraConnectedPotions);
                    return new MatchResult
                    {
                        connectedPotions = new List<Potion>(uniquePotions),
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedPotions = new List<Potion>(uniquePotions),
                direction = _matchedResults.direction
            };
        }
        return new MatchResult
        {
            connectedPotions = _matchedResults.connectedPotions,
            direction = _matchedResults.direction
        };
    }

    MatchResult IsConnected(Potion potion)
    {
        List<Potion> connectedPotions = new();
        ItemType potionType = potion.potionType;

        connectedPotions.Add(potion);

        CheckDirection(potion, new Vector2Int(1, 0), connectedPotions);
        CheckDirection(potion, new Vector2Int(-1, 0), connectedPotions);
        if (connectedPotions.Count == 3)
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedPotions.Count > 3)
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.LongHorizontal
            };
        }
        connectedPotions.Clear();
        connectedPotions.Add(potion);

        CheckDirection(potion, new Vector2Int(0, 1), connectedPotions);
        CheckDirection(potion, new Vector2Int(0, -1), connectedPotions);
        if (connectedPotions.Count == 3)
        {
            return new MatchResult
            {
                connectedPotions = connectedPotions,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedPotions.Count > 3)
        {
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

    void CheckDirection(Potion pot, Vector2Int misery, List<Potion> connectedPotions)
    {
        ItemType potionType = pot.potionType;
        int x = pot.xIndex + misery.x;
        int y = pot.yIndex + misery.y;

        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (potionBoard[x, y].isUsable && potionBoard[x, y].potion != null)
            {
                Potion neighbourPotion = potionBoard[x, y].potion.GetComponent<Potion>();

                if (!neighbourPotion.isMatched && neighbourPotion.potionType == potionType)
                {
                    connectedPotions.Add(neighbourPotion);
                    x += misery.x;
                    y += misery.y;
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

    private void SelectPotion(Potion _potion)
    {
        if (_potion == null) return;

        if (selectedPotion == null)
        {
            selectedPotion = _potion;
        }
        else if (selectedPotion == _potion)
        {
            selectedPotion = null;
        }
        else
        {
            SwapPotion(selectedPotion, _potion);
            selectedPotion = null;
        }
    }

    private void SwapPotion(Potion _currentPotion, Potion _targetPotion)
    {
        if (!IsAdjacent(_currentPotion, _targetPotion))
        {
            return;
        }

        AudioManager.Instance.PlaySFX("Swipe");

        lastMoveTime = Time.time;
        selectedMatch = null;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            isShaking = false;
            shakeCoroutine = null;
        }

        ClearAllGlowEffects();
        DoSwap(_currentPotion, _targetPotion);
        StartCoroutine(ProcessMatches(_currentPotion, _targetPotion));
    }

    private void DoSwap(Potion _currentPotion, Potion _targetPotion)
    {
        GameObject temp = potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion;

        potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion = potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion;
        potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion = temp;

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

        Debug.Log("ProcessMatches: Checking board after swap");
        processedPotions.Clear();

        _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
        _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);

        while (_currentPotion.isMoving || _targetPotion.isMoving)
            yield return null;

        bool hasMatch = CheckBoard(true);

        if (!hasMatch)
        {
            DoSwap(_currentPotion, _targetPotion);
            _currentPotion.MoveToTarget(potionBoard[_targetPotion.xIndex, _targetPotion.yIndex].potion.transform.position);
            _targetPotion.MoveToTarget(potionBoard[_currentPotion.xIndex, _currentPotion.yIndex].potion.transform.position);
            while (_currentPotion.isMoving || _targetPotion.isMoving)
                yield return null;
            isProcessingMove = false;
            yield break;
        }

        yield return StartCoroutine(ResolveCascadingMatches());

        yield return StartCoroutine(TryShuffleWithDelay());

        isProcessingMove = false;
    }

    private IEnumerator ResolveCascadingMatches()
    {
        bool hasMoreMatches = true;
        cascadeComboCount = 0;

        while (hasMoreMatches)
        {
            Debug.Log("ResolveCascadingMatches: Checking for cascading matches");
            hasMoreMatches = CheckBoard(false);
            if (hasMoreMatches)
            {
                cascadeComboCount++;
                Debug.Log($"Cascading match #{cascadeComboCount} detected");

                CheckBoard(true);
                yield return new WaitUntil(() => AreAllPotionsSettled());

                if (cascadeComboCount >= 2)
                {
                    int randomSpriteIndex = Random.Range(1, 4);
                    yield return StartCoroutine(ShowComboMessage(comboMessageSprites[randomSpriteIndex]));
                }
            }
        }
        processedPotions.Clear();
    }

    private IEnumerator TryShuffleWithDelay()
    {
        yield return new WaitUntil(() => AreAllPotionsSettled());

        if (!HasPossibleMoves() && !isTutorialLevel)
        {
            int maxAttempts = 100;
            int attempts = 0;

            do
            {
                ShuffleBoard();
                yield return StartCoroutine(ShowComboMessage(comboMessageSprites[0])); // Exibir sprite de Reshuffle
                yield return new WaitUntil(() => AreAllPotionsSettled());
                attempts++;
            } while (!HasPossibleMoves() && attempts < maxAttempts);
        }
    }

    private IEnumerator ShowComboMessage(Sprite messageSprite)
    {
        if (comboMessagePrefab == null || messageSprite == null)
        {
            Debug.LogWarning("Combo message prefab ou sprite não configurado!");
            yield break;
        }

        GameObject messageGO = Instantiate(comboMessagePrefab, new Vector3(0f, 0f, -1f), Quaternion.identity);
        SpriteRenderer spriteRenderer = messageGO.GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Prefab deve conter SpriteRenderer!");
            Destroy(messageGO);
            yield break;
        }

        messageGO.transform.SetParent(potionParent.transform, false);

        spriteRenderer.sprite = messageSprite;
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        messageGO.transform.localScale = Vector3.one * (0.5f / 3f);

        float fadeInDuration = 0.3f;
        float scaleDuration = 0.5f;
        float displayDuration = 1f;
        float fadeOutDuration = 0.3f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(spriteRenderer.DOFade(1f, fadeInDuration));
        sequence.Join(messageGO.transform.DOScale(0.3334f, scaleDuration).SetEase(Ease.OutBack));
        sequence.AppendInterval(displayDuration);
        sequence.Append(spriteRenderer.DOFade(0f, fadeOutDuration));
        sequence.Join(messageGO.transform.DOScale(0.1667f, fadeOutDuration));

        yield return sequence.WaitForCompletion();

        Destroy(messageGO);
    }

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

    private IEnumerator AnimatePotionToUI(List<(Potion potion, ItemType type)> potionsToAnimate, Dictionary<ItemType, int> potionsToReduce)
    {
        isAnimatingPotions = true;
        Dictionary<ItemType, List<Potion>> potionsByType = new Dictionary<ItemType, List<Potion>>();
        foreach (var (potion, type) in potionsToAnimate)
        {
            if (!potionsByType.ContainsKey(type))
            {
                potionsByType[type] = new List<Potion>();
            }
            potionsByType[type].Add(potion);
        }

        foreach (var kvp in potionsByType)
        {
            ItemType type = kvp.Key;
            List<Potion> potions = kvp.Value;

            Vector3 centerPoint = Vector3.zero;
            int validPotionCount = 0;
            foreach (Potion potion in potions)
            {
                if (potion != null && potion.gameObject != null)
                {
                    centerPoint += potion.transform.position;
                    validPotionCount++;
                }
            }
            if (validPotionCount > 0)
            {
                centerPoint /= validPotionCount;
            }
            else
            {
                continue;
            }

            GameObject targetUI = null;
            switch (type)
            {
                case ItemType.Violet:
                    targetUI = violetCountUI;
                    break;
                case ItemType.Green:
                    targetUI = greenCountUI;
                    break;
                case ItemType.Red:
                    targetUI = redCountUI;
                    break;
                case ItemType.Orange:
                    targetUI = orangeCountUI;
                    break;
                case ItemType.Blue:
                    targetUI = blueCountUI;
                    break;
            }

            if (targetUI == null)
            {
                continue;
            }

            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(targetUI.transform.position);
            targetPosition.z = centerPoint.z;

            List<GameObject> potionClones = new List<GameObject>();
            List<Vector3> startPositions = new List<Vector3>();

            for (int i = 0; i < potions.Count; i++)
            {
                Potion potion = potions[i];
                if (potion == null || potion.gameObject == null)
                {
                    continue;
                }

                GameObject potionClone = Instantiate(potion.gameObject, potion.transform.position, Quaternion.identity);
                potionClone.transform.SetParent(potionParent.transform);
                potionClone.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                SpriteRenderer cloneRenderer = potionClone.GetComponent<SpriteRenderer>();
                if (cloneRenderer != null)
                {
                    cloneRenderer.sortingOrder = 4;
                }

                Potion clonePotion = potionClone.GetComponent<Potion>();
                if (clonePotion != null)
                {
                    clonePotion.enabled = false;
                    Collider2D collider = potionClone.GetComponent<Collider2D>();
                    if (collider != null) collider.enabled = false;
                }

                potionClones.Add(potionClone);
                startPositions.Add(potionClone.transform.position);
            }

            float convergeDuration = 0.2f;
            float elapsedConverge = 0f;
            while (elapsedConverge < convergeDuration)
            {
                elapsedConverge += Time.deltaTime;
                float t = elapsedConverge / convergeDuration;
                for (int i = 0; i < potionClones.Count; i++)
                {
                    if (potionClones[i] != null)
                    {
                        potionClones[i].transform.position = Vector3.Lerp(startPositions[i], centerPoint, t);
                    }
                }
                yield return null;
            }

            for (int i = 0; i < potionClones.Count; i++)
            {
                if (potionClones[i] != null)
                {
                    potionClones[i].transform.position = centerPoint;
                    startPositions[i] = centerPoint;
                }
            }

            float duration = 0.8f;
            float height = 2f;
            float delayBetweenPotions = 0.08f;

            List<Coroutine> animationCoroutines = new List<Coroutine>();

            for (int i = 0; i < potionClones.Count; i++)
            {
                if (potionClones[i] == null) continue;

                Coroutine animationCoroutine = StartCoroutine(AnimateSinglePotion(
                    potionClones[i],
                    startPositions[i],
                    targetPosition,
                    duration,
                    height + i * 0.2f,
                    type,
                    i + 1,
                    potionsToReduce
                ));
                animationCoroutines.Add(animationCoroutine);

                if (i < potionClones.Count - 1)
                {
                    yield return new WaitForSeconds(delayBetweenPotions);
                }
            }

            foreach (var coroutine in animationCoroutines)
            {
                yield return coroutine;
            }

            UpdateUIVisibility();
            RepositionCountUIs();
        }

        // Verificação de vitória e derrota após todas as animações
        bool isVictory = violetPotionCount.count + greenPotionCount.count + orangePotionCount.count +
                         redPotionCount.count + bluePotionCount.count <= 0 && !won;
        if (isVictory)
        {
            if (!win)
            {
                win = true;
                GameObject targetPlayer = GameObject.FindWithTag("Player");
                PlayerManager playerManager = targetPlayer.GetComponent<PlayerManager>();
                playerManager.AddMoney(10 + (int)timer.GetMovesLeft() * 5);
                level.PassLevel();
            }
            canLose.value = false;
            won = true;
            WinGame.Raise();
        }
        else if (!won && !hasLost && timer.GetMovesLeft() <= 0)
        {
            Debug.Log("Checking lose condition after animations");
            CheckLoseCondition();
        }

        isAnimatingPotions = false;
    }

    private IEnumerator AnimateSinglePotion(GameObject clone, Vector3 startPosition, Vector3 targetPosition, float duration, float height, ItemType type, int potionIndex, Dictionary<ItemType, int> potionsToReduce)
    {
        float elapsedTime = 0f;
        float initialScale = 0.8f; // Escala inicial da poção

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Posição com trajetória parabólica
            float x = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float y = Mathf.Lerp(startPosition.y, targetPosition.y, t);
            float parabola = height * Mathf.Sin(t * Mathf.PI);
            y += parabola;

            // Escala diminuindo gradualmente
            float scale = Mathf.Lerp(initialScale, finalPotionScale, t);
            clone.transform.localScale = new Vector3(scale, scale, scale);

            clone.transform.position = new Vector3(x, y, startPosition.z);

            yield return null;
        }

        // Posição e escala finais
        clone.transform.position = targetPosition;
        clone.transform.localScale = new Vector3(finalPotionScale, finalPotionScale, finalPotionScale);

        AudioManager.Instance.PlaySFX("Collect");

        // Redução da contagem de objetivos
        if (potionsToReduce.ContainsKey(type) && potionsToReduce[type] > 0)
        {
            switch (type)
            {
                case ItemType.Violet:
                    violetPotionCount.count = Mathf.Max(0, violetPotionCount.count - 1);
                    Debug.Log($"Reducing Violet count to {violetPotionCount.count} for potion {potionIndex} in AnimateSinglePotion");
                    break;
                case ItemType.Green:
                    greenPotionCount.count = Mathf.Max(0, greenPotionCount.count - 1);
                    Debug.Log($"Reducing Green count to {greenPotionCount.count} for potion {potionIndex} in AnimateSinglePotion");
                    break;
                case ItemType.Red:
                    redPotionCount.count = Mathf.Max(0, redPotionCount.count - 1);
                    Debug.Log($"Reducing Red count to {redPotionCount.count} for potion {potionIndex} in AnimateSinglePotion");
                    break;
                case ItemType.Orange:
                    orangePotionCount.count = Mathf.Max(0, orangePotionCount.count - 1);
                    Debug.Log($"Reducing Orange count to {orangePotionCount.count} for potion {potionIndex} in AnimateSinglePotion");
                    break;
                case ItemType.Blue:
                    bluePotionCount.count = Mathf.Max(0, bluePotionCount.count - 1);
                    Debug.Log($"Reducing Blue count to {bluePotionCount.count} for potion {potionIndex} in AnimateSinglePotion");
                    break;
            }
            potionsToReduce[type]--;
        }

        Destroy(clone);
    }

    private void ClearAllGlowEffects()
    {
        foreach (var ps in activeGlowEffects)
        {
            if (ps != null && ps.gameObject != null)
            {
                Destroy(ps.gameObject);
            }
        }
        activeGlowEffects.Clear();
    }

    private IEnumerator ShakePossibleMatch()
    {
        if (isShaking)
        {
            Debug.Log($"ShakePossibleMatch já está rodando, ignorando nova chamada (Time: {Time.time})");
            yield break;
        }

        isShaking = true;
        Debug.Log($"Iniciando ShakePossibleMatch (Time: {Time.time})");

        if (selectedMatch.HasValue)
        {
            var match = selectedMatch.Value;
            if (match.matchPotions == null || match.matchPotions.Any(p => p == null || p.gameObject == null))
            {
                Debug.Log("Selected match contém poções inválidas. Resetando selectedMatch.");
                selectedMatch = null;
            }
            else
            {
                bool isValid = true;
                foreach (var potion in match.matchPotions)
                {
                    if (potion.xIndex < 0 || potion.xIndex >= width || potion.yIndex < 0 || potion.yIndex >= height ||
                        potionBoard[potion.xIndex, potion.yIndex].potion != potion.gameObject)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid)
                {
                    Debug.Log("Selected match não está mais válido. Resetando selectedMatch.");
                    selectedMatch = null;
                }
            }
        }

        if (selectedMatch == null && !isTutorialLevel)
        {
            if (!HasPossibleMoves())
            {
                Debug.Log("No possible moves available for animation.");
                isShaking = false;
                shakeCoroutine = null;
                yield break;
            }

            int randomIndex = Random.Range(0, possibleMatches.Count);
            selectedMatch = possibleMatches[randomIndex];
        }

        List<Potion> matchPotions = selectedMatch.Value.matchPotions;

        if (matchPotions == null || matchPotions.Any(p => p == null || p.gameObject == null))
        {
            Debug.LogWarning("One or more match potions are null or destroyed. Resetting selectedMatch.");
            selectedMatch = null;
            isShaking = false;
            shakeCoroutine = null;
            yield break;
        }

        Debug.Log($"Animating {matchPotions.Count} potions for match at ({selectedMatch.Value.x1}, {selectedMatch.Value.y1}) to ({selectedMatch.Value.x2}, {selectedMatch.Value.y2}) (Time: {Time.time})");

        Dictionary<Potion, Vector3> originalPositions = new Dictionary<Potion, Vector3>();
        Dictionary<Potion, Vector3> originalScales = new Dictionary<Potion, Vector3>();

        foreach (Potion potion in matchPotions)
        {
            if (potion != null && potion.gameObject != null)
            {
                originalPositions[potion] = potion.transform.position;
                originalScales[potion] = new Vector3(0.8f, 0.8f, 0.8f);

                if (glowVFXPrefab != null)
                {
                    GameObject vfx = Instantiate(glowVFXPrefab, potion.transform.position, Quaternion.identity, potion.transform);
                    ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
                    if (ps != null)
                    {
                        var main = ps.main;
                        main.startColor = potionColors[potion.potionType];
                        ParticleSystemRenderer renderer = vfx.GetComponent<ParticleSystemRenderer>();
                        if (renderer != null)
                        {
                            renderer.sortingOrder = 2;
                        }
                        activeGlowEffects.Add(ps);
                    }
                }
            }
        }

        float bounceFrequency = 1.2f;
        float scaleMagnitude = 0.05f;
        float heightMagnitude = 0.07f;
        float fadeInDuration = 0.2f;
        float elapsed = 0f;
        bool fadeInCompleted = false;

        while (isShaking)
        {
            elapsed += Time.deltaTime;

            float fadeInFactor = fadeInCompleted ? 1f : Mathf.Clamp01(elapsed / fadeInDuration);
            if (elapsed >= fadeInDuration)
            {
                fadeInCompleted = true;
            }

            foreach (Potion potion in matchPotions)
            {
                if (potion != null && potion.gameObject != null)
                {
                    float bounce = Mathf.Abs(Mathf.Sin(Time.time * bounceFrequency * Mathf.PI)) * heightMagnitude;
                    float scale = 0.8f + Mathf.Abs(Mathf.Sin(Time.time * bounceFrequency * Mathf.PI)) * scaleMagnitude;

                    bounce *= fadeInFactor;
                    scale = 0.8f + (scale - 0.8f) * fadeInFactor;

                    Vector3 originalPos = originalPositions[potion];
                    potion.transform.position = new Vector3(originalPos.x, originalPos.y + bounce, originalPos.z);
                    potion.transform.localScale = new Vector3(scale, scale, scale);
                }
            }

            foreach (var ps in activeGlowEffects)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTimeMultiplier = 10f * fadeInFactor;
                }
            }

            yield return null;
        }

        float fadeOutDuration = 0.2f;
        float fadeOutElapsed = 0f;
        while (fadeOutElapsed < fadeOutDuration)
        {
            fadeOutElapsed += Time.deltaTime;
            float t = fadeOutElapsed / fadeOutDuration;

            foreach (Potion potion in matchPotions)
            {
                if (potion != null && potion.gameObject != null)
                {
                    potion.transform.position = Vector3.Lerp(potion.transform.position, originalPositions[potion], t);
                    potion.transform.localScale = Vector3.Lerp(potion.transform.localScale, originalScales[potion], t);
                }
            }

            foreach (var ps in activeGlowEffects)
            {
                if (ps != null)
                {
                    var emission = ps.emission;
                    emission.rateOverTimeMultiplier = 10f * (1f - t);
                }
            }

            yield return null;
        }

        foreach (Potion potion in matchPotions)
        {
            if (potion != null && potion.gameObject != null)
            {
                potion.transform.position = originalPositions[potion];
                potion.transform.localScale = originalScales[potion];
            }
        }

        ClearAllGlowEffects();

        Debug.Log($"Finalizando ShakePossibleMatch (Time: {Time.time})");
        isShaking = false;
        shakeCoroutine = null;
    }

    public void WinGameBool(bool state)
    {
        won = state;
    }

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

