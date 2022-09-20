using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.StatCounting.UI
{
    public class StatCountingUIController : MonoBehaviour
    {
        [SerializeField] private UIDocument _statCountingDocument;
        [SerializeField] private PremadeCounterType[] _premadeCounters;
        [SerializeField] private UnityEvent _onBackButtonClick;
        
        private void OnEnable()
        {
            var root = _statCountingDocument.rootVisualElement;
            var scrollView = new ScrollView{touchScrollBehavior = ScrollView.TouchScrollBehavior.Elastic, verticalPageSize = 1000, elasticity = 1000};
            scrollView.Add(new StatCountingChanger(_premadeCounters));
            root.Add(scrollView);
            var backButton = root.Q<Button>(StatCountingChanger.BACK_BUTTON_NAME);
            backButton.clicked += _onBackButtonClick.Invoke;
            backButton.clicked += () => AppSettings.TrySave();
        }
    }
}