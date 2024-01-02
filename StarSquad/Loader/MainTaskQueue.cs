using System;
using System.Collections.Generic;
using StarSquad.Net;
using UnityEngine;

namespace StarSquad.Loader
{
    public class MainTaskQueue
    {
        private volatile bool queued;
        private readonly Queue<Action> queue = new();

        public void RunOnMainThread(Action action)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(action);
                this.queued = true;
            } 
        }

        public void ProcessTaskQueue()
        {
            if (!this.queued) return;
            
            lock (this.queue)
            {
                this.queued = false;

                while (this.queue.Count > 0)
                {
                    var action = this.queue.Dequeue();

                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Failed to invoke task!");
                        Debug.Log(e);
                        
                        NetworkManager.GetNet().connectionManager.SafeDisconnect("task queue failed");
                        
                        LoaderManager.instance.ShowBadError("Something went wrong in the game process");
                    }
                }
            }
        }
    }
}