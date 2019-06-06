using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip music_win_clip;
    public AudioClip music_loss_clip;
    public AudioClip draw_card_clip;
    public AudioClip throw_card_clip;
    public AudioClip uno_btn_clip;
    public AudioClip choose_color_clip;

    [Header("Gameplay")]
    [Range(0, 100)]
    public int LeftRoomProbability = 10;
    [Range(0, 100)]
    public int UnoProbability = 70;
    [Range(0, 100)]
    public int LowercaseNameProbability = 30;

    public float cardDealTime = 0.05f;
    public Card _cardPrefab;
    public Transform cardDeckTransform;
    public Image cardWastePile;
    public GameObject arrowObject, unoBtn, cardDeckBtn;
    public Popup colorChoose, playerChoose, noNetwork;
    public GameObject loadingView, rayCastBlocker;
    public Animator cardEffectAnimator;
    public ParticleSystem wildCardParticle;
    public GameObject menuButton;

    [Header("Player Setting")]
    public List<Player> players;
    public TextAsset multiplayerNames;
    public TextAsset computerProfiles;
    public bool clockwiseTurn = true;
    public int currentPlayerIndex = 0;
    public Player CurrentPlayer { get { return players[currentPlayerIndex]; } }

    [Header("Game Over")]
    public GameObject gameOverPopup;
    public ParticleSystem starParticle;
    public List<GameObject> playerObject;
    public GameObject loseTimerAnimation;

    private List<Card> cards;
    private List<Card> wasteCards;

    public CardType CurrentType
    {
        get { return _currentType; }
        set { _currentType = value; cardWastePile.color = value.GetColor(); }
    }

    public CardValue CurrentValue
    {
        get { return _currentValue; }
        set { _currentValue = value; }
    }

    [SerializeField] CardType _currentType;
    [SerializeField] CardValue _currentValue;

    public bool IsDeckArrow
    {
        get { return arrowObject.activeSelf; }
    }
    public static GamePlayManager instance;

    System.DateTime pauseTime;
    int fastForwardTime = 0;
    bool setup = false, multiplayerLoaded = false, gameOver = false;

    void Start()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        if (GameManager.currentGameMode == GameMode.Computer)
        {
            SetTotalPlayer(4);
            SetupGame();
        }
        else
        {
            StartCoroutine(CheckNetwork());
            playerChoose.ShowPopup();
        }
    }

    public void OnPlayerSelect(ToggleGroup group)
    {
        playerChoose.HidePopup(false);
        int i = 4;
        foreach (var t in group.ActiveToggles())
        {
            i = int.Parse(t.name);
        }
        StartCoroutine(StartMultiPlayerGameMode(i));
        GameManager.PlayButton();
    }

    IEnumerator StartMultiPlayerGameMode(int i)
    {
        loadingView.SetActive(true);
        yield return new WaitForSeconds(Random.Range(3f, 10f));
        loadingView.SetActive(false);
        SetTotalPlayer(i);
        SetupGame();

        bool leftRoom = Random.Range(0, 100) <= LeftRoomProbability && players.Count > 2;
        if (leftRoom)
        {
            float inTime = Random.Range(7, 5 * 60);
            StartCoroutine(RemovePlayerFromRoom(inTime));
        }

        multiplayerLoaded = true;
    }

    void SetTotalPlayer(int totalPlayer = 4)
    {
        cardDeckBtn.SetActive(true);
        cardWastePile.gameObject.SetActive(true);
        unoBtn.SetActive(true);
        if (totalPlayer == 2)
        {
            players[0].gameObject.SetActive(true);
            players[0].CardPanelBG.SetActive(true);
            players[2].gameObject.SetActive(true);
            players[2].CardPanelBG.SetActive(true);
            players.RemoveAt(3);
            players.RemoveAt(1);

        }
        else if (totalPlayer == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                players[i].gameObject.SetActive(true);
                players[i].CardPanelBG.SetActive(true);
            }
            players[3].CardPanelBG.SetActive(true);
            players.RemoveAt(3);

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                players[i].gameObject.SetActive(true);
                players[i].CardPanelBG.SetActive(true);
            }
        }
    }

    void SetupGame()
    {
        menuButton.SetActive(true);
        currentPlayerIndex = Random.Range(0, players.Count);
        players[0].SetAvatarProfile(GameManager.PlayerAvatarProfile);

        if (GameManager.currentGameMode == GameMode.MultiPlayer)
        {
            string[] nameList = multiplayerNames.text.Split('\n');
            List<int> indexes = new List<int>();

            for (int i = 1; i < players.Count; i++)
            {
                while (true)
                {
                    int index = Random.Range(0, nameList.Length);
                    var name = nameList[index].Trim();
                    if (name.Length == 0) continue;

                    if (!indexes.Contains(index))
                    {
                        indexes.Add(index);
                        if (Random.value < LowercaseNameProbability / 100f) name = name.ToLower();
                        players[i].SetAvatarProfile(new AvatarProfile { avatarIndex = index % GameManager.TOTAL_AVATAR, avatarName = name });
                        break;
                    }
                }
            }
        }
        else
        {
            var profiles = JsonUtility.FromJson<AvatarProfiles>(computerProfiles.text).profiles;
            for(int i = 0; i < profiles.Count; i++)
            {
                players[i + 1].SetAvatarProfile(profiles[i]);
            }
        }

        CreateDeck();
        cards.Shuffle();
        StartCoroutine(DealCards(7));
    }

    void CreateDeck()
    {
        cards = new List<Card>();
        wasteCards = new List<Card>();
        for (int j = 1; j <= 4; j++)
        {
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.Wild));
            cards.Add(CreateCardOnDeck(CardType.Other, CardValue.DrawFour));
        }
        for (int i = 0; i <= 12; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
                cards.Add(CreateCardOnDeck((CardType)j, (CardValue)i));
            }
        }
    }

    Card CreateCardOnDeck(CardType t, CardValue v)
    {
        Card temp = Instantiate(_cardPrefab, cardDeckTransform.position, Quaternion.identity, cardDeckTransform);
        temp.Type = t;
        temp.Value = v;
        temp.IsOpen = false;
        temp.CalcPoint();
        temp.name = t.ToString() + "_" + v.ToString();
        return temp;
    }

    IEnumerator DealCards(int total)
    {
        yield return new WaitForSeconds(1f);
        for (int t = 0; t < total; t++)
        {
            for (int i = 0; i < players.Count; i++)
            {
                PickCardFromDeck(players[i]);
                yield return new WaitForSeconds(cardDealTime);
            }
        }

        yield return new WaitForSeconds(1.5f);
        int a = 0;
        while (cards[a].Type == CardType.Other)
        {
            a++;
        }

        PutCardToWastePile(cards[a]);
        cards.RemoveAt(a);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].cardsPanel.UpdatePos();
        }

        setup = true;
        CurrentPlayer.OnTurn();
    }

    IEnumerator DealCardsToPlayer(Player p, int NoOfCard = 1, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        for (int t = 0; t < NoOfCard; t++)
        {
            PickCardFromDeck(p, true);
            yield return new WaitForSeconds(cardDealTime);
        }
    }

    public Card PickCardFromDeck(Player p, bool updatePos = false)
    {
        if (cards.Count == 0)
        {
            print("Card Over");
            while (wasteCards.Count > 5)
            {
                cards.Add(wasteCards[0]);
                wasteCards[0].transform.SetParent(cardDeckTransform);
                wasteCards[0].transform.localPosition = Vector3.zero;
                wasteCards[0].transform.localRotation = Quaternion.Euler(Vector3.zero);
                wasteCards[0].IsOpen = false;
                wasteCards.RemoveAt(0);
            }
        }
        Card temp = cards[0];
        p.AddCard(cards[0]);
        cards[0].IsOpen = p.isUserPlayer;
        if (updatePos)
            p.cardsPanel.UpdatePos();
        else
            cards[0].SetTargetPosAndRot(Vector3.zero, 0f);
        cards.RemoveAt(0);
        GameManager.PlaySound(throw_card_clip);
        return temp;

    }

    public void PutCardToWastePile(Card c, Player p = null)
    {
        if (p != null)
        {
            p.RemoveCard(c);
            if (p.cardsPanel.cards.Count == 1 && !p.unoClicked)
            {
                ApplyUnoCharge(CurrentPlayer);
            }
            GameManager.PlaySound(draw_card_clip);
        }

        CurrentType = c.Type;
        CurrentValue = c.Value;
        wasteCards.Add(c);
        c.IsOpen = true;
        c.transform.SetParent(cardWastePile.transform, true);
        c.SetTargetPosAndRot(new Vector3(Random.Range(-15f, 15f), Random.Range(-15f, 15f), 1), c.transform.localRotation.eulerAngles.z + Random.Range(-15f, 15f));

        if (p != null)
        {
            if (p.cardsPanel.cards.Count == 0)
            {
                Invoke("SetupGameOver", 2f);
                return;
            }
            if (c.Type == CardType.Other)
            {
                CurrentPlayer.Timer = true;
                CurrentPlayer.choosingColor = true;
                if (CurrentPlayer.isUserPlayer)
                {
                    colorChoose.ShowPopup();
                }
                else
                {
                    Invoke("ChooseColorforAI", Random.Range(3f, 9f));
                }
            }
            else
            {
                if (c.Value == CardValue.Reverse)
                {
                    clockwiseTurn = !clockwiseTurn;
                    cardEffectAnimator.Play(clockwiseTurn ? "ClockWiseAnim" : "AntiClockWiseAnim");
                    Invoke("NextPlayerTurn", 1.5f);
                }
                else if (c.Value == CardValue.Skip)
                {
                    NextPlayerIndex();
                    CurrentPlayer.ShowMessage("Turn Skipped!");
                    Invoke("NextPlayerTurn", 1.5f);
                }
                else if (c.Value == CardValue.DrawTwo)
                {
                    NextPlayerIndex();
                    CurrentPlayer.ShowMessage("+2");
                    wildCardParticle.Emit(30);
                    StartCoroutine(DealCardsToPlayer(CurrentPlayer, 2, .5f));
                    Invoke("NextPlayerTurn", 1.5f);
                }
                else
                {
                    NextPlayerTurn();
                }
            }
        }
    }

    private IEnumerator RemovePlayerFromRoom(float time)
    {
        yield return new WaitForSeconds(time);

        if (gameOver) yield break;

        List<int> indexes = new List<int>();
        for(int i = 1; i < players.Count; i++)
        {
            indexes.Add(i);
        }
        indexes.Shuffle();

        int index = -1;
        foreach (var i in indexes)
        {
            if (!players[i].Timer)
            {
                index = i;
                break;
            }
        }

        var player = players[index];
        player.isInRoom = false;

        Toast.instance.ShowMessage(player.playerName + " left the room", 2.5f);

        yield return new WaitForSeconds(2f);

        player.gameObject.SetActive(false);
        foreach (var item in player.cardsPanel.cards)
        {
            item.gameObject.SetActive(false);
        }
    }

    void ChooseColorforAI()
    {
        CurrentPlayer.ChooseBestColor();
    }

    public void NextPlayerIndex()
    {
        int step = clockwiseTurn ? 1 : -1;
        do
        {
            currentPlayerIndex = Mod(currentPlayerIndex + step, players.Count);
        } while (!players[currentPlayerIndex].isInRoom);
    }

    private int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void NextPlayerTurn()
    {
        NextPlayerIndex();
        CurrentPlayer.OnTurn();
    }

    public void OnColorSelect(int i)
    {
        if (!colorChoose.isOpen) return;
        colorChoose.HidePopup();

        SelectColor(i);
    }

    public void SelectColor(int i)
    {
        CurrentPlayer.Timer = false;
        CurrentPlayer.choosingColor = false;

        CurrentType = (CardType)i;
        cardEffectAnimator.Play("DrawFourAnim");
        if (CurrentValue == CardValue.Wild)
        {
            wildCardParticle.gameObject.SetActive(true);
            wildCardParticle.Emit(30);
            Invoke("NextPlayerTurn", 1.5f);
            GameManager.PlaySound(choose_color_clip);
        }
        else
        {
            NextPlayerIndex();
            CurrentPlayer.ShowMessage("+4");
            StartCoroutine(DealCardsToPlayer(CurrentPlayer, 4, .5f));
            Invoke("NextPlayerTurn", 2f);
            GameManager.PlaySound(choose_color_clip);
        }
    }

    public void EnableDeckClick()
    {
        arrowObject.SetActive(true);
    }

    public void OnDeckClick()
    {
        if (!setup) return;

        if (arrowObject.activeInHierarchy)
        {

            arrowObject.SetActive(false);
            CurrentPlayer.pickFromDeck = true;
            PickCardFromDeck(CurrentPlayer, true);
            if (CurrentPlayer.cardsPanel.AllowedCard.Count == 0 || (!CurrentPlayer.Timer && CurrentPlayer.isUserPlayer))
            {
                CurrentPlayer.OnTurnEnd();
                NextPlayerTurn();
            }
            else
            {
                CurrentPlayer.UpdateCardColor();
            }
        }
        else if (!CurrentPlayer.pickFromDeck && CurrentPlayer.isUserPlayer)
        {
            PickCardFromDeck(CurrentPlayer, true);
            CurrentPlayer.pickFromDeck = true;
            CurrentPlayer.UpdateCardColor();
        }
    }

    public void EnableUnoBtn()
    {
        unoBtn.GetComponent<Button>().interactable = true;
    }

    public void DisableUnoBtn()
    {
        unoBtn.GetComponent<Button>().interactable = false;
    }

    public void OnUnoClick()
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Uno!", true);
        CurrentPlayer.unoClicked = true;
        GameManager.PlaySound(uno_btn_clip);
    }

    public void ApplyUnoCharge(Player p)
    {
        DisableUnoBtn();
        CurrentPlayer.ShowMessage("Uno Charges");
        StartCoroutine(DealCardsToPlayer(p, 2, .3f));
    }

    public void SetupGameOver()
    {
        gameOver = true;
        for(int i = players.Count - 1; i >= 0; i--)
        {
            if (!players[i].isInRoom)
            {
                players.RemoveAt(i);
            }
        }

        if (players.Count == 2)
        {
            playerObject[0].SetActive(true);
            playerObject[2].SetActive(true);
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "2nd Place";
            playerObject.RemoveAt(3);
            playerObject.RemoveAt(1);

        }
        else if (players.Count == 3)
        {
            playerObject.RemoveAt(2);
            for (int i = 0; i < 3; i++)
            {
                playerObject[i].SetActive(true);
            }
            playerObject[2].transform.GetChild(2).GetComponent<Text>().text = "3rd Place";

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                playerObject[i].SetActive(true);
            }
        }

        players.Sort((x, y) => x.GetTotalPoints().CompareTo(y.GetTotalPoints()));
        var winner = players[0];

        starParticle.gameObject.SetActive(winner.isUserPlayer);
        playerObject[0].GetComponentsInChildren<Image>()[1].sprite = winner.avatarImage.sprite;

        for (int i = 1; i < playerObject.Count; i++)
        {
            var playerNameText = playerObject[i].GetComponentInChildren<Text>();
            playerNameText.text = players[i].playerName;
            playerNameText.GetComponent<EllipsisText>().UpdateText();
            playerObject[i].GetComponentsInChildren<Image>()[1].sprite = players[i].avatarImage.sprite;
        }

        GameManager.PlaySound(winner.isUserPlayer ? music_win_clip : music_loss_clip);
        gameOverPopup.SetActive(true);

        for(int i = 1; i < players.Count; i++)
        {
            if (players[i].isUserPlayer)
            {
                loseTimerAnimation.SetActive(true);
                loseTimerAnimation.transform.position = playerObject[i].transform.position;
                break;
            }
        }

        gameOverPopup.GetComponent<Animator>().enabled = winner.isUserPlayer;
        gameOverPopup.GetComponentInChildren<Text>().text = winner.isUserPlayer ? "You win Game." : "You Lost Game ...   Try Again.";
        fastForwardTime = 0;
    }

    IEnumerator CheckNetwork()
    {
        while (true)
        {
            WWW www = new WWW("https://www.google.com");
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (noNetwork.isOpen)
                {
                    noNetwork.HidePopup();

                    Time.timeScale = 1;
                    OnApplicationPause(false);
                }
            }
            else
            {
                if (Time.timeScale == 1)
                {
                    noNetwork.ShowPopup();

                    Time.timeScale = 0;
                    pauseTime = System.DateTime.Now;
                }
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            pauseTime = System.DateTime.Now;
        }
        else
        {
            if (GameManager.currentGameMode == GameMode.MultiPlayer && multiplayerLoaded && !gameOver)
            {
                fastForwardTime += Mathf.Clamp((int)(System.DateTime.Now - pauseTime).TotalSeconds, 0, 3600);
                if (Time.timeScale == 1f)
                {
                    StartCoroutine(DoFastForward());
                }
            }
        }
    }

    IEnumerator DoFastForward()
    {
        Time.timeScale = 10f;
        rayCastBlocker.SetActive(true);
        while (fastForwardTime > 0)
        {
            yield return new WaitForSeconds(1f);
            fastForwardTime--;
        }
        Time.timeScale = 1f;
        rayCastBlocker.SetActive(false);

    }
}

[System.Serializable]
public class AvatarProfile
{
    public int avatarIndex;
    public string avatarName;
}

public class AvatarProfiles
{
    public List<AvatarProfile> profiles;
}