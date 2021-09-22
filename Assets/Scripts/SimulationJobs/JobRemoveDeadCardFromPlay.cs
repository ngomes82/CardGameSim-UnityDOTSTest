using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct JobRemoveDeadCardFromPlay : IJobParallelFor
{
    public int maxCardsInPlay;

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
        int inPlayIdx = playerIdx * maxCardsInPlay;

        int cardToRemoveIdx = -1;

        for (int i = 0; i < inPlayCount[playerIdx] - 1; i++)
        {
            int cardIdx = inPlayIdx + i;
            if (cardAttributes1[cardIdx] <= 0f)
            {
                cardToRemoveIdx = cardIdx;
                break;
            }
        }

        if (cardToRemoveIdx > -1)
        {
            int topIndex = inPlayIdx + inPlayCount[playerIdx] - 1;

            inPlay[cardToRemoveIdx] = inPlay[topIndex];
            cardAttributes1[cardToRemoveIdx] = cardAttributes1[topIndex];
            cardAttributes2[cardToRemoveIdx] = cardAttributes2[topIndex];
            cardAttributes3[cardToRemoveIdx] = cardAttributes3[topIndex];


            inPlay[topIndex] = 0;
            cardAttributes1[topIndex] = 0f;
            cardAttributes2[topIndex] = 0f;
            cardAttributes3[topIndex] = 0f;

            inPlayCount[playerIdx]--;
        }
    }
}