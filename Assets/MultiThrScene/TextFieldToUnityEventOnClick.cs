using System.Collections;
using System.Collections.Generic;
using UKnack.Preconcrete.UI.SimpleToolkit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace InGame
{
    public class TextFieldToUnityEventOnClick : EffortlessButtonClick
    {
        [SerializeField]
        private string _textFieldId;

        [SerializeField]
        private UnityEvent<string> _unityEvent;

        private TextField _textField;
        protected override void ButtonClicked()
        {
            _unityEvent?.Invoke(_textField.text);
        }

        protected override void LayoutReady(VisualElement layout)
        {
            if (string.IsNullOrWhiteSpace(_textFieldId))
                throw new System.Exception($"{nameof(_textFieldId)} should not be Empty");
            _textField = layout.Q<TextField>(_textFieldId);
            ValidateTextField();
            base.LayoutReady(layout);
        }

        protected override void LayoutGonnaBeDestroyedNow()
        {
            ValidateTextField();
            _textField = null;
            base.LayoutGonnaBeDestroyedNow();
        }

        private void ValidateTextField()
        {
            if (_textField == null)
                throw new System.Exception($"TextField with id {_textFieldId} not found in layout");
        }
    }
}
