using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project_CP
{
    public enum Floor { G = 1, S, T1, T2 };

    class Elevator
    {
        Agent user;
        public Floor floor = Floor.G;
        readonly List<Floor> floors = new List<Floor> { Floor.G, Floor.S, Floor.T1, Floor.T2 };
        TextWriter tw = null;
        public bool canExit = false;
        public Elevator(TextWriter tw)
        {
            this.tw = tw;
        }

        public List<Floor> availableFloors()
        {
            List<Floor> _ = new List<Floor>();
            floors.ForEach(x => _.Add(x));
            _.Remove(floor);
            return _;
        }

        public void Enter(Agent agent)
        {
            user = agent;
        }

        public void Moving(Floor dest)
        {
            if (floor != dest)
            {
                Thread.Sleep(500);
                tw.WriteLine("The elevator is moving.");
                var time = Math.Abs(floor - dest);
                //Thread.Sleep(time * 1000);
                for(int i = 0; i < time; i++)
                {
                    tw.WriteLine($"\t\t\t{i+1} sec has passed.");
                }
                tw.WriteLine($"The elevator is at {dest}.");
                floor = dest;
            }
            else
            {
                tw.WriteLine("The elevator is here.");
            }
        }

        public void InElevator(Floor elevatorAction)
        {
            canExit = false;
            if (user != null)
            {
                if (elevatorAction == Floor.T1 || elevatorAction == Floor.T2)
                {
                    if (user.securityLevel == Agents.TopSecret)
                    {
                        canExit = true;
                        Leave();
                    }
                    else
                    {
                        tw.WriteLine("\nYou don't have authority!!!\n");
                        elevatorAction = user.GetRandomChoiceAction();
                    }
                }
                else if (elevatorAction == Floor.S)
                {
                    if (user.securityLevel == Agents.Secret || user.securityLevel == Agents.TopSecret)
                    {
                        canExit = true;
                        Leave();
                    }
                    else
                    {
                        tw.WriteLine("\nYou don't have authority!!!\n");
                        elevatorAction = user.GetRandomChoiceAction();
                    }
                }
                else
                {
                    canExit = true;
                    Leave();
                }
            }
        }

        public void Leave()
        {
            user = null;
        }
    }
}
