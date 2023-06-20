using Bible_Blazer_PWA.Components.Interactor.BibleReferencesWriter;
using Bible_Blazer_PWA.Components.Interactor.Transitions;
using BibleComponents;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class Interaction: IInteractionCoordinator
    {
        private LessonCenteredContainer Container;
        private static Interaction Instance;
        private IInteractionModel currentModel;
        private Dictionary<Type, List<Type>> transitions;
        public Interaction(LessonCenteredContainer container)
        {
            Container = container;
            Instance = this;
            transitions = new();
            IEnumerable<Type> transitionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()).Where(
                    x => x.IsClass
                    && x.BaseType?.IsGenericType == true
                    && x.BaseType.GetGenericTypeDefinition() == typeof(Transition<>));
            foreach (var transitionType in transitionTypes)
            {
                Type modelType = transitionType.BaseType.GetGenericArguments().First();
                if (!transitions.ContainsKey(modelType))
                {
                    transitions.Add(modelType, new List<Type>(1));
                }
                transitions[modelType].Add(transitionType);
            }
        }
        public static LessonCenteredContainer GetLessonCenteredContainer() => Instance.Container;
        protected TInteractionModel SetInteractionModel<TInteractionModel, TParameters>(TParameters parameters)
            where TInteractionModel : IInteractionModel, new()
            where TParameters: IInteractionModelParameters<TInteractionModel>
        {
            var model = new TInteractionModel();
            parameters.ApplyParametersToModel(model);
            ApplyTransitions(model);
            model.Previous = currentModel;
            Container.SetInteractionModel(model);
            model.OnClose += () =>
            {
                Container.SetInteractionModel(null);
                Container.Refresh();
            };
            Container.Refresh();
            currentModel = model;
            return model;
        }

        private void ApplyTransitions<TInteractionModel>(TInteractionModel model) where TInteractionModel : IInteractionModel, new()
        {
            if (transitions.ContainsKey(model.GetType()))
            {
                foreach(Type transitionType in transitions[model.GetType()])
                {
                    var transition = (Transition<TInteractionModel>)Activator.CreateInstance(transitionType);
                    transition.ApplyTransition(model);
                }
            }
        }

        private void GoToPreviousImpl()
        {
            if (currentModel is not Command)
            {
                currentModel.Previous.Next = currentModel;
            }
            currentModel = currentModel.Previous;
            Container.SetInteractionModel(currentModel);
            Container.Refresh();
        }

        public static void GoToPrevious()
        {
            Instance.GoToPreviousImpl();
        }

        internal static void RemoveCurrent()
        {
            Instance.RemoveCurrentImpl();
        }
        internal void RemoveCurrentImpl()
        {
            if (currentModel.Previous?.Next != null)
                currentModel.Previous.Next = currentModel.Previous.Next.Next;
            if (currentModel.Previous?.Next != null)
                currentModel.Previous.Next.Previous = currentModel.Previous.Next;
            currentModel = currentModel.Previous;
            Container.SetInteractionModel(currentModel);
            Container.Refresh();
        }

        public static class ModelOfType<TInteractionModel> where TInteractionModel: IInteractionModel, new()
        {
            public static class WithParameters<TParameters> where TParameters: IInteractionModelParameters<TInteractionModel>
            {
                public static TInteractionModel Apply(TParameters parameters)
                {
                    return Instance.SetInteractionModel<TInteractionModel, TParameters>(parameters);
                }
            }
        }
    }
}
