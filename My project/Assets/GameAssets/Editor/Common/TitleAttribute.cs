using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAsset
{
    public class TitleAttribute : PropertyAttribute
    {
        public string newTitle { get; private set; }
        public TitleAttribute(string title)
        {
            newTitle = title;
        }
    }

}
