using UnityEngine;
using UnityEngine.UI;

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
    public Dropdown apostarDropdown;
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
        playerPuntuacion.text = player.GetComponent<CardHand>().points.ToString();
        if (player.GetComponent<CardHand>().points != 21 && dealer.GetComponent<CardHand>().points != 21)
        {
            habilitarBotonesInteraccion();
            return;
        }
        
        string quien = "Dealer";

        if (player.GetComponent<CardHand>().points == 21){
            quien = "Jugador";
            creditoBanco += 2 * creditoApostado;
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
        // Si el jugador tiene Blackjack, la probabilidad es 0, ya que ganaría automáticamente
        float probDealerWin = 0f;
        if (playerScore != 21)
        {
            // Lógica para calcular la probabilidad
        }

        // Calcular la probabilidad de que el jugador obtenga una puntuación entre 17 y 21 si pide una carta adicional
        // Esto implica tener en cuenta las cartas actuales del jugador y las posibles cartas adicionales
        float probPlayerGood = 0f;
        // Lógica para calcular la probabilidad

        // Calcular la probabilidad de que el jugador supere los 21 puntos si pide una carta adicional
        // Esto implica tener en cuenta las cartas actuales del jugador y las posibles cartas adicionales
        float probPlayerBust = 0f;
        // Lógica para calcular la probabilidad

        // Actualizar los textos de probabilidades en la interfaz de usuario
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
        CalculateProbabilities();
    }       

    public void Hit()
    {
        PushPlayer();

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "El Jugador ha perdido, gana el Dealer";       
            inhabilitarBotonesInteraccion();
            mostrarDealer();
        }
        if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "El Jugador ha hecho Blackjack, gana la partida";
            creditoBanco += 2 * creditoApostado;
            inhabilitarBotonesInteraccion();
            mostrarDealer();
        }
        playerPuntuacion.text = player.GetComponent<CardHand>().points.ToString();
        CalculateProbabilities(); 
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
            creditoBanco += 2 * creditoApostado;
        }
        else
        {
            int playerScore = player.GetComponent<CardHand>().points;
            int dealerScore = dealer.GetComponent<CardHand>().points;

            if (player.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points)
            {
                finalMessage.text = "El Jugador gana la partida";
                creditoBanco += 2 * creditoApostado;
            }
            else if (playerScore < dealerScore)
            {
                finalMessage.text = "El Dealer gana la partida";
            }
            else
            {
                finalMessage.text = "La partida termina en empate";
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

    }

    private void inhabilitarBotonesInteraccion()
    {
        hitButton.interactable = false;
        stickButton.interactable = false;
        playAgainButton.interactable = true;
    }
    private void habilitarBotonesInteraccion()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        playAgainButton.interactable = false;
    }
    private void mostrarDealer()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        dealerPuntuacion.text = dealer.GetComponent<CardHand>().points.ToString();
    }
}
