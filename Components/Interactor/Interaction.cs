using Bible_Blazer_PWA.Components.Interactor.Menu;
using Bible_Blazer_PWA.Components.Interactor.Transitions;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Bible_Blazer_PWA.Components.Interactor.Interaction;

namespace Bible_Blazer_PWA.Components.Interactor
{
    public partial class Interaction : IInteractionCoordinator
    {
        #region Events
        public static event Action OnMainModelChanged;
        private void MainModelChanged() => OnMainModelChanged?.Invoke();
        public static event Action OnSideModelChanged;
        private void SideModelChanged() => OnSideModelChanged?.Invoke();

        public static event Action<int> OnResize;
        private static void Resize(int size) => OnResize?.Invoke(size);

        public static event Action OnTurnOver;
        public static void TurnOver() => OnTurnOver?.Invoke();

        #endregion

        #region Public State
        public static bool HasPrevious { get => Instance.CurrentSideModel?.Previous is not null; }
        public static bool HasNext { get => Instance.CurrentSideModel?.Next is not null; }
        public static InteractionPanel GetInteractionPanel() => Instance.SideContainer;
        #endregion

        #region Public Methods
        public static void Enlarge(InteractionPanelMenu menu, int sizePoints)
        {
            Instance.SideContainer.Size = Instance.SideContainer.Size + sizePoints;
            Resize(Instance.SideContainer.Size);
        }

        public static void Shrink(InteractionPanelMenu menu, int sizePoints)
        {
            Instance.SideContainer.Size = Instance.SideContainer.Size - sizePoints;
            Resize(Instance.SideContainer.Size);
        }

        public static InteractionConfig GetConfig() => Instance.Config;
        public static void SetMainContainer(MainContentPanel mainContainer) { Instance.MainContainer = mainContainer; }

        #endregion





        #region Ctor
        public Interaction(InteractionPanel sideContainer)
        {
            SideContainer = sideContainer;

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
            sideContainer.ChildInitialized += () => Resize(sideContainer.Size);
            sideContainer.SetInteractionModel(null);
        }
        #endregion

        #region Private State

        private InteractionPanel SideContainer;
        private MainContentPanel MainContainer;
        private static Interaction Instance;
        private InteractionConfig Config;
        private IInteractionModel CurrentSideModel
        {
            get => currentSideModel;
            set
            {
                bool changed = !Object.ReferenceEquals(currentSideModel, value);
                currentSideModel = value;
                if (changed)
                {
                    SideModelChanged();
                }
            }
        }

        private IInteractionModel CurrentMainModel
        {
            get => currentMainModel;
            set
            {
                bool changed = !Object.ReferenceEquals(currentMainModel, value);
                currentMainModel = value;
                if (changed)
                {
                    MainModelChanged();
                }
            }
        }

        private Dictionary<Type, List<Type>> transitions;
        private IInteractionModel currentSideModel;
        private IInteractionModel currentMainModel;

        #endregion

        #region Navigation

        private void GoToPreviousImpl()
        {
            if (CurrentSideModel.ShouldPersistInHistory)
            {
                CurrentSideModel.Previous.Next = CurrentSideModel;
            }
            CurrentSideModel = CurrentSideModel.Previous;
            SideContainer.SetInteractionModel(CurrentSideModel);
            SideContainer.Refresh();
        }

        public static void GoToPrevious()
        {
            Instance.GoToPreviousImpl();
        }
        private void GoToNextImpl()
        {
            if (CurrentSideModel.ShouldPersistInHistory)
            {
                CurrentSideModel.Next.Previous = CurrentSideModel;
            }
            CurrentSideModel = CurrentSideModel.Next;
            SideContainer.SetInteractionModel(CurrentSideModel);
            SideContainer.Refresh();
        }

        public static void GoToNext()
        {
            Instance.GoToNextImpl();
        }

        #endregion

        #region Nested Classes

        public abstract class InteractionModel<TSelf>
            where TSelf : InteractionModel<TSelf>, IInteractionModel
        {
            public abstract class Parameters
            {
                public abstract void ApplyParametersToModel(TSelf model);
                public Type GetModelType() => typeof(TSelf);
            }

            public abstract class WithParameters<TParameters>
                where TParameters : Parameters
            {
                public static TSelf Apply(TParameters parameters, bool toMainContent = false)
                {
                    return Instance.SetInteractionModel<TSelf, TParameters>(parameters, toMainContent);
                }
            }
            public static TSelf Apply(bool toMainContent = false)
            {
                return Instance.SetInteractionModel<TSelf>(toMainContent);
            }
        }

        #endregion

        internal static void RemoveCurrent()
        {
            Instance.RemoveCurrentImpl();
        }
        internal void RemoveCurrentImpl()
        {
            if (CurrentSideModel.Previous?.Next != null)
                CurrentSideModel.Previous.Next = CurrentSideModel.Previous.Next.Next;
            if (CurrentSideModel.Previous?.Next != null)
                CurrentSideModel.Previous.Next.Previous = CurrentSideModel.Previous.Next;
            CurrentSideModel = CurrentSideModel.Previous;
            SideContainer.SetInteractionModel(CurrentSideModel);
            SideContainer.Refresh();
        }
        protected TInteractionModel SetInteractionModel<TInteractionModel>(bool toMainContent)
            where TInteractionModel : IInteractionModel
        {
            TInteractionModel model = Activator.CreateInstance<TInteractionModel>();
            ApplyTransitions(model);
            model.Previous = CurrentSideModel;
            InteractionContainerComponent container = toMainContent ? MainContainer : SideContainer;
            container.SetInteractionModel(model);
            model.OnClose += () =>
            {
                container.SetInteractionModel(null);
                container.Refresh();
            };
            container.Refresh();
            CurrentSideModel = toMainContent ? CurrentSideModel : model;
            CurrentMainModel = toMainContent ? model : CurrentSideModel;            
            return model;
        }
        protected TInteractionModel SetInteractionModel<TInteractionModel, TParameters>(InteractionModel<TInteractionModel>.Parameters parameters, bool toMainContent = false)
            where TInteractionModel : InteractionModel<TInteractionModel>, IInteractionModel
            where TParameters : InteractionModel<TInteractionModel>.Parameters
        {
            TInteractionModel model = Activator.CreateInstance<TInteractionModel>();
            parameters.ApplyParametersToModel(model);
            ApplyTransitions(model);
            model.Previous = CurrentSideModel;
            InteractionContainerComponent container = toMainContent ? MainContainer : SideContainer;
            container.SetInteractionModel(model);
            model.OnClose += () =>
            {
                container.SetInteractionModel(null);
                container.Refresh();
            };
            container.Refresh();
            CurrentSideModel = toMainContent ? CurrentSideModel : model;
            CurrentMainModel = toMainContent ? model : CurrentSideModel;
            return model;
        }
        private void ApplyTransitions<TInteractionModel>(TInteractionModel model) where TInteractionModel : IInteractionModel
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
