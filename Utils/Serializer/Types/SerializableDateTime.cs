using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Replay.Utils
{
    [Serializable]
    public class SerializableDateTime
    {
        [SerializeField] public long ticks;
        [SerializeField] public DateTimeKind kind;

        public SerializableDateTime(DateTime dateTime)
        {
            ticks = dateTime.Ticks;
            kind = dateTime.Kind;
        }

        public DateTime ToDateTime() =>
            new (ticks, kind);
    }

    [Serializable]
    [JsonConverter(typeof(SerializableDateJsonConverter))]
    public class SerializableDate
    {
        public enum Month
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        [SerializeField] public Month month = (Month)DateTime.Now.Month;
        [SerializeField] [Range(1, 31)] public int day = DateTime.Now.Day;
        [SerializeField] public int year = DateTime.Now.Year;
        
        public void SetDate(DateTime date)
        {
            month = (Month)date.Month;
            day = date.Day;
            year = date.Year;
        }
        
        public DateTime ToDateTime() => 
            new (year, month.intValue(), day);
    }
}

