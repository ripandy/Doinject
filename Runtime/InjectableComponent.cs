﻿using System.Linq;
using UnityEngine;

namespace Doinject
{
    public class InjectableComponent : MonoBehaviour, IInjectableComponent
    {
        private bool Injected { get; set; }

        [Inject]
        public void Construct()
        {
            Injected = true;
        }

        private async void Start()
        {
            if (Injected) return;
            Injected = true;

            var context = FindParentContext();
            foreach (var component in GetComponents(typeof(IInjectableComponent)).Cast<IInjectableComponent>())
            {
                if ((InjectableComponent)component == this) continue;
                await context.Context.Container.InjectIntoAsync(component);
            }
        }

        private IContext FindParentContext()
        {
            if (transform.parent)
            {
                var parentContext = transform.parent.GetComponentInParent<GameObjectContext>();
                if (parentContext) return parentContext;
            }

            if (SceneContext.TryGetSceneContext(gameObject.scene, out var sceneContext))
                return sceneContext;

            return ProjectContext.Instance;
        }
    }
}