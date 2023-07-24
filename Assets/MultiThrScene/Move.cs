using UKnack.Attributes;
using UKnack.Events;
using UnityEngine;

namespace InGame
{
    public class Move : MonoBehaviour, ISubscriberToEvent<Vector3>
    {
        [SerializeField]
        [ValidReference]
        private SOEvent<Vector3> _move;

        [SerializeField]
        [ProvidedComponent]
        private Transform _providedTransform;

        private string _description = string.Empty;
        public string Description => _description;

        public void OnEventNotification(Vector3 t)
        {
            _providedTransform.Translate(t);
        }

        private void OnEnable()
        {
            if (_providedTransform == null)
                _providedTransform = transform;

            _description = $"{gameObject.name}-{nameof(Move)}";
            if ( _move == null )
                throw new System.ArgumentNullException(nameof( _move) );
            _move.Subscribe( this );
        }

        private void OnDisable()
        {
            _move.UnsubscribeNullSafe(this);
        }
    }
}
