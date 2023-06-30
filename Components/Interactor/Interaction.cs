using Bible_Blazer_PWA.Components.Interactor.Transitions;
using BibleComponents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class Interaction : IInteractionCoordinator
    {
        #region Events
        public static event Action OnModelChanged;
        private void ModelChanged() => OnModelChanged?.Invoke();
        #endregion

        #region Public State
        public static bool HasPrevious { get => Instance.CurrentModel.Previous is not null; }
        public static bool HasNext { get => Instance.CurrentModel.Next is not null; }
        public static InteractionPanel GetInteractionPanel() => Instance.Container;
        #endregion

        #region Public Methods
        public static void Enlarge()
        {
            Instance.Container.Size = Instance.Container.Size + 1;
        }

        public static void Shrink()
        {
            Instance.Container.Size = Instance.Container.Size - 1;
        }

        #endregion

        #region Ctor
        public Interaction(InteractionPanel container)
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
        #endregion

        #region Private State

        private InteractionPanel Container;
        private static Interaction Instance;
        private IInteractionModel CurrentModel
        {
            get => currentModel;
            set
            {
                bool changed = !Object.ReferenceEquals(currentModel, value);                
                currentModel = value;
                if (changed)
                {
                    ModelChanged();
                }
            }
        }
        private Dictionary<Type, List<Type>> transitions;
        private IInteractionModel currentModel;

        #endregion

        #region Navigation

        private void GoToPreviousImpl()
        {
            if (CurrentModel is not Command)
            {
                CurrentModel.Previous.Next = CurrentModel;
            }
            CurrentModel = CurrentModel.Previous;
            Container.SetInteractionModel(CurrentModel);
            Container.Refresh();
        }

        public static void GoToPrevious()
        {
            Instance.GoToPreviousImpl();
        }
        private void GoToNextImpl()
        {
            if (CurrentModel is not Command)
            {
                CurrentModel.Next.Previous = CurrentModel;
            }
            CurrentModel = CurrentModel.Next;
            Container.SetInteractionModel(CurrentModel);
            Container.Refresh();
        }

        public static void GoToNext()
        {
            Instance.GoToNextImpl();
        }

        #endregion

        #region Nested Classes
        public static class ModelOfType<TInteractionModel> where TInteractionModel : IInteractionModel, new()
        {
            public static class WithParameters<TParameters> where TParameters : IInteractionModelParameters<TInteractionModel>
            {
                public static TInteractionModel Apply(TParameters parameters)
                {
                    return Instance.SetInteractionModel<TInteractionModel, TParameters>(parameters);
                }
            }
        }
        #endregion

        internal static void RemoveCurrent()
        {
            Instance.RemoveCurrentImpl();
        }
        internal void RemoveCurrentImpl()
        {
            if (CurrentModel.Previous?.Next != null)
                CurrentModel.Previous.Next = CurrentModel.Previous.Next.Next;
            if (CurrentModel.Previous?.Next != null)
                CurrentModel.Previous.Next.Previous = CurrentModel.Previous.Next;
            CurrentModel = CurrentModel.Previous;
            Container.SetInteractionModel(CurrentModel);
            Container.Refresh();
        }
        protected TInteractionModel SetInteractionModel<TInteractionModel, TParameters>(TParameters parameters)
            where TInteractionModel : IInteractionModel, new()
            where TParameters : IInteractionModelParameters<TInteractionModel>
        {
            var model = new TInteractionModel();
            parameters.ApplyParametersToModel(model);
            ApplyTransitions(model);
            model.Previous = CurrentModel;
            Container.SetInteractionModel(model);
            model.OnClose += () =>
            {
                Container.SetInteractionModel(null);
                Container.Refresh();
            };
            Container.Refresh();
            CurrentModel = model;
            return model;
        }
        private void ApplyTransitions<TInteractionModel>(TInteractionModel model) where TInteractionModel : IInteractionModel, new()
        {
            if (transitions.ContainsKey(model.GetType()))
            {
                foreach (Type transitionType in transitions[model.GetType()])
                {
                    var transition = (Transition<TInteractionModel>)Activator.CreateInstance(transitionType);
                    transition.ApplyTransition(model);
                }
            }
        }
    }
}
