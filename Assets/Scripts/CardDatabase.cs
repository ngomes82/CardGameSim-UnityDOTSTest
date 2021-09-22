using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft;
using Newtonsoft.Json;

public class CardDatabase : MonoBehaviour
{
    public TextAsset json;

    public Dictionary<string, CardData> cardDatabase;

    public List<string> cardKeys;
    public List<float> cardWeights;
    public float sumWeights;

    public int seed = -1; //847100009
    public int player1Victory;
    public int player2Victory;

    public float health1 = 0f;
    public float health2 = 0f;

    List<string> deck1 = new List<string>();
    List<string> deck2 = new List<string>();

    List<string> hand1 = new List<string>();
    List<string> hand2 = new List<string>();

    List<string> cardsInPlay1 = new List<string>();
    List<Dictionary<string,float>> attributesInPlay1 = new List<Dictionary<string,float>>();

    List<string> cardsInPlay2 = new List<string>();
    List<Dictionary<string, float>> attributesInPlay2 = new List<Dictionary<string, float>>();


    List<string> startingDeck1 = new List<string>();
    List<string> startingDeck2 = new List<string>();

    private bool player1Start = false;

    public void Awake()
    {
        if (seed == -1)
        {
            seed = Random.Range(0, int.MaxValue);
        }

        Random.InitState(seed);

        LoadCardData();
    }

    public void LoadCardData()
    {
        cardDatabase = JsonConvert.DeserializeObject<Dictionary<string, CardData>>(json.text);

        float sumRarities = 0f;
        foreach(var card in cardDatabase)
        {
            sumRarities += card.Value.rarity;
        }

        foreach(var card in cardDatabase)
        {
            float weight = sumRarities * card.Value.rarity;
            sumWeights += weight;

            cardKeys.Add(card.Key);
            cardWeights.Add(weight);
        }

        CreateRandomDeck(startingDeck1, 52);
        CreateRandomDeck(startingDeck2, 52);
    }

    private void Update()
    {
        int simsPerFrameCounter = 10000;
        while(simsPerFrameCounter > 0 )
        {
            if (deck1.Count > 0 && deck2.Count > 0 && health1 > 0.0f && health2 > 0.0f)
            {
                PlayTurn(player1Start);
                player1Start = !player1Start;
            }
            else
            {
                player1Start = Random.value > 0.5f;

                if (health1 > health2 && health1 > 0f)
                {
                    player1Victory++;
                }
                else if (health2 > health1 && health2 > 0f)
                {
                    player2Victory++;
                }

                health1 = 100.0f;
                health2 = 100.0f;

                deck1.Clear();
                deck2.Clear();

                hand1.Clear();
                hand2.Clear();

                cardsInPlay1.Clear();
                cardsInPlay2.Clear();

                attributesInPlay1.Clear();
                attributesInPlay2.Clear();

                deck1 = new List<string>(startingDeck1);
                deck2 = new List<string>(startingDeck2);

                deck1.Shuffle();
                deck2.Shuffle();

                DrawHand(deck1, hand1, 5);
                DrawHand(deck2, hand2, 5);
            }

            simsPerFrameCounter--;
        }

        Debug.Log($"Victory Counts {player1Victory} vs {player2Victory}  Ratio { player1Victory / (float)(player1Victory + player2Victory)}");

    }

    private void PlayTurn(bool player1Start)
    {
        if (player1Start)
        {
            PlayCard(hand1, cardsInPlay1, attributesInPlay1);
            EvaluateAttack(cardsInPlay1, attributesInPlay1, ref health2, cardsInPlay2, attributesInPlay2);
            DrawHand(deck1, hand1, 5);

            PlayCard(hand2, cardsInPlay2, attributesInPlay2);
            EvaluateAttack(cardsInPlay2, attributesInPlay2, ref health1, cardsInPlay1, attributesInPlay1);
            DrawHand(deck2, hand2, 5);
        }
        else
        {
            PlayCard(hand2, cardsInPlay2, attributesInPlay2);
            EvaluateAttack(cardsInPlay2, attributesInPlay2, ref health1, cardsInPlay1, attributesInPlay1);
            DrawHand(deck2, hand2, 5);

            PlayCard(hand1, cardsInPlay1, attributesInPlay1);
            EvaluateAttack(cardsInPlay1, attributesInPlay1, ref health2, cardsInPlay2, attributesInPlay2);
            DrawHand(deck1, hand1, 5);
        }
    }



    private void PlayCard(List<string> hand, List<string> cardsInPlay, List<Dictionary<string, float>> attributesInPlay)
    {
        int randIndex1 = Random.Range(0, hand.Count);
        string cardKey = hand[randIndex1];

        cardsInPlay.Add(cardKey);
        attributesInPlay.Add(new Dictionary<string, float>(cardDatabase[cardKey].attributes) );

        hand.RemoveAt(randIndex1);
    }


    private void EvaluateAttack(List<string> attackerCardsInPlay, List<Dictionary<string, float>> attackerAttributesInPlay,
                                ref float defenderHealth, List<string> defenderCardsInPlay, List<Dictionary<string, float>> defenderAttributesInPlay)
    {

        for(int i=0; i < attackerCardsInPlay.Count; i++)
        {
            Dictionary<string, float> attackerCardAttributes = attackerAttributesInPlay[i];
            int randIndex = Random.Range(0, defenderCardsInPlay.Count);

            if (randIndex < defenderCardsInPlay.Count - 1)
            {
                Dictionary<string, float> defenderCardAttributes = defenderAttributesInPlay[randIndex];

                defenderCardAttributes["health"] -= attackerCardAttributes["attack"] * 1f - defenderCardAttributes["defense"];

                if (defenderCardAttributes["health"] <= 0f)
                {
                    defenderAttributesInPlay.RemoveAt(randIndex);
                    defenderCardsInPlay.RemoveAt(randIndex);
                }
            }
            else
            {
                defenderHealth -= attackerCardAttributes["attack"];
            }
        }
    }

    private void DrawHand(List<string> deck, List<string> hand, int size)
    {
        while(hand.Count < size)
        {
            hand.Add( deck.Pop() );
        }
    }

    private void CreateRandomDeck(List<string> deck, int size)
    {
        for (int i = 0; i < size; i++)
        {
            float rand = Random.Range(0f, sumWeights);

            for (int k = 0; k < cardKeys.Count; k++)
            {
                rand -= cardWeights[k];
                if (rand <= 0f || k == cardKeys.Count-1)
                {
                    deck.Add(cardKeys[k]);
                    break;
                }
            }
        }
    }
}

public class CardData
{
    public string name;
    public float rarity;
    public Dictionary<string, float> attributes;
}

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> toShuffle)
    {
        int n = toShuffle.Count;
        for (int i = 0; i < (n - 1); i++)
        {
            int r = i + Random.Range(0, n - i);
            T t = toShuffle[r];
            toShuffle[r] = toShuffle[i];
            toShuffle[i] = t;
        }
    }

    public static T Pop<T>(this List<T> toPop)
    {
        T toReturn = toPop[toPop.Count - 1];

        toPop.RemoveAt(toPop.Count - 1);

        return toReturn;
    }
}