using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pranam
{
    public static class ThreadUtils
    {
        public static void RunInBackgroundAndForget(this Task task)
        {
        }

        public static void RunInBackgroundAndForget(Action action)
        {
            Task.Run(action).RunInBackgroundAndForget();
        }

        public static T WaitAndGetResult<T>(this Task<T> task, int timeoutMiliseconds = -1)
        {
            if (task == null) return default(T);
            if (timeoutMiliseconds > 0)
            {
                task.Wait(timeoutMiliseconds);
            }
            else
                task.Wait();

            return task.Result;
        }

        public static void WaitTillAvailableToProcess<TA>(this SimpleRestmeQueue<TA> queue, TA newObject)
        {
            if (queue != null)
            {
                #region This code can be better improved using message queue mechanism

                if (queue.ExecutionWaitRequired)
                {
                    while (queue.ExecutionWaitRequired)
                    {
                        lock (queue)
                        {
                            if (queue.QueueItems == null)
                            {
                                queue.QueueItems = new List<TA>();
                            }

                            if (queue.ExecutionWaitRequired)
                                Thread.Sleep(1);
                            else
                            {
                                queue.QueueItems.Add(newObject);
                                break;
                            }

                            if (queue.ExecutionWaitRequired)
                                Thread.Sleep(100);
                        }
                    }
                }
                else
                {
                    if (queue.QueueItems == null)
                    {
                        queue.QueueItems = new List<TA>();
                    }

                    queue.QueueItems.Add(newObject);
                }

                #endregion
            }
        }

        public static void ClearProcessingObject<TA>(this SimpleRestmeQueue<TA> queue,
            TA singleObjectToRemove = default,
            bool throwExceptionIfObjectNotFound = true, bool updateWaitRequired = true)
        {
            if (queue != null)
            {
                lock (queue)
                {
                    if (singleObjectToRemove != null)
                    {
                        if (queue.QueueItems == null)
                        {
                            queue.QueueItems = new List<TA>();
                        }

                        var indexOfObject = queue.QueueItems.IndexOf(singleObjectToRemove);
                        if (indexOfObject >= 0)
                        {
                            queue.QueueItems.Remove(singleObjectToRemove);
                            if (updateWaitRequired)
                            {
                                queue.ExecutionWaitRequired = false;
                            }
                        }
                        else if (throwExceptionIfObjectNotFound)
                        {
                            throw new PranamException("Object to remove is no longer in the processing queue");
                        }
                    }
                    else
                    {
                        queue.QueueItems.Clear();
                        if (updateWaitRequired)
                        {
                            queue.ExecutionWaitRequired = false;
                        }
                    }
                }
            }
        }
    }
}