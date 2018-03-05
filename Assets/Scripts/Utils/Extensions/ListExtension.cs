using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension {

    //================================================================//
    //===================Fisher_Yates_CardDeck_Shuffle====================//
    //================================================================//

    /// With the Fisher-Yates shuffle, first implemented on computers by Durstenfeld in 1964, 
    ///   we randomly sort elements. This is an accurate, effective shuffling method for all array types.

    public static List<T> Shuffle<T> (this List<T> aList) {

        T myGO;

        int n = aList.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, (n - i));
            myGO = aList[r];
            aList[r] = aList[i];
            aList[i] = myGO;
        }

        return aList;
    }
}
