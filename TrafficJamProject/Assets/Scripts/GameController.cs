using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cai
{
    public class GameController : MonoBehaviour
    {
        private void Awake()
        {
            Physics2D.queriesStartInColliders = false;
        }
    }

}
