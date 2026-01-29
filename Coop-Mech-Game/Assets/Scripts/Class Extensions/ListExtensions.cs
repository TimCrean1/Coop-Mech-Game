using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i.IsValidIndex(list))
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }

    public static bool IsValidIndex<T>(this int index, IList<T> list)
    {
        return list != null && index >=0 && index < list.Count;
    }
}
