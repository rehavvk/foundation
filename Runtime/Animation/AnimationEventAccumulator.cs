using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Foundation.Animation
{
    public delegate void AnimationEventListenerDelegate();
    
    public class AnimationEventAccumulator : MonoBehaviour
    {
        private readonly Dictionary<AnimationEvent, List<AnimationEventListenerDelegate>> listeners = new Dictionary<AnimationEvent, List<AnimationEventListenerDelegate>>();
        
        public void InvokeEvent(AnimationEvent animationEvent)
        {
            if (listeners.TryGetValue(animationEvent, out List<AnimationEventListenerDelegate> resultListeners))
            {
                foreach (AnimationEventListenerDelegate listener in resultListeners)
                {
                    listener.Invoke();
                }
            }
        }
        
        public void Subscribe(AnimationEvent animationEvent, AnimationEventListenerDelegate callback)
        {
            if (!animationEvent)
                return;
            
            if (!listeners.ContainsKey(animationEvent))
            {
                listeners.Add(animationEvent, new List<AnimationEventListenerDelegate>
                {
                    callback
                });   
            }
            else
            {
                listeners[animationEvent].Add(callback);
            }
        }
        
        public void Unsubscribe(AnimationEvent animationEvent, AnimationEventListenerDelegate callback)
        {
            if (!animationEvent)
                return;

            if (listeners.TryGetValue(animationEvent, out List<AnimationEventListenerDelegate> resultListeners))
            {
                resultListeners.Remove(callback);
            }
        }
    }
}