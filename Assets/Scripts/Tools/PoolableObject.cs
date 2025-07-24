using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Маркер, указывающий на то, что данный объект может быть помещен в пул
    /// Хранит ссылку на родительский пул для возможности возврата
    /// </summary>
    public class PoolableObject : MonoBehaviour
    {
        public SimpleObjectPool ParentPool { get; set; }
    }
}