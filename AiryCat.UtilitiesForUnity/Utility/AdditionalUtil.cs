using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalUtil {

    /// <summary>
    /// Gets a random subset from the given list and of the given size.
    /// </summary>
    /// <returns>The random subset.</returns>
    /// <param name="set">Set.</param>
    /// <param name="subsetSize">Subset size.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static ICollection<T> GetRandomSubset<T>(ICollection<T> set, int subsetSize)
    {

        // Create a clone to avoid modifying the given set.
        List<T> clonedSet = new List<T>(set);

        ICollection<T> subset = new List<T>();
        for (int i = 0; i < subsetSize; i++)
        {
            int index = Random.Range(0, clonedSet.Count);
            subset.Add(clonedSet[index]);
            clonedSet.RemoveAt(index);
        }

        return subset;
    }
}
