using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker : MonoBehaviour
{
    static Queue<ICommand> commandBuffer;
    static List<ICommand> commandHistory;
    static int counter;

    [SerializeField] private InputManager _controls;//Input System

    private void Awake()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();
    }

    public static void AddCommand(ICommand command)
    {
        while (commandHistory.Count > counter)
        {
            commandHistory.RemoveAt(counter);
        }
        commandBuffer.Enqueue(command);
    }

    public void Reset()
    {
        commandBuffer = new Queue<ICommand>();
        commandHistory = new List<ICommand>();
        counter = 0;
    }

    private void Update()
    {

        if(commandBuffer.Count > 0)
        {
            ICommand c = commandBuffer.Dequeue();
            c.Execute();

            commandHistory.Add(c);
            counter++;
        }
        else
        {
            if (_controls.Undo())
            {
                if (counter > 0)
                {
                    counter--;
                    commandHistory[counter].Undo();

                }
                
            }
            else if(_controls.Redo())
            {
                if(counter < commandHistory.Count)
                {
                    commandHistory[counter].Execute();
                    counter++;
                }
            }
        }
    }
}
