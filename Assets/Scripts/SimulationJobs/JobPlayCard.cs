using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public struct JobPlayCard : IJobParallelFor
{
    public Random rand;
    public int handSize;
    public int maxCardsInPlay;


    [NativeDisableParallelForRestriction]
    public NativeArray<byte> handCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> hands;

    [NativeDisableParallelForRestriction]
    public NativeArray<byte> inPlayCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> inPlay;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes1;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes2;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes3;

    public void Execute(int playerIdx)
    {
        if (handCount[playerIdx] > 0 && inPlayCount[playerIdx] < maxCardsInPlay)
        {
            int handIdx = playerIdx * handSize;
            int inPlayIdx = playerIdx * maxCardsInPlay;

            int randHandCardIdx = handIdx + rand.NextInt(0, handCount[playerIdx] - 1);

            int topHandIdx = handIdx + handCount[playerIdx] - 1;
            int emptyInPlayIdx = inPlayIdx + inPlayCount[playerIdx];

            inPlay[emptyInPlayIdx] = hands[randHandCardIdx];

            //TODO: Pull attributes out of actual data fed in from JSON?
            cardAttributes1[emptyInPlayIdx] = rand.NextFloat(2.0f, 10.0f);
            cardAttributes2[emptyInPlayIdx] = rand.NextFloat(1.0f, 5.0f);
            cardAttributes3[emptyInPlayIdx] = rand.NextFloat(0.0f, 0.0f);
            //END TODO

            hands[randHandCardIdx] = hands[topHandIdx];
            hands[topHandIdx] = 0;

            inPlayCount[playerIdx]++;
            handCount[playerIdx]--;
        }
    }
}