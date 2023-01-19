using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADN2 : MonoBehaviour
{
    public bool greedy;
    // non-greedy vs non-greedy => 100% survival chance for both
    //                             50% reproduction chance for both
    // greedy vs non-greedy => 100% survival chance for greedy
    //                         100% reproduction chance for greedy
    //                         50% survival chance for non-greedy
    // greedy vs greedy => 50% survival rate for each blob
}
