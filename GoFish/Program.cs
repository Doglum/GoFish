using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish
{
    public static class Globals
    {
        public static Random numgen = new Random();
    }
    class Card
    {
        public int value { get; set; }
        public int suit { get; set; }
        public string name { get; set; }
        public string nameSansSuit { get; set; }

        public Card(int val, int suitNum)
        {
            value = val;
            suit = suitNum;
            string msg = "";
            switch (value)
            {
                case 1:
                    msg += "A";
                    break;
                case 11:
                    msg += "J";
                    break;
                case 12:
                    msg += "Q";
                    break;
                case 13:
                    msg += "K";
                    break;
                default:
                    msg += value.ToString();
                    break;
            }
            nameSansSuit = msg;

            switch (suit)
            {
                case 1:
                    msg += "♠";
                    break;
                case 2:
                    msg += "♥";
                    break;
                case 3:
                    msg += "♦";
                    break;
                case 4:
                    msg += "♣";
                    break;
                default:
                    msg = "Invalid";
                    break;
            }
            name = msg;
        }
    }
    class Program
    {

        static List<Card> CreateDeck()
        {
            List<Card> deck = new List<Card>();
            for (int suit = 1; suit <= 4; suit++)
            {
                for (int value = 1; value <= 13; value++)
                {
                    deck.Add(new Card(value, suit));
                }
            }
            return deck;
        }

        static List<Card> CreateHand(ref List<Card> deck, int size)
        {
            List<Card> hand = new List<Card>();

            int position = 0;
            for (int i = 0; i < size; i++)
            {
                position = Globals.numgen.Next(0, deck.Count - 1);
                hand.Add(deck[position]);
                deck.Remove(deck[position]);
            }
            return hand;
        }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            List<Card> deck = CreateDeck();
            List<Card> playerHand = CreateHand(ref deck, 7);
            List<Card> computerHand = CreateHand(ref deck, 7);
            List<Card> retrievedCards = new List<Card>();
            List<Card> repeatedCards = new List<Card>();
            List<char> rememberedCards = new List<char>();
            bool gameOver = false;
            bool cpuUsedRememberedCard = false;
            bool deckEmptyMessageDisplayed = false;
            char playerCardRequest = 'b';
            string playerInput = "a";
            Card computerCardRequest = new Card(1, 1); //should never be asked for by CPU before its card selection
            bool playerGotCard = false;
            bool computerGotCard = false;
            bool playerInvalidCard = true;
            int playerSetCount = 0;
            int computerSetCount = 0;
            while (!gameOver)
            {
                //bool resets
                playerGotCard = false;
                computerGotCard = false;
                cpuUsedRememberedCard = false;

                //displays player's info
                playerHand = playerHand.OrderByDescending(o => o.value).ToList();
                Console.WriteLine("Here are your cards:");
                foreach (Card card in playerHand)
                {
                    Console.WriteLine(card.name);
                }
                //asks user what card they want to go after
                playerInvalidCard = true;
                Console.WriteLine("What card would you like to ask for?");
                playerInput = Console.ReadLine().ToUpper();
                while (playerInvalidCard)
                {
                    if (playerInput != "")
                    {
                        playerCardRequest = playerInput[0];
                        foreach (Card card in playerHand)
                        {
                            if (card.name[0] == playerCardRequest)
                            {
                                playerInvalidCard = false;
                                break;
                            }
                        }
                    }

                    if (playerInvalidCard)
                    {
                        Console.WriteLine("You don't have that card, enter again");
                        playerInput = Console.ReadLine().ToUpper();
                    }
                }
                //adds the player's request to remembered cards and removes cards from further memory
                rememberedCards.Add(playerInput[0]);
                if (rememberedCards.Count>4)
                {
                    rememberedCards.Remove(rememberedCards[0]);
                }


                //checks if the requested card is in the computer's hand and removes it if it is
                for (int i = 0; i < computerHand.Count; i++)
                {
                    if (computerHand[i].name[0] == playerCardRequest)
                    {
                        playerGotCard = true;
                        Console.WriteLine("You got " + computerHand[i].name);
                        retrievedCards.Add(computerHand[i]);
                    }
                }
                foreach (Card card in retrievedCards)
                {
                    computerHand.Remove(card);
                    playerHand.Add(card);
                }
                retrievedCards.Clear();

                //if player guesses wrong, get card from deck
                if (!playerGotCard && deck.Count != 0)
                {
                    playerHand.Add(CreateHand(ref deck, 1)[0]);
                    Console.WriteLine("You go fishing and receive " + playerHand[playerHand.Count - 1].name);
                }

                //game over check
                if (playerHand.Count == 0 || computerHand.Count == 0)
                {
                    gameOver = true;
                }

                //AI's turn
                if (!gameOver)
                {
                    //checks if the cpu has a card from the player's hand it can remember:
                    foreach (Card card in computerHand)
                    {
                        if (rememberedCards.Contains(card.name[0]))
                        {
                            computerCardRequest = card;
                            cpuUsedRememberedCard = true;
                            rememberedCards.RemoveAll(item => item == card.name[0]); //removes all instances of that char from CPU memory
                            break;
                        }
                    }
                    //if the cpu cannot remember, pick a random card:
                    if (!cpuUsedRememberedCard)
                    {
                        computerCardRequest = computerHand[Globals.numgen.Next(0, computerHand.Count - 1)];
                    }

                    Console.WriteLine("Computer asks for a " + computerCardRequest.nameSansSuit);
                    for (int i = 0; i < playerHand.Count; i++)
                    {
                        if (playerHand[i].name[0] == computerCardRequest.name[0])
                        {
                            computerGotCard = true;
                            Console.WriteLine("The Computer got " + playerHand[i].name);
                            retrievedCards.Add(playerHand[i]);
                        }
                    }
                    foreach (Card card in retrievedCards)
                    {
                        playerHand.Remove(card);
                        computerHand.Add(card);
                    }
                    retrievedCards.Clear();

                    //if computer doesn't get a card, send it fishing
                    if (!computerGotCard && deck.Count != 0)
                    {
                        computerHand.Add(CreateHand(ref deck, 1)[0]);
                        Console.WriteLine("The computer goes fishing");
                    }
                }
                //removes sets from player hand if they have them
                for (int i = 1; i <= 13; i++)
                {
                    foreach (Card card in playerHand)
                    {
                        if (card.value == i)
                        {
                            repeatedCards.Add(card);
                        }
                    }

                    if (repeatedCards.Count == 4)
                    {
                        playerSetCount += 1;
                        Console.WriteLine("----You completed the " + repeatedCards[0].nameSansSuit + " set----");
                        foreach (Card card in repeatedCards)
                        {
                            playerHand.Remove(card);
                        }
                    }
                    repeatedCards.Clear();
                }

                //removes sets from the computer
                for (int i = 1; i <= 13; i++)
                {
                    foreach (Card card in computerHand)
                    {
                        if (card.value == i)
                        {
                            repeatedCards.Add(card);
                        }
                    }

                    if (repeatedCards.Count == 4)
                    {
                        computerSetCount += 1;
                        Console.WriteLine("----The Computer completed the " + repeatedCards[0].nameSansSuit + " set----");
                        foreach (Card card in repeatedCards)
                        {
                            computerHand.Remove(card);
                        }
                    }
                    repeatedCards.Clear();
                }
                if (deck.Count == 0 && !deckEmptyMessageDisplayed)
                {
                    Console.WriteLine("----Deck Empty. No more fishing----");
                    deckEmptyMessageDisplayed = true;
                }

                //game over check
                if (playerHand.Count == 0 || computerHand.Count == 0)
                {
                    gameOver = true;
                }
            }
            Console.WriteLine("----Game over due to a player using all cards----");
            Console.WriteLine("The deck had " + deck.Count + " cards left");
            Console.WriteLine("You had " + playerSetCount.ToString() + " sets with " + playerHand.Count.ToString() + " cards to spare");
            Console.WriteLine("The Computer had " + computerSetCount.ToString() + " sets with " + computerHand.Count.ToString() + " cards to spare");
            if (playerSetCount > computerSetCount)
            {
                Console.WriteLine("You Win");
            }
            else if (playerSetCount == computerSetCount)
            {
                Console.WriteLine("You Tie");
            }
            else
            {
                Console.WriteLine("You Lose");
            }
            Console.WriteLine("-------------------------------------------");

            Console.ReadLine();
        }
    }
}







