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

    //Textos para cuando sea Blackjack
    public Text playerBlackjack;
    public Text dealerBlackjack;

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
        hitButton.interactable = false;
        stickButton.interactable = false;
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
        Debug.Log("Valores de las cartas inicializados" + string.Join(", ", values));
    }

    private void ShuffleCards()
    {
        int n = faces.Length;
        Sprite tempFace;
        int tempValue;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            // Intercambiamos la carta en la posición k con la carta en la posición n
            tempFace = faces[k];
            faces[k] = faces[n];
            faces[n] = tempFace;

            tempValue = values[k];
            values[k] = values[n];
            values[n] = tempValue;
        }
        Debug.Log("Valores ordenados" + string.Join(", ", values));
    }


    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */      

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */                
         
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
    public void ApostarCreditos()
    {

    }
}
