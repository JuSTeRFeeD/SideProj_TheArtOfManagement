using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Scriptable
{
    [CreateAssetMenu(menuName ="Game/Employer Data")]
    public class EmployerData : ScriptableObject
    {
        [SerializeField] private string employerName;
        [PreviewField]
        [SerializeField] private Sprite icon;

        public string EmployerName => employerName;
        public Sprite Icon => icon;
    }
}