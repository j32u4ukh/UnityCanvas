using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExpansion;
using UList = UnityExpansion.List;
using UArray = UnityExpansion.Array;

public class TestUtils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<int> numbers = new List<int>() {1, 2, 3, 4 };
        Debugging.Log(UList.ToString(numbers));

        int[] array = numbers.ToArray();
        Debugging.Log(UArray.ToString(array));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
