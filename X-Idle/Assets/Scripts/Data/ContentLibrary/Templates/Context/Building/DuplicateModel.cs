using System;
using UnityEngine;

namespace Data.ContentLibrary.Templates.Context.Building
{
    [Serializable]
    public class DuplicateModel
    {
        [SerializeField] private bool m_canDuplicate;
        [SerializeField] private int m_maxDuplicateAmount;

        public bool CanDuplicate => m_canDuplicate;
        public int MaxDuplicateAmount => m_maxDuplicateAmount;
    }
}