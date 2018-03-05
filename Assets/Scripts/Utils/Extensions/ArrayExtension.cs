using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension {

	//================================================================//
    //===================Fisher_Yates_CardDeck_Shuffle====================//
    //================================================================//

    /// With the Fisher-Yates shuffle, first implemented on computers by Durstenfeld in 1964, 
    ///   we randomly sort elements. This is an accurate, effective shuffling method for all array types.

    public static T[] Shuffle<T> (this T[] array) {

        T myGO;

        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, (n - i));
            myGO = array[r];
            array[r] = array[i];
            array[i] = myGO;
        }

        return array;
    }
}
