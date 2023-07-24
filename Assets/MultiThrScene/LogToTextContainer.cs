using System.Collections.Generic;
using System.Linq;
using UKnack.Attributes;
using UKnack.Events;
using UKnack.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace InGame
{
    public class LogToTextContainer : MonoBehaviour, ISubscriberToEvent<string>
    {
        [SerializeField]
        [ProvidedComponent]
        private UIDocument _layoutProvider;

        [SerializeField]
        [ValidReference]
        private SOEvent<string> _logEvent;

        [SerializeField]
        private string _logTextElementId;

        private VisualElement _textContainer;

        private string _description = nameof(LogToTextContainer);
        public string Description => _description;
        public void OnEventNotification(string t) => 
            RecievedNewTextMessage(t);

        [SerializeField]
        private bool _logBuilderReverseMessages = false;
        [SerializeField]
        private int _logMaxTextMessages = 8;

        private Queue<string> _logBuilder = new();
     

        private void RecievedNewTextMessage(string text)
        {
            if (_logBuilder.Count > _logMaxTextMessages) 
                _logBuilder.Dequeue();
            _logBuilder.Enqueue(text);

            _textContainer.TryAssignTextWithoutNotification(
                _logBuilderReverseMessages ? 
                string.Join("\n", _logBuilder.Reverse()) : 
                string.Join("\n", _logBuilder));
        }

        private void OnEnable()
        {
            _description = $"{gameObject.name} + {nameof(LogToTextContainer)}";
            ValidateFields();
            _textContainer = _layoutProvider.rootVisualElement.TryFindSomeKindOfTextStorage(_logTextElementId);
            if (_textContainer == null)
                throw new System.Exception($"No any text container found with id:{_logTextElementId}");
            _logEvent.Subscribe(this);
            Debug.Log("OnEnable finished");
        }

        private void OnDisable()
        {
            _textContainer = null;
            ValidateFields();
            _logEvent.UnsubscribeNullSafe(this);
            Debug.Log("OnDisable finished");
        }
        private void ValidateFields()
        {
            if (_layoutProvider == null)
                _layoutProvider = GetComponentInParent<UIDocument>();
            if (_layoutProvider == null)
                throw new System.ArgumentNullException($"{nameof(_layoutProvider)} no layout set and no UIDocument found in gameObject and it parents");
            if (_logEvent == null)
                throw new System.ArgumentNullException(nameof(_logEvent));
            if (string.IsNullOrWhiteSpace(_logTextElementId))
                throw new System.Exception($"{nameof(_logTextElementId)} should not be Empty");
        }
    }
}
