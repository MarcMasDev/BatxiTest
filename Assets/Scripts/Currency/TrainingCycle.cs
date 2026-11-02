using System.Collections.Generic;
using UnityEngine;


public class TrainingCycle : MonoBehaviour
{
    [SerializeField] private ActionController actionController;
    [SerializeField] private ProgressionBalance balance;

    private List<TrainingAction> machinesLeft = new List<TrainingAction>(); //Máquinas que faltan en esta serie
    private bool onMachine;
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (onMachine)
        {
            actionController.Play(TrainingAction.Resting);
            timer = balance.restSeconds;
        }
        else
        {
            actionController.Play(NextMachine());
            timer = balance.machineSeconds;
        }

        onMachine = !onMachine;
    }

    private TrainingAction NextMachine()
    {
        if (machinesLeft.Count == 0) //Serie terminada (o la primera): vuelven las 4
        {
            machinesLeft.Add(TrainingAction.Curl);
            machinesLeft.Add(TrainingAction.Run);
            machinesLeft.Add(TrainingAction.BenchPress);
            machinesLeft.Add(TrainingAction.LegTrain);
        }

        //Coger una al azar y quitarla de la lista = orden aleatorio sin repetir
        int index = Random.Range(0, machinesLeft.Count);
        TrainingAction machine = machinesLeft[index];
        machinesLeft.RemoveAt(index);
        return machine;
    }
}
