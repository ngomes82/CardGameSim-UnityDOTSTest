using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public struct JobEvaluateRound : IJobParallelFor
{
    public Random rand;
    public int nPlayers;
    public int maxCardsInPlay;

    [NativeDisableParallelForRestriction]
    public NativeArray<byte> inPlayCount;

    [NativeDisableParallelForRestriction]
    public NativeArray<ushort> inPlay;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes1; //Health

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes2; //Attack

    [NativeDisableParallelForRestriction]
    public NativeArray<float> cardAttributes3; //Defense

    public void Execute(int playerIdx)
    {
        if (inPlayCount[playerIdx] == 0)
            return;

        int relativePlayerIdx = playerIdx % nPlayers;
        int nextPlayerDirection = relativePlayerIdx == 0 ? 1 : -1;
        int otherPlayerIdx = playerIdx + nextPlayerDirection;

        if (inPlayCount[otherPlayerIdx] == 0)
            return;

        int playerInPlayIdx = playerIdx * maxCardsInPlay;
        int otherPlayerInPlayIdx = otherPlayerIdx * maxCardsInPlay;

        int randCardToAttackIdx = playerInPlayIdx + rand.NextInt(0, inPlayCount[playerIdx]);
        int randCardToDefendIdx = otherPlayerInPlayIdx + rand.NextInt(0, inPlayCount[otherPlayerIdx]);

        cardAttributes1[randCardToDefendIdx] -= cardAttributes2[randCardToAttackIdx] * (1f - cardAttributes3[randCardToDefendIdx]);

    }
}