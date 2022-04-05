using System;   
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace PriorityQueue
{
    public class PriorityQueue<U> 
    {
        private readonly List<(double,U)> _pq = new List<(double,U)>();

        public int Count => _pq.Count;

        public void Enqueue(double value, U item)
        {
            if(_pq.Count == 0 )
            {
                _pq.Insert(0, (value, item));
            }
            else
            {
                int i = 0;
                while(true)
                {
                    if(i == _pq.Count)
                    {
                        _pq.Add((value, item));
                        break;
                    }
                    if(value < _pq[i].Item1)
                    {
                        _pq.Insert(i, (value, item));
                        break; 
                    }
                    i++;
                }
            }
        }

        public U Dequeue()
        {
            (double, U) highestPrioritizedItem = _pq[0];
            _pq.RemoveAt(0);
            return highestPrioritizedItem.Item2;
        }

        public U AtIndex(int index)
        {
            return _pq[index].Item2;
        }
    }
}
