using System.Collections.Generic;
using UnityEngine;

namespace GameEnvironment.Controller
{
    public class PlaceholderRoot : MonoBehaviour
    {
        [SerializeField] List<PlaceHolder> m_placeHolders;
        public IList<PlaceHolder> PlaceHolders => m_placeHolders;
    }
}
