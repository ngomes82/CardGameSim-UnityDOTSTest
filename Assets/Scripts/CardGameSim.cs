using System;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;


public class CardGameSim : UnityEngine.MonoBehaviour
{
    public Text deckDebug;
    public Text handDebug;
    public Text inPlayDebug;
    public Text attribute1Debug;

    public int nSims = 1000;
    public int nRoundsPerFrame = 10;
    public int nPlayers = 2;
    public int deckSize = 52;
    public int handSize = 5;
    public int maxCardsInPlay = 5;
    public int nGamesToDrawDebugLogs = 6;


    public CardGameSimData simData;

    public JobHandle completionHandle;

    void Start()
    {
        simData = new CardGameSimData(nSims, nPlayers, nRoundsPerFrame, deckSize, handSize, maxCardsInPlay);
    }

    void Update()
    {
        simData.rand = new Random((uint)(DateTime.Now.Millisecond) + 1);

        JobHandle prevRoundJob = default(JobHandle);

        for(int i=0; i < nRoundsPerFrame; i++)
        {
            prevRoundJob = simData.ScheduleJobsForGameRound(prevRoundJob);
        }

        completionHandle = prevRoundJob;
    }

    private void LateUpdate()
    {
        completionHandle.Complete();
            
        DrawDebugString(deckDebug, nGamesToDrawDebugLogs, simData.deckSize, simData.deckCount, simData.decks);
        DrawDebugString(handDebug, nGamesToDrawDebugLogs, simData.handSize, simData.handCount, simData.hands);
        DrawDebugString(inPlayDebug, nGamesToDrawDebugLogs, simData.maxCardsInPlay, simData.inPlayCount, simData.inPlay);
        DrawDebugString(attribute1Debug, nGamesToDrawDebugLogs, simData.maxCardsInPlay, simData.inPlayCount, simData.cardAttributes1);
    }

    private void DrawDebugString<T>(Text textField, int nToDrawn, int maxSize, NativeArray<byte> varCount, NativeArray<T> vars) where T : struct
    {
        StringBuilder sb = new StringBuilder();
            
        for (int i = 0; i < nToDrawn * maxSize; i++)
        {
            if (i % maxSize == 0)
            {
                if (i > 0)
                    sb.Append('\n');

                int idx = i / maxSize;
                sb.Append($"({varCount[idx]}) ");
            }

            sb.Append($"{vars[i]},");
        }

        textField.text = sb.ToString();
    }

    private void OnDestroy()
    {
        simData.Dispose();
    }
}
