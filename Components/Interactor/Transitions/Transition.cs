namespace Bible_Blazer_PWA.Components.Interactor.Transitions
{
    public abstract class Transition<TFrom>
    {
        public abstract void ApplyTransition(TFrom source);
    }
}