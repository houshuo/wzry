namespace ApolloUpdate
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ApolloUpdateSpeedCounter
    {
        private uint currentSize;
        private bool doTimer;
        private uint lastCurrentSize;
        private LinkedList<uint> mSpeedCountList = new LinkedList<uint>();
        private uint speed;
        public float timer = 1f;

        public uint GetCurrentSpeed()
        {
            return this.speed;
        }

        public uint GetSpeed()
        {
            uint num = 1;
            uint num2 = 0;
            ulong num3 = 0L;
            foreach (uint num4 in this.mSpeedCountList)
            {
                num3 += (num4 * num) * num;
                num2 += num * num;
                num++;
            }
            return (uint) (num3 / ((num2 <= 0) ? ((ulong) 1) : ((ulong) num2)));
        }

        public void SetSize(uint size)
        {
            this.currentSize = size;
        }

        public void SpeedCounter()
        {
            if (this.doTimer)
            {
                this.timer -= Time.deltaTime;
                if (this.timer <= 0f)
                {
                    this.timer = 1f;
                    uint num = this.currentSize - this.lastCurrentSize;
                    this.lastCurrentSize = this.currentSize;
                    if (this.mSpeedCountList.Count >= 5)
                    {
                        this.mSpeedCountList.RemoveFirst();
                        this.mSpeedCountList.AddLast(num);
                    }
                    else
                    {
                        this.mSpeedCountList.AddLast(num);
                    }
                    this.speed = num;
                }
            }
        }

        public void StartSpeedCounter()
        {
            this.doTimer = true;
            this.mSpeedCountList.Clear();
            this.lastCurrentSize = 0;
            this.timer = 0f;
            this.speed = 0;
        }

        public void StopSpeedCounter()
        {
            this.doTimer = false;
            this.mSpeedCountList.Clear();
            this.lastCurrentSize = 0;
            this.timer = 0f;
            this.speed = 0;
        }
    }
}

