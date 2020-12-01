using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_CP
{
    public enum Agents { Confidential, Secret, TopSecret };

    class Agent
    {
        public static Mutex mutex = new Mutex();

        TextWriter tw;

        public string agentName { get; set; }

        public int brUsed;

        public Elevator elevator { get; set; }
        public Floor button;

        Random rand = new Random();

        Floor floor;

        ManualResetEvent eventAtHome = new ManualResetEvent(false);

        public Agents securityLevel;

        public Agent(TextWriter tw, Elevator elevator, string agentName, Agents securityLevel)
        {
            this.tw = tw;
            this.elevator = elevator;
            this.agentName = agentName;
            this.securityLevel = securityLevel;
            brUsed = rand.Next(5, 7);
        }

        public Floor GetRandomChoiceAction()
        {
            var availableButtons = elevator.availableFloors();
            int index = rand.Next(100) % 3;
            return availableButtons[index];
        }

        public void GoToWork()
        {
            tw.WriteLine(agentName + " goes to work.");
            floor = Floor.G;
        }


        public void EnterElevator()
        {
            tw.WriteLine(agentName + " waits in line for the elevator.");
            mutex.WaitOne();
            Thread.Sleep(500);
            tw.WriteLine(agentName + " calls the elevator.");
            elevator.Moving(floor);
            elevator.Enter(this);
            tw.WriteLine(agentName + " entered the elevator.");
            while (true)
            {
                
                button = GetRandomChoiceAction();
                switch (button)
                {
                    case Floor.G:
                        elevator.Moving(Floor.G);
                        tw.WriteLine(agentName + " is at ground floor.");
                        floor = Floor.G;
                        elevator.InElevator(Floor.G);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.S:
                        elevator.Moving(Floor.S);
                        tw.WriteLine(agentName + " is at secret floor with nuclear weapons.");
                        floor = Floor.S;
                        elevator.InElevator(Floor.S);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.T1:
                        elevator.Moving(Floor.T1);
                        tw.WriteLine(agentName + " is at secret floor with experimental weapons.");
                        floor = Floor.T1;
                        elevator.InElevator(Floor.T1);
                        if (elevator.canExit) goto outside;
                        break;
                    case Floor.T2:
                        elevator.Moving(Floor.T2);
                        tw.WriteLine(agentName + " is at top-secret floor that stores alien remains.");
                        floor = Floor.T2;
                        elevator.InElevator(Floor.T2);
                        if (elevator.canExit) goto outside;
                        break;
                    default:
                        throw new ArgumentException(button + " action is not supported.");
                }
            }
        outside:
            return;
        }

        public void LeaveElevator()
        {
            // Simulate some work.
            Thread.Sleep(500);

            tw.WriteLine(agentName + " is leaving the elevator.");
            floor = elevator.floor;
            
            // Release the Mutex.
            mutex.ReleaseMutex();
        }

        private void WorkingProcess()
        {
            GoToWork();
            while (brUsed!=0)
            {
                EnterElevator();
                LeaveElevator();
                Thread.Sleep(500);
                brUsed--;
            }
            tw.WriteLine($"{agentName} has finished work.");
            eventAtHome.Set();
        }

        public void Working()
        {
            new Thread(WorkingProcess).Start();
        }

        public bool AtHome
        {
            get
            {
                return eventAtHome.WaitOne(0);
            }
        }
    }
}
