using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;

    public Text playerPuntuacion;
    public Text dealerPuntuacion;
    public TMP_Dropdown apostarDropdown;
    public Text credito;

    //Probabilidades
    public Text probMore;
    public Text probGood;
    public Text probDealer;

    //Enteros de credito
    int creditoBanco = 1000;
    int creditoApostado;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {
        inhabilitarBotonesInteraccion();
        InitCardValues();   
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            int valor;
            switch (i % 13)
            {
                case 0: // As
                    valor = 11;
                    break;
                case 10: // J
                case 11: // Q
                case 12: // K
                    valor = 10;
                    break;
                default:
                    valor = i % 13 + 1;
                    break;
            }
            values[i] = valor;
        }
    }

    private void ShuffleCards()
    {
        Sprite tempFace;
        int tempValue;

        for (int i = 51; i >= 0; i--)
        {
            int k = UnityEngine.Random.Range(0,52);
            // Intercambiamos la carta en la posición k con la carta en la posición n
            tempFace = faces[k];
            faces[k] = faces[i];
            faces[i] = tempFace;

            tempValue = values[k];
            values[k] = values[i];
            values[i] = tempValue;
        }
    }


    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }
        CalculateProbabilities();
        ApostarCreditos();
        playerPuntuacion.text = player.GetComponent<CardHand>().points.ToString();
        if (player.GetComponent<CardHand>().points != 21 && dealer.GetComponent<CardHand>().points != 21)
        {
            habilitarBotonesInteraccion();
            return;
        }
        
        string quien = "Dealer";

        if (player.GetComponent<CardHand>().points == 21){
            quien = "Jugador";
            ganarPartidaCredito();
        }
            finalMessage.text = "El " + quien+ " ha hecho Blackjack, gana la partida";
            inhabilitarBotonesInteraccion();
            mostrarDealer();
    }

    private void CalculateProbabilities()
    {
        CardHand playerHand = player.GetComponent<CardHand>();
        CardHand dealerHand = dealer.GetComponent<CardHand>();
        int playerScore = playerHand.points;
        int dealerScore = dealerHand.points;

        // Calcular la probabilidad de que el crupier tenga una puntuación mayor que la del jugador
        // Esto implica tener en cuenta la carta oculta del crupier y las cartas del jugador
        float probDealerWin = 0f;
        if (playerScore != 21)
        {
            int dealerHiddenCardValue = dealerHand.cards[0].GetComponent<CardModel>().value;

            int dealerMaxScore = dealerScore + (dealerHiddenCardValue == 11 ? 10 : 0); // Asumiendo que la carta oculta es un As

            int remainingCards = 52 - cardIndex;
            int cardsHigherThanPlayer = 0;

            for (int i = cardIndex; i < faces.Length; i++)
            {
                int cardValue = values[i] == 11 ? 10 : values[i]; // Si es un As, tomamos el valor 10
                if (dealerMaxScore + cardValue > playerScore && dealerMaxScore + cardValue <= 21)
                {
                    cardsHigherThanPlayer++;
                }
            }

            probDealerWin = (float)cardsHigherThanPlayer / remainingCards;
        }

        // Calcular la probabilidad de que el jugador obtenga una puntuación entre 17 y 21 si pide una carta adicional
        float probPlayerGood = 0f;
        if (playerScore < 17)
        {
            int cardsInRange = 0;

            for (int i = cardIndex; i < faces.Length; i++)
            {
                int cardValue = values[i] == 11 ? 10 : values[i]; // Si es un As, tomamos el valor 10
                if (playerScore + cardValue >= 17 && playerScore + cardValue <= 21)
                {
                    cardsInRange++;
                }
            }

            probPlayerGood = (float)cardsInRange / (52 - cardIndex);
        }

        // Calcular la probabilidad de que el jugador supere los 21 puntos si pide una carta adicional
        float probPlayerBust = 0f;
        if (playerScore < 21)
        {
            int cardsBust = 0;

            for (int i = cardIndex; i < faces.Length; i++)
            {
                int cardValue = values[i] == 11 ? 10 : values[i]; // Si es un As, tomamos el valor 10
                if (playerScore + cardValue > 21)
                {
                    cardsBust++;
                }
            }

            probPlayerBust = (float)cardsBust / (52 - cardIndex);
        }

        // Ajustar la normalización para que las probabilidades no se reduzcan a 0 demasiado pronto
        float minProb = 0.05f; // Valor mínimo para cualquier probabilidad

        probDealerWin = Mathf.Max(probDealerWin, minProb);
        probPlayerGood = Mathf.Max(probPlayerGood, minProb);
        probPlayerBust = Mathf.Max(probPlayerBust, minProb);

        // Normalizar las probabilidades para asegurar que sumen 1
        float totalProb = probDealerWin + probPlayerGood + probPlayerBust;

        probDealerWin /= totalProb;
        probPlayerGood /= totalProb;
        probPlayerBust /= totalProb;
        UpdateProbabilityUI(probDealerWin, probPlayerGood, probPlayerBust);
    }

    private void UpdateProbabilityUI(float probDealerWin, float probPlayerGood, float probPlayerBust)
    {
        // Actualizar los textos de las probabilidades en la interfaz de usuario con los valores calculados
        probDealer.text = (probDealerWin * 100).ToString("F2") + "%";
        probGood.text = (probPlayerGood * 100).ToString("F2") + "%";
        probMore.text = (probPlayerBust * 100).ToString("F2") + "%";
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
    }       

    public void Hit()
    {
        PushPlayer();
        CalculateProbabilities();
        int playerPoints = player.GetComponent<CardHand>().points;
        if (playerPoints > 21)
        {
            finalMessage.text = "El Jugador ha perdido, gana el Dealer";       
            inhabilitarBotonesInteraccion();
            mostrarDealer();
        }
        if (playerPoints == 21)
        {
            finalMessage.text = "El Jugador ha hecho Blackjack, gana la partida";
            ganarPartidaCredito();
            inhabilitarBotonesInteraccion();
            mostrarDealer();
        }
        playerPuntuacion.text = playerPoints.ToString();
    }

    public void Stand()
    {
        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
        }
        mostrarDealer();

        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "El Dealer ha perdido, gana el Jugador";
            ganarPartidaCredito();
        }
        else
        {
            int playerScore = player.GetComponent<CardHand>().points;
            int dealerScore = dealer.GetComponent<CardHand>().points;

            if (playerScore > dealerScore)
            {
                finalMessage.text = "El Jugador gana la partida";
                ganarPartidaCredito();
            }
            else if (playerScore < dealerScore)
            {
                finalMessage.text = "El Dealer gana la partida";
            }
            else
            {
                finalMessage.text = "La partida termina en empate";
                empatarPartidaCredito();
            }
        }
        inhabilitarBotonesInteraccion();
    }


    public void PlayAgain()
    {
        habilitarBotonesInteraccion();
        finalMessage.text = "";
        playerPuntuacion.text = "";
        dealerPuntuacion.text = "";

        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
    public void ApostarCreditos()
    {
        if (creditoBanco > 0)
        {
            string labelText = GameObject.Find("Credits").transform.Find("Label").GetComponent<TextMeshProUGUI>().text;
            creditoApostado = int.Parse(labelText.Substring(0, 3));
            if(creditoApostado > creditoBanco)
            {
                creditoApostado = creditoBanco;
            }
            creditoBanco -= creditoApostado;
            credito.text = creditoBanco.ToString();
        }
        else{
            creditoApostado = 0;
        }
    }

    private void inhabilitarBotonesInteraccion()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        playAgainButton.interactable = true;
        apostarDropdown.interactable = true;
    }
    private void habilitarBotonesInteraccion()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        playAgainButton.interactable = false;
        apostarDropdown.interactable = false;
    }
    private void mostrarDealer()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        dealerPuntuacion.text = dealer.GetComponent<CardHand>().points.ToString();
    }
    private void ganarPartidaCredito()
    {
        creditoBanco += 2 * creditoApostado;
        credito.text = creditoBanco.ToString();
    }
    private void empatarPartidaCredito()
    {
        creditoBanco += creditoApostado;
        credito.text = creditoBanco.ToString();
    }
}