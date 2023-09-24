using Bible_Blazer_PWA.Components.Interactor.Menu;
using Bible_Blazer_PWA.Components.Interactor.Transitions;
using Bible_Blazer_PWA.Services;
using BibleComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public class Interaction : IInteractionCoordinator
    {
        #region Events
        public static event Action OnModelChanged;
        private void ModelChanged() => OnModelChanged?.Invoke();
        
        public static event Action<int> OnResize;
        private static void Resize(int size) => OnResize?.Invoke(size);

        public static event Action OnTurnOver;
        public static void TurnOver() => OnTurnOver?.Invoke();

        #endregion

        #region Public State
        public static bool HasPrevious { get => Instance.CurrentModel.Previous is not null; }
        public static bool HasNext { get => Instance.CurrentModel.Next is not null; }
        public static InteractionPanel GetInteractionPanel() => Instance.Container;
        #endregion

        #region Public Methods
        public static void Enlarge(InteractionPanelMenu menu, int sizePoints)
        {
            Instance.Container.Size = Instance.Container.Size + sizePoints;
            Resize(Instance.Container.Size);
        }

        public static void Shrink(InteractionPanelMenu menu, int sizePoints)
        {
            Instance.Container.Size = Instance.Container.Size - sizePoints;
            Resize(Instance.Container.Size);
        }

        public static InteractionConfig GetConfig() => Instance.Config;

        #endregion

        #region Ctor
        public Interaction(InteractionPanel container)
        {
            Container = container;

            Instance = this;
            Config = new();
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
            container.ChildInitialized += () => Resize(container.Size);
            container.SetInteractionModel(null);
        }
        #endregion

        #region Private State

        private InteractionPanel Container;
        private static Interaction Instance;
        private InteractionConfig Config;
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
            public static TInteractionModel Apply()
            {
                return Instance.SetInteractionModel<TInteractionModel>();
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
        protected TInteractionModel SetInteractionModel<TInteractionModel>()
            where TInteractionModel : IInteractionModel, new()
        {
            var model = new TInteractionModel();
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
